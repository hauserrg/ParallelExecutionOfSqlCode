using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParallelExecutionOfSqlCode
{
    public class DI
    {
        public bool Debug { get; set; }
        public string ConnectionString { get; set; }

        /// <summary>
        /// No backslash
        /// </summary>
        public string FolderPath { get; set; }

        private string metadataFile = "metadata.txt";
        private string logFile = "log.txt";

        /// <summary>
        /// Create the file path by appending the folder to the file
        /// </summary>
        public string CreateFilePath(string fileName)
        {
            return FolderPath + @"\" + fileName;
        }

        public string MetadataFilePath()
        {
            return CreateFilePath(metadataFile);
        }
        public string LogFilePath()
        {
            return CreateFilePath(logFile);
        }
    }
}