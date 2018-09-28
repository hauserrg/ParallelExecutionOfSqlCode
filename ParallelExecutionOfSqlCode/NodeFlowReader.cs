using SharedLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            /*Is the path avlid - error
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
        /// Load the metadata into NodeFlow
        /// </summary>
        internal static ConcurrentDictionary<SingleNode, bool> Load(DI di)
        {
            var singleNodeXml = SerializeHelper.Deserialize<List<SingleNodeXml>>(File.ReadAllText(di.MetadataFilePath()));
            return SingleNode.ConvertXml(singleNodeXml);
        }
    }
}