using Our.Umbraco.FullTextSearch.Models;
using System.Collections.Generic;

namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface ISearch
    {
        SearchType SearchType { get; set; }
        string SearchTerm { get; set; }
        ICollection<string> SearchTermQuoted { get; }
        ICollection<string> SearchTermSplit { get; }
        string[] TitleProperties { get; set; }
        double TitleBoost { get; set; }
        string[] BodyProperties { get; set; }
        string[] SummaryProperties { get; set; }
        int[] RootNodeIds { get; set; }
        int SummaryLength { get; set; }
        int PageLength { get; set; }
        double Fuzzyness { get; set; }
        bool AddWildcard { get; set; }
        bool HighlightSearchTerms { get; set; }
        string Culture { get; set; }
        string[] AllowedContentTypes { get; set; }
        string Searcher { get; set; }
        string Index { get; set; }

        Search SetIndex(string index);
        Search SetSearcher(string searcher);
    }
}
