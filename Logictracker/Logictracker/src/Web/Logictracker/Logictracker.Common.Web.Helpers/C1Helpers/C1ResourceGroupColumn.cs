#region Usings

using System.Web.UI;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;

#endregion

namespace Logictracker.Web.Helpers.C1Helpers
{
    [ToolboxData("<{0}:C1ResourceGroupColumn></{0}:C1ResourceGroupColumn>")]
    public class C1ResourceGroupColumn : C1BoundField
    {
        /// <summary>
        /// The resource name to be displayed as group text.
        /// </summary>
        public string GroupVariableName
        {
            get { return (string)ViewState["GroupVariableName"]; }
            set { ViewState["GroupVariableName"] = value; }
        }

        /// <summary>
        /// The name of the resource set wich contains the resource to be displayed as gruop text.
        /// </summary>
        public string GroupResourceName
        {
            get { return (string)ViewState["GroupResourceName"]; }
            set { ViewState["GroupResourceName"] = value; }
        }

        /// <summary>
        /// The Position where grouping will be displayed.
        /// Default: Header.
        /// </summary>
        public GroupPosition Position
        {

            get { return (GroupPosition)(ViewState["Position"] ?? GroupPosition.Header); }
            set { ViewState["Position"] = value; }
        }

        /// <summary>
        /// The initial outline mode.
        /// Defaul: StartExpanded.
        /// </summary>
        public OutlineMode OutlineMode
        {
            get { return (OutlineMode)(ViewState["OutlineMode"] ?? OutlineMode.StartExpanded); }
            set { ViewState["OutlineMode"] = value; }
        }

        /// <summary>
        /// The GroupSingleRow property.
        /// Default: true.
        /// </summary>
        public bool GroupSingleRow 
        {
            get { return (bool) (ViewState["GroupSingleRow"] ?? true); }
            set { ViewState["GroupSingleRow"] = value; }
        }

        /// <summary>
        /// Changes the group info header text according to the specified resource.
        /// </summary>
        public override GroupInfo GroupInfo
        {
            get
            {
                var group = base.GroupInfo;

                group.Position = Position;

                group.OutlineMode = OutlineMode;

                group.GroupSingleRow = GroupSingleRow;

                group.HeaderText = CultureManager.GetString(GroupResourceName, GroupVariableName);

                return group;
            }
        }

        /// <summary>
        /// Set default values 
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Visible = false;
            GroupInfo.HeaderText = "";
        }

    }
}
