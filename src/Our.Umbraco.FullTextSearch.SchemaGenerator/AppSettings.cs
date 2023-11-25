using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Our.Umbraco.FullTextSearch.Options;

namespace Our.Umbraco.FullTextSearch.SchemaGenerator
{
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

}
