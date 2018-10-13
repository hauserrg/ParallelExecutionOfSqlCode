using System;
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
        private ConcurrentDictionary<SingleNode, Task> nodes;
        private DI di;
        private NodeFlowLogger log;
        private List<int> CompleteIdsFromPriorRun;

        internal NodeFlow(ConcurrentDictionary<SingleNode, Task> nodes, DI di)
        {
            this.nodes = nodes;
            this.di = di;
            this.log = new NodeFlowLogger(di);
        }

        internal void Run()
        {
            try
            {
                //Allow restart from a previous failed run.
                CompleteIdsFromPriorRun = NodeFlowReader.LogReader(di);
                if (CompleteIdsFromPriorRun.Count > 0)
                    log.Log("Loaded completed nodes from prior run ('log.txt')");

                //Run nodes
                while (nodes.Count > 0)
                {
                    var nodesToRun = FindNodesToRun();
                    foreach (var nodeToRun in nodesToRun)
                    {
                        //Run the node
                        var task = RunNodeAsync(nodeToRun);
                        nodes[nodeToRun] = task;
                    }
                    Task.WaitAny(nodes.Values.Where(x => x != null).ToArray());

                    //Look for errors in tasks
                    foreach (var nodeTask in nodes.Where(x => x.Value != null))
                    {
                        if(nodeTask.Value.Status == TaskStatus.Faulted)
                        {
                            nodeTask.Key.EndState = EndState.Error;
                            log.NodeEnd(nodeTask.Key);
                            throw new Exception();
                        }
                    }
                }
            }
            catch (Exception)
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
            if (CompleteIdsFromPriorRun.Contains(nodeToRun.Id))
            {
                await RunPriorCompleteAsync(nodeToRun);
            }
            else if (di.Debug)
            {
                await RunDebugAsync(nodeToRun);
            }
            else
            {
                await RunSQLAsync(nodeToRun);
            }
            RunAsyncComplete(nodeToRun);
        }

        private async Task RunPriorCompleteAsync(SingleNode nodeToRun)
        {
            log.NodePriorComplete(nodeToRun);
            await Task.Delay(100);
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
            nodeToRun.EndState = EndState.Success; //1.
            log.NodeEnd(nodeToRun);
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

            nodeToRun.EndState = EndState.Success;
            log.NodeEnd(nodeToRun);
        }

        /// <summary>
        /// Update the log
        /// Remove the node from the list
        /// Remove the node from the beginning of other nodes
        /// </summary>
        private void RunAsyncComplete(SingleNode nodeToRun)
        {
            var removedQ = nodes.TryRemove(nodeToRun, out Task nodeRemoved); //2.
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