#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Security;
using Logictracker.Web.Controls.DataBinders;

#endregion

namespace Logictracker.Web.Controls.BindableListControl
{
    public class BindableListControlBase
    {
        private IBindableListControl Control { get; set; }
        public bool NeedsBinding { get; set; }
        public IDictionary<BindEntities, IBindableListControl> Parents { get; set; }

        public VsProperty<BindEntities> EntityProperty { get; private set; }
        public VsProperty<bool> AddAllItemProperty { get; private set; }
        public VsProperty<bool> AddNoneItemProperty { get; private set; }
        public VsProperty<string> ParentControlsProperty { get; private set; }
        private VsProperty<IEnumerable<int>> SelectedValuesProperty { get; set; }

        public BindableListControlBase(IBindableListControl control)
        {
            Control = control;
            var ctrl = Control as Control;
            EntityProperty = ctrl.CreateVsProperty("Entity", BindEntities.Empresa);
            AddAllItemProperty = ctrl.CreateVsProperty("AddAllItem", false);
            AddNoneItemProperty = ctrl.CreateVsProperty("AddNoneItem", false);
            ParentControlsProperty = ctrl.CreateVsProperty("ParentControls", string.Empty);
            SelectedValuesProperty = ctrl.CreateVsProperty("SelectedValues", new int[0] as IEnumerable<int>);
        }

        public void OnPagePreLoad()
        {
            NeedsBinding = true;
            Parents = Control.GetParents().ToDictionary(p => p.Entity, p => p);
            foreach (var parent in Parents.Values)
            {
                parent.RegisterChild(Control);
                parent.SelectedIndexChanged += ParentSelectedIndexChanged;
            }
            EncloseInUpdatePanel();
        }

        void ParentSelectedIndexChanged(object sender, EventArgs e)
        {
            NeedsBinding = true;
        }

        public void OnPreRender()
        {
            DataBind();
            SetSelected();
        }

        public void OnSelectedIndexChanged()
        {
            SelectedValuesProperty.Set(Control.Selected);
        }

        public void DataBind()
        {
            foreach (var parent in Parents.Values) parent.DataBind();
            if (NeedsBinding)
            {
                Control.ClearItems();
                if (WebSecurity.Authenticated)
                {
                    var dataBinder = DataBinderFactory.GetDataBinder(EntityProperty.Get());
                    if (AddAllItemProperty.Get()) AddItem(dataBinder.ItemAllName, dataBinder.ItemAllValue);
                    if (AddNoneItemProperty.Get()) AddItem(dataBinder.ItemNoneName, dataBinder.ItemNoneValue);
                    dataBinder.DataBind(Control);
                }
            }
            NeedsBinding = false;
        }

        public void AddItem(string text, int value)
        {
            Control.AddItem(text, value);
        }

        public IBindableListControl GetParent(BindEntities entity)
        {
            return !Parents.ContainsKey(entity) ? null : Parents[entity];
        }

        public IEnumerable<int> GetParentSelected(BindEntities entity)
        {
            var parent = GetParent(entity);
            return parent != null ? parent.Selected : new int[0];
        }

        public void RegisterChild(IBindableListControl control)
        {
            Control.RegisterChild(control);
        }

        public void Select(IEnumerable<int> value)
        {
            SelectedValuesProperty.Set(value);
            if (!NeedsBinding) SetSelected();
        }

        private void SetSelected()
        {
            DataBind();
            Control.ClearSelection();
            var vals = SelectedValuesProperty.Get();
            if (vals.Count() > 0)
            {
                foreach (var val in vals)
                {
                    Control.SelectItem(val);
                }
            }
            OnSelectedIndexChanged();
        }

        protected void EncloseInUpdatePanel()
        {
            if (Parents.Count == 0) return;
            var upd = GetUpdatePanel();
            upd.ContentTemplateContainer.Controls.Add(Control as Control);
        }
        protected UpdatePanel GetUpdatePanel()
        {
            var ctl = Control as Control;
            if (ctl != null && ctl.Parent != null)
            {
                var upd = ctl.Parent.FindControl(string.Format("{0}__updatepanel", ctl.ID)) as UpdatePanel;
                if (upd != null) return upd;

                upd = new UpdatePanel { ID = string.Format("{0}__updatepanel", ctl.ID), UpdateMode = UpdatePanelUpdateMode.Conditional, ChildrenAsTriggers = false };

                foreach (var parent in Parents.Values.Cast<Control>())
                {
                    var trg = new AsyncPostBackTrigger { ControlID = parent.ID, EventName = "SelectedIndexChanged" };
                    upd.Triggers.Add(trg);
                }
                ctl.Parent.Controls.Add(upd);
                return upd;
            }
            return null;
        }

        public void SetOptionGroup(ListItem li, OptionGroupInfo optionGroup)
        {
            if (optionGroup == null) return;

            li.Attributes.Add("OptionGroup", optionGroup.Text);

            const string backgroundRepeat = "background-repeat: no-repeat;padding-left: 20px;";

            var imageUrl = !string.IsNullOrEmpty(optionGroup.ImageUrl) ? string.Format("background-image: url({0}); background-position: left center;", (Control as Control).ResolveUrl(optionGroup.ImageUrl)) : string.Empty;
            var backgroundColor = !string.IsNullOrEmpty(optionGroup.BackgroundColor) ? string.Format("background-color: {0};", optionGroup.BackgroundColor) : string.Empty;
            var color = !string.IsNullOrEmpty(optionGroup.TextColor) ? string.Format("color: {0};", optionGroup.TextColor) : string.Empty;

            var style = string.Concat(backgroundRepeat, imageUrl, backgroundColor, color);

            if (!string.IsNullOrEmpty(color) || !string.IsNullOrEmpty(backgroundColor) || !string.IsNullOrEmpty(imageUrl)) li.Attributes.Add("style", style);
        }
    }
}
