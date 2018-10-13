using SharedLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelExecutionOfSqlCode
{
    public class NodeFlowReader
    {
        /// <summary>
        /// Ensure the folder contents meet the specification
        /// </summary>
        internal static void Validate(DI di)
        {
            //ToDo
            /*Is the path valid - error
             * Contains metadata.txt file - error
             * contains named list of files - error
             * all files are names - warning
             * contains order of execution - error
             * codes are internally valid - error
             * at least 1 start - error
             * at least 1 end - error
             * all before are afters - error
             * all numbers are in the named list of files - error
             * */
        }

        /// <summary>
        /// Returns a list of integers that represent the Ids of node completed in a previous run.
        /// For example, if the log file says "Node 1 has completed (Success)" then node 1 would be marked as previously run.
        /// </summary>
        /// <param name="di"></param>
        /// <returns></returns>
        internal static List<int> LogReader(DI di)
        {
            var completeIds = new List<int>();
            if (File.Exists(di.LogFilePath()))
            {
                var lines = File.ReadAllLines(di.LogFilePath());
                foreach (var line in lines)
                {
                    if(line.Contains("(Success)"))
                    {
                        var token = line.Split();
                        int id;
                        Int32.TryParse(token[1], out id);
                        completeIds.Add(id);
                    }
                }
            }
            return completeIds;
        }

        /// <summary>
        /// Load the metadata into NodeFlow
        /// </summary>
        internal static ConcurrentDictionary<SingleNode, Task> Load(DI di)
        {
            var singleNodeXml = SerializeHelper.Deserialize<List<SingleNodeXml>>(File.ReadAllText(di.MetadataFilePath()));
            return SingleNode.ConvertXml(singleNodeXml);
        }
    }
}