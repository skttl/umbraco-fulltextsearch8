using Our.Umbraco.FullTextSearch.Options;

namespace Our.Umbraco.FullTextSearch.SchemaGenerator;

internal class AppSettings
{
    public UmbracoDefinition Umbraco { get; set; }

    /// <summary>
    /// Configuration of settings
    /// </summary>
    internal class UmbracoDefinition
    {
        /// <summary>
        /// FullTextSearch settings
        /// </summary>
        public FullTextSearchOptions FullTextSearch { get; set; }

    }
}
