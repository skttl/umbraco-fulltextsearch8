using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web.JavaScript;

namespace Our.Umbraco.FullTextSearch
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ServerVariablesComposer : ComponentComposer<ServerVariablesComponent>
    {
    }

    public class ServerVariablesComponent : IComponent
    {
        public void Initialize()
        {
            ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;
        }

        private void ServerVariablesParser_Parsing(object sender, Dictionary<string, object> e)
        {
            e.Add("FullTextSearch", new Dictionary<string, string>
            {
                { "Version", Assembly.GetExecutingAssembly().GetName().Version.ToString() }
            });
        }

        public void Terminate()
        {
        }
    }
}
