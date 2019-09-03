using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Our.Umbraco.FullTextSearch.Models
{
    [TableName("FullTextCache")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class CacheItem
    {
        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("NodeId")]
        public int NodeId { get; set; }

        [Column("Culture")]
        public string Culture { get; set; }

        [Column("Text")]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string Text { get; set; }

        [Column("LastUpdated")]
        public DateTime LastUpdated { get; set; }
    }
}
