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
        string[] BodyProperties { get; set; }
        string[] SummaryProperties { get; set; }
        int[] RootNodeIds { get; set; }
        int SummaryLength { get; set; }
        int PageLength { get; set; }
        double Fuzzyness { get; set; }
        bool AddWildcard { get; set; }
    }
}
