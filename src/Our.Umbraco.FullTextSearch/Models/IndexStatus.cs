using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.FullTextSearch.Models
{
    public class IndexStatus
    {
        public long TotalIndexableNodes { get; set; }
        public long TotalIndexedNodes { get; set; }
        public long IncorrectIndexedNodes { get; set; }
        public long MissingIndexedNodes { get; set; }
    }
}
