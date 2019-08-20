using Examine;
using Examine.LuceneEngine.Providers;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;
using System.Collections.Generic;
using Umbraco.Examine;

namespace Our.Umbraco.FullTextSearch.Index
{
    public class FullTextIndexCreator : LuceneIndexCreator
    {
        public override IEnumerable<IIndex> Create()
        {
            var index = new LuceneIndex("FullTextIndex",
                CreateFileSystemLuceneDirectory("FullTextIndex"),
                new FieldDefinitionCollection(
                    new FieldDefinition("carId", FieldDefinitionTypes.FullText),
                    new FieldDefinition("brand", FieldDefinitionTypes.FullText),
                    new FieldDefinition("serie", FieldDefinitionTypes.FullText),
                    new FieldDefinition("engine", FieldDefinitionTypes.FullText),
                    new FieldDefinition("carInfo", FieldDefinitionTypes.FullText)
                ),
                new StandardAnalyzer(Version.LUCENE_30));

            return new[] { index };
        }
    }
}
