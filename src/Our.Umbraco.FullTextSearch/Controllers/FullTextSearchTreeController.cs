using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.ModelBinders;

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
        public FullTextSearchTreeController(ILocalizedTextService localizedTextService, UmbracoApiControllerTypeCollection types, IEventAggregator eventAggregator) : base(localizedTextService, types, eventAggregator)
        {

        }
        protected override ActionResult<TreeNode> CreateRootNode(FormCollection queryStrings)
        {
            var root = base.CreateRootNode(queryStrings);

            root.Value.Icon = "icon-search";
            root.Value.HasChildren = false;
            root.Value.RoutePath = $"{SectionAlias}/{TreeAlias}/index";
            root.Value.MenuUrl = null;

            return root;
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, [Microsoft.AspNetCore.Mvc.ModelBinder(typeof(HttpQueryStringModelBinder))] FormCollection queryStrings)
        {
            return null;
        }

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, [Microsoft.AspNetCore.Mvc.ModelBinder(typeof(HttpQueryStringModelBinder))] FormCollection queryStrings)
        {
            return null;
        }
    }
}
