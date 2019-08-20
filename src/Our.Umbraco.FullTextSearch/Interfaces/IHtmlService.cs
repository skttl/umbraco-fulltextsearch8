namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface IHtmlService
    {
        bool GetHtmlByUrl(string url, out string fullHtml);
        string GetTextFromHtml(string fullHtml);
    }
}
