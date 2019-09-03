using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Our.Umbraco.FullTextSearch.Models
{
    [TableName("FullTextCacheTasks")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class CacheTask
    {
        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("NodeId")]
        public int NodeId { get; set; }

        [Column("Started")]
        public bool Started { get; set; }
    }
}
