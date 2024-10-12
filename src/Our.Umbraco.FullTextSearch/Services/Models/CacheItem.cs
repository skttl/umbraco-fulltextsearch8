using NPoco;
using System;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Our.Umbraco.FullTextSearch.Services.Models;

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
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public string Text { get; set; }

    [Column("LastUpdated")]
    public DateTime LastUpdated { get; set; }
}
