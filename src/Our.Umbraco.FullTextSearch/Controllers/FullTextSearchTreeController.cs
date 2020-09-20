using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Trees;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using System.Web.Http.ModelBinding;
using Umbraco.Web.WebApi.Filters;
using System.Net.Http.Formatting;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.FullTextSearch.Controllers
{
    [Tree(
        Constants.Applications.Settings,
        "fullTextSearch",
        IsSingleNodeTree = true,
        TreeGroup = Constants.Trees.Groups.ThirdParty,
        TreeTitle = "Full Text Search",
        TreeUse = TreeUse.Main)]
    [PluginController("FullTextSearch")]
    public class FullTextSearchTreeController : TreeController
    {
        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var root = base.CreateRootNode(queryStrings);

            root.Icon = "icon-search";
            root.HasChildren = false;
            root.RoutePath = $"{SectionAlias}/{TreeAlias}/index";
            root.MenuUrl = null;

            return root;
        }
        protected override MenuItemCollection GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            return null;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            return null;
        }
    }
}
