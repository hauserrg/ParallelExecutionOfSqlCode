using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelExecutionOfSqlCode
{
    internal class NodeFlowLogger
    {
        internal List<int> nodesRunning;
        private List<string> report;
        private DI di;

        internal NodeFlowLogger(DI di)
        {
            this.nodesRunning = new List<int>();
            this.report = new List<string>();
            this.di = di;
        }

        internal void WriteToFile()
        {
            File.WriteAllLines(di.LogFilePath(), report.ToArray());
        }

        internal void NodeStart(SingleNode singleNode)
        {
            nodesRunning.Add(singleNode.Id);
            singleNode.TimeStart = DateTime.Now;
            this.Log(String.Format("Node {0} has started.", singleNode.Id));
        }

        internal void NodeEnd(SingleNode singleNode)
        {
            nodesRunning.Remove(singleNode.Id);
            singleNode.TimeEnd = DateTime.Now;
            this.Log(String.Format("Node {0} has completed ({1})({2} -> {3}).", singleNode.Id, singleNode.EndState, singleNode.TimeStart, singleNode.TimeEnd));
        }

        internal void NodePriorComplete(SingleNode singleNode)
        {
            this.Log(String.Format("Node {0} has completed in a prior run (Success)", singleNode.Id.ToString()));
        }

        /// <summary>
        /// When an error occurs log,
        /// 1. Change the nodes state to error, if it is was running during the error
        /// 2. If the node had not yet started to run, log this fact
        /// </summary>
        /// <param name="nodes"></param>
        internal void Error(ConcurrentDictionary<SingleNode, Task> nodes)
        {
            foreach (var node in nodes)
            {
                if (nodesRunning.Contains(node.Key.Id))
                {
                    node.Key.EndState = EndState.StoppedByError;
                    node.Key.TimeEnd = DateTime.Now;
                    this.Log(String.Format("Node {0} has completed ({1})({2} -> {3}).", node.Key.Id, node.Key.EndState, node.Key.TimeStart, node.Key.TimeEnd));
                }
            }
            this.Log("No other nodes were run.");
        }

        internal void Log(string message)
        {
            report.Add(message);
        }
    }
}