namespace Our.Umbraco.FullTextSearch.Models
{
    /// <summary>
    /// Some data indicating how to process a given document property from umbraco in search
    /// </summary>
    public class SearchProperty
    {
        public string PropertyName { get; private set; }
        public double BoostMultiplier { get; private set; }
        public double FuzzyMultiplier { get; private set; }
        public bool Wildcard { get; set; }
        public SearchProperty(string propertyName, double boostMultipler = 1.0, double fuzzyMultipler = 1.0, bool wildcard = false)
        {
            PropertyName = propertyName;
            BoostMultiplier = boostMultipler;
            FuzzyMultiplier = fuzzyMultipler;
            Wildcard = wildcard;
        }
    }
}