using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Our.Umbraco.FullTextSearch
{
    public class Composer : IUserComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddFullTextSearch();
        }
    }
}
