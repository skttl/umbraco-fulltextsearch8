using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.FullTextSearch.Controllers.Models
{
    public class IndexedNodeResult
    {
        public List<IndexedNode> Items { get; set; }
        public int PageNumber { get; set; }
        public long TotalPages { get; set; }

        public IndexedNodeResult()
        {
            Items = new List<IndexedNode>();
        }
    }
}
