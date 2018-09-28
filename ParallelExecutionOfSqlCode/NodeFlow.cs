﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelExecutionOfSqlCode
{
    public class NodeFlow
    {
        private ConcurrentDictionary<SingleNode, bool> nodes;
        private DI di;
        private NodeFlowLogger log;

        internal NodeFlow(ConcurrentDictionary<SingleNode, bool> nodes, DI di)
        {
            this.nodes = nodes;
            this.di = di;
            this.log = new NodeFlowLogger(di);
        }

        #pragma warning disable CS4014, CS1998
        internal void Run()
        {
            try
            {
                while(nodes.Count > 0)
                {
                    var nodesToRun = FindNodesToRun();
                    foreach (var nodeToRun in nodesToRun)
                    {
                        var task = RunNodeAsync(nodeToRun);
                        if (task.Exception != null)
                        {
                            nodeToRun.EndState = EndState.Error;
                            log.NodeEnd(nodeToRun);
                            throw new Exception();
                        }
                    }
                    Thread.Sleep(500);
                }
            }
            catch (Exception e)
            {
                log.Error(nodes);
            }
            finally
            {
                log.WriteToFile();
            }
        }

        private async Task RunNodeAsync(SingleNode nodeToRun)
        {
            if (di.Debug)
            {
                await RunDebugAsync(nodeToRun);
                return;
            }
            else
            {
                await RunSQLAsync(nodeToRun);
                return;
            }
        }

        /// <summary>
        /// Runs the SQL code associated with the node
        /// </summary>
        /// <param name="nodeToRun"></param>
        /// <returns></returns>
        private async Task RunSQLAsync(SingleNode nodeToRun)
        {
            log.NodeStart(nodeToRun);
            var sqlCode = File.ReadAllText(di.CreateFilePath(nodeToRun.FileName));
            await SharedLibrary.SqlTable.ExecuteNonQueryAsync(sqlCode, di.ConnectionString);
            RunAsyncComplete(nodeToRun);
        }

        /// <summary>
        /// Runs the node under debug conditions
        /// </summary>
        /// <param name="nodeToRun"></param>
        /// <returns></returns>
        private async Task RunDebugAsync(SingleNode nodeToRun)
        {
            log.NodeStart(nodeToRun);

            //Wait for the prescribed amount of time
            var text = File.ReadAllText(di.CreateFilePath(nodeToRun.FileName));
            if (text.ToLower() == "error")
                throw new Exception();
            else
            {
                var sleep = Int32.Parse(text);
                await Task.Delay(sleep);
            }

            RunAsyncComplete(nodeToRun);
        }

        /// <summary>
        /// Update the log
        /// Remove the node from the list
        /// Remove the node from the beginning of other nodes
        /// </summary>
        private void RunAsyncComplete(SingleNode nodeToRun)
        {
            nodeToRun.EndState = EndState.Success; //1.
            log.NodeEnd(nodeToRun);

            nodes.TryRemove(nodeToRun, out bool nodeRemoved); //2.
            foreach (var node in nodes) //3.
            {
                if (node.Key.Before.ContainsKey(nodeToRun.Id))
                    node.Key.Before.TryRemove(nodeToRun.Id, out bool beforeIdRemoved);
            }
        }

        /// <summary>
        /// Identifies nodes that are ready to run
        /// </summary>
        /// <returns></returns>
        private List<SingleNode> FindNodesToRun()
        {
            var nodesToRun = new List<SingleNode>();
            foreach (var node in nodes)
            {
                //the node is ready to run and not already running
                if (node.Key.Before.Count == 0 && !log.nodesRunning.Contains(node.Key.Id))
                    nodesToRun.Add(node.Key);
            }
            return nodesToRun;
        }
    }
}