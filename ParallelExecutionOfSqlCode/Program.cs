using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelExecutionOfSqlCode
{
    class Program
    {
        static void Main(string[] args)
        {
            //You could host this on SharePoint and allow jobs to schedule

            //Note: "log.txt" and "metadata.txt" are reserved names within the folder.
            args = new string[3];
            args[0] = "true";
            args[1] = "";
            args[2] = @"C:\Users\George\Desktop\NodeFlowFolder";

            var di = new DI()
            {
                Debug = args[0] == "true" ? true : false,
                ConnectionString = args[1],
                FolderPath = args[2]
            };
            
            var nodes = NodeFlowReader.Load(di);
            NodeFlowReader.Validate(di);
            var nodeFlow = new NodeFlow(nodes, di);
            nodeFlow.Run();
        }
    }
}
