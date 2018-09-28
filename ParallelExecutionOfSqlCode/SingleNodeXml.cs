using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParallelExecutionOfSqlCode
{

    public class SingleNodeXml
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
        public List<int> Before { get; set; }

        /// <summary>
        /// Ids to run after running this SQL file
        /// </summary>
        public List<int> After { get; set; }
    }
}