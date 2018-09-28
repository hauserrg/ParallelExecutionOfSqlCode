using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ParallelExecutionOfSqlCode
{
    internal enum EndState { NotRun, Success, Error, StoppedByError };
    internal class SingleNode
    {
        /// <summary>
        /// The name of the SQL file to run for this id
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// A unique identifier for the node
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Ids to run prior to running this SQL file
        /// </summary>
        public ConcurrentDictionary<int, bool> Before { get; set; }

        /// <summary>
        /// Ids to run after running this SQL file
        /// </summary>
        public ConcurrentDictionary<int, bool> After { get; set; }

        internal EndState EndState { get; set; }
        internal DateTime TimeStart { get; set; }
        internal DateTime TimeEnd { get; set; }

        internal static ConcurrentDictionary<SingleNode, bool> ConvertXml(List<SingleNodeXml> singleNodeXml)
        {
            var singleNodes = new ConcurrentDictionary<SingleNode, bool>();
            foreach (var nodeXml in singleNodeXml)
            {
                var singleNode = new SingleNode()
                {
                    Id = nodeXml.Id,
                    FileName = nodeXml.FileName,
                    Before = ConvertListToConcurrentDictionary(nodeXml.Before),
                    After = ConvertListToConcurrentDictionary(nodeXml.After),
                    EndState = EndState.NotRun
                };
                singleNodes.TryAdd(singleNode, false);
            }
            return singleNodes;
        }

        private static ConcurrentDictionary<int, bool> ConvertListToConcurrentDictionary(List<int> list)
        {
            var dict = new ConcurrentDictionary<int, bool>();
            foreach (var listItem in list)
                dict.TryAdd(listItem, false);
            return dict;
        }
    }
}