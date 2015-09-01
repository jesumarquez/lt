#region Usings

using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Logictracker.Culture;
using Logictracker.Types.SecurityObjects;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.App_Controls
{
    public partial class Menu : BaseUserControl
    {
        #region Private Properties

        private Module Module { get { return CurrentPage == null? null : CurrentPage.Module; } }

        private ApplicationSecuredPage CurrentPage { get { return Page as ApplicationSecuredPage; } }

        private string Culture
        {
            get { return (string) ViewState["Culture"]; }
            set { ViewState["Culture"] = value; }
        }

        #endregion

        #region Protected Methods

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Usuario == null) return;

            var sm = ScriptManager.GetCurrent(Page);

            var cultureChanged = Culture != Usuario.Culture.Name;

            if (!cultureChanged && IsPostBack && (sm == null || sm.IsInAsyncPostBack)) return;

            BindItems();

            Culture = Usuario.Culture.Name;

            UpdatePanel1.Update();
        }

        #endregion

        /// <summary>
        /// Binds all the user profile asigned groups and functions.
        /// </summary>
        private void BindItems()
        {
            var modules = from m in Usuario.Modules.Values orderby m.GroupOrder, m.Group, m.ModuleSubGroup, m.ModuleOrder select m;

            var lastGroupId = -1;
            var lastSubGroup = "$NONE$";
            var groupIdx = 0;

            Panel panelGroup = null;
            Panel panelSubGroupContent = null;

            foreach (var module in modules)
            {
                var groupId = module.GroupId;
                var groupName = CultureManager.GetMenu(module.Group);

                if(lastGroupId != groupId)
                {
                    var panel = new AccordionPane { SkinID = "Menu", Header = acpPanel.Header, Content = acpPanel.Content };

                    var headtext = string.Format(
                        @"<table style='width: 100%; font-size: inherit; border-spacing: 0px; padding: 0px;'>
                            <tr>
                                <td style='width: 20px;'>
                                    <img src='{0}' align='absmiddle' />
                                </td>
                                <td>
                                    {1}
                                </td>
                            </tr>
                        </table>", ResolveUrl(string.Concat("~/", module.Group, ".image")), groupName);

                    ((Label) panel.HeaderContainer.FindControl("lblHeader")).Text = headtext;

                    accordion.Panes.Add(panel);

                    panelGroup = (panel.ContentContainer.FindControl("panelContent") as Panel);

                    if (Module != null && Module.GroupId == groupId) accordion.SelectedIndex = groupIdx;

                    groupIdx++;
                }

                var subGroupName = module.ModuleSubGroup;

                if (lastSubGroup != subGroupName || lastGroupId != groupId)
                {
                    var panelSubGroup = new Panel { CssClass = "menu_subgroup"};
                    panelSubGroupContent = new Panel {ID = groupId + "_" + subGroupName.Replace(' ', '_')};

                    if (panelGroup != null) panelGroup.Controls.Add(panelSubGroup);

                    Panel subGroupTitle = null;

                    if (!string.IsNullOrEmpty(subGroupName))
                    {
                        subGroupTitle = new Panel { CssClass = "menu_subgroup_header"};
                        subGroupTitle.Controls.Add(new Literal {Text = CultureManager.GetMenu(subGroupName)});
                        subGroupTitle.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");

                        if(Usuario.MenuStartCollapsed && !(Module != null && Module.GroupId == groupId && subGroupName == Module.ModuleSubGroup))
                        {
                            panelSubGroupContent.Style.Add("display", "none");
                            subGroupTitle.CssClass = "menu_subgroup_header_collapsed";
                        }

                        panelSubGroup.Controls.Add(subGroupTitle);
                    }
                    panelSubGroup.Controls.Add(panelSubGroupContent);


                    if (!string.IsNullOrEmpty(subGroupName) && subGroupTitle != null)
                    {
                        var scriptClick = string.Format(
                            @"if($get('{0}').style.display == 'none')
                                    {{ $get('{0}').style.display = ''; this.className = 'menu_subgroup_header';}}
                                else {{$get('{0}').style.display = 'none'; this.className = 'menu_subgroup_header_collapsed';}}",
                            panelSubGroupContent.ClientID);

                        subGroupTitle.Attributes.Add("onclick", scriptClick);
                    }
                    
                }

                var outsideLink = module.Url.StartsWith("http://") || module.Url.StartsWith("https://");
                var blank = outsideLink || (module.Parameters.IndexOf("_blank", StringComparison.CurrentCultureIgnoreCase) > -1);
                var moduleId = module.Id;
                var moduleName = CultureManager.GetMenu(module.Name);
                var modulePath = outsideLink ? module.Url : GetPath(ApplicationPath, module);
                var moduleImageUrl = ResolveUrl(string.Concat("~/", module.Name, ".image"));
                var moduleTarget = blank ? "_blank" : string.Empty;
                var itemClass = Module != null && Module.Id == moduleId ? "menu_item_selected" : "menu_item";

                var moduleLink = new Panel { CssClass = itemClass };
                moduleLink.Controls.Add(new Literal {Text = "<table style='width: 100%; font-size: inherit; border-spacing: 0px; padding: 0px;'><tr><td style='width: 20px;'>"});
                moduleLink.Controls.Add(new Image { ImageUrl = moduleImageUrl, ImageAlign = ImageAlign.AbsMiddle });
                moduleLink.Controls.Add(new Literal { Text = "</td><td>" });

                var link = new HyperLink{Text = moduleName,NavigateUrl = modulePath,Target = moduleTarget};
                link.Style.Add(HtmlTextWriterStyle.Color, "inherit");
                link.Style.Add(HtmlTextWriterStyle.TextDecoration, "inherit");
                //link.Style.Add(HtmlTextWriterStyle.MarginLeft, "3px");

                moduleLink.Controls.Add(link);
                moduleLink.Controls.Add(new Literal { Text = "</td></tr></table>" });

                var linkString = string.IsNullOrEmpty(moduleTarget) ? "location.href = '" + modulePath + "'; return false;" 
                    : "window.open('" + modulePath + "', '" + moduleTarget + "'); return false;";

                moduleLink.Attributes.Add("onclick", linkString);

                if (Module != null && module.Id != Module.Id)
                {
                    moduleLink.Attributes.Add("onmouseover", "this.className = 'menu_item_over'");
                    moduleLink.Attributes.Add("onmouseout", "this.className = '" + itemClass + "'");
                }

                if (panelSubGroupContent != null) panelSubGroupContent.Controls.Add(moduleLink);

                lastGroupId = groupId;
                lastSubGroup = subGroupName;

                RegisterModuleForSearch(itemClass, moduleImageUrl, moduleName, modulePath, moduleTarget);
            }
        }
    
        private string GetPath(string root, Module module)
        {
            var fullpath = root;

            if (!ApplicationPath.EndsWith("/")) fullpath = string.Concat(fullpath, "/");
            fullpath = string.Concat(fullpath, module.GroupUrl.Trim('/'), "/");
            fullpath = string.Concat(fullpath, module.Url.Trim('/'));

            return ResolveUrl(fullpath);
        }

        private void RegisterModuleForSearch(string itemClass, string moduleImageUrl, string moduleName, string modulePath, string moduleTarget)
        {
            var linkString = string.IsNullOrEmpty(moduleTarget) 
                ? "location.href = '" + modulePath + "'; return false;"
                : "window.open('" + modulePath + "', '" + moduleTarget + "'); return false;";

            string module = string.Format(@"<div class=""{0}"" title=""{4}"" style=""display: none;"" onclick=""{5}"">

<table style=""width: 100%; font-size: inherit; border-spacing: 0px; padding: 0px;"">
<tr><td style=""width: 20px;"">
<img src=""{1}"" align=""absmiddle"" />
</td><td>
<a href=""{2}"" target=""{3}"" style=""color:inherit; text-decoration:inherit;"">{4}</a>
</td></tr></table>
</div>", 
       itemClass,
       moduleImageUrl,
       modulePath,
       moduleTarget,
       moduleName,
       linkString
       );
            search_results.Controls.Add(new Literal{Text = module});
        }
    }
}