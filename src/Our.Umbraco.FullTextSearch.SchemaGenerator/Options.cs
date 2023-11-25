using CommandLine;

namespace Our.Umbraco.FullTextSearch.SchemaGenerator
{
    internal class Options
    {
        [Option('o', "outputFile", Required = false,
            HelpText = "",
            Default = "Our.Umbraco.FullTextSearch\\appsettings-schema.umbraco-fulltextsearch.json")]
        public string OutputFile { get; set; }
    }
}
