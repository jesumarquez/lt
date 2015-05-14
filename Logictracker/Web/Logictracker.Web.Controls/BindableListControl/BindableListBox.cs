#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

#endregion

namespace Logictracker.Web.Controls.BindableListControl
{
    public class BindableListBox : ListBox, IBindableListControl
    {
        private BindableListControlBase _base;
        protected BindableListControlBase Base
        {
            get { return _base ?? (_base = new BindableListControlBase(this)); }
        }

        #region Event Handlers
        protected override void OnPagePreLoad(object sender, EventArgs e)
        {
            Base.OnPagePreLoad();
            base.OnPagePreLoad(sender, e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            Base.OnPreRender();
            base.OnPreRender(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            Base.OnSelectedIndexChanged();
            base.OnSelectedIndexChanged(e);
        }
        #endregion

        #region Implementation of IBindableListControl

        public BindEntities Entity
        {
            get { return Base.EntityProperty.Get(); }
            set { Base.EntityProperty.Set(value); }
        }

        public bool AddAllItem
        {
            get { return Base.AddAllItemProperty.Get(); }
            set { Base.AddAllItemProperty.Set(value); }
        }

        public bool AddNoneItem
        {
            get { return Base.AddNoneItemProperty.Get(); }
            set { Base.AddNoneItemProperty.Set(value); }
        }

        public string ParentControls
        {
            get { return Base.ParentControlsProperty.Get(); }
            set { Base.ParentControlsProperty.Set(value); }
        }

        public IEnumerable<int> Selected
        {
            get { return Items.Cast<ListItem>().Where(i => i.Selected).Select(i => Convert.ToInt32(i.Value)); }
            set { Base.Select(value); }
        }

        public void ClearItems()
        {
            Items.Clear();
        }

        public void AddItem(string text, int value)
        {
            AddItem(text, value, null);
        }

        public void AddItem(string text, int value, OptionGroupInfo optionGroup)
        {
            var li = new ListItem(text, value.ToString());

            Base.SetOptionGroup(li, optionGroup);

            Items.Add(li);
        }

        public IBindableListControl GetParent(BindEntities entity)
        {
            return Base.GetParent(entity);
        }

        public IEnumerable<int> GetParentSelected(BindEntities entity)
        {
            return Base.GetParentSelected(entity);
        }

        public void RegisterChild(IBindableListControl control)
        {
            Base.RegisterChild(control);
        }

        public void SelectItem(int value)
        {
            var li = Items.FindByValue(value.ToString());
            if (li != null) li.Selected = true;
        }

        #endregion

        #region Option Group
        public const string OptionGroupTag = "optgroup";
        private const string OptionTag = "option";
        protected void RenderListItem(ListItem item, HtmlTextWriter writer)
        {
            if (item.Selected) writer.AddAttribute("selected", "selected");
            if (!string.IsNullOrEmpty(item.Value)) writer.AddAttribute("value", item.Value);
            foreach (var key in item.Attributes.Keys.Cast<string>().Where(key => key != OptionGroupTag))
            {
            	writer.AddAttribute(key, item.Attributes[key]);
            }
            writer.RenderBeginTag(OptionTag);
            writer.Write(item.Text);
            writer.RenderEndTag();
        }
        protected void RenderOptionGroupBeginTag(string optionGroupName, HtmlTextWriter writer)
        {
            writer.AddAttribute("label", optionGroupName);
            writer.RenderBeginTag(OptionGroupTag);
        }
        protected void RenderOptionGroupEndTag(HtmlTextWriter writer)
        {
            writer.RenderEndTag();
        }
        protected override void RenderContents(HtmlTextWriter writer)
        {
            var renderedOptionGroups = new List<String>();

            foreach (ListItem item in Items)
            {
                Page.ClientScript.RegisterForEventValidation(UniqueID, item.Value);

                if (item.Attributes[OptionGroupTag] == null) RenderListItem(item, writer);
                else
                {
                    var currentOptionGroup = item.Attributes[OptionGroupTag];

                    if (renderedOptionGroups.Contains(currentOptionGroup)) RenderListItem(item, writer);
                    else
                    {
                        if (renderedOptionGroups.Count > 0) RenderOptionGroupEndTag(writer);

                        RenderOptionGroupBeginTag(currentOptionGroup, writer);

                        renderedOptionGroups.Add(currentOptionGroup);

                        RenderListItem(item, writer);
                    }
                }
            }


            if (renderedOptionGroups.Count > 0) RenderOptionGroupEndTag(writer);
        }

        protected override object SaveViewState()
        {
            var state = new object[Items.Count + 1];
            var baseState = base.SaveViewState();
            state[0] = baseState;
            var itemHasAttributes = false;

            for (var i = 0; i < Items.Count; i++)
            {
            	if (Items[i].Attributes.Count <= 0) continue;
            	itemHasAttributes = true;
            	var attributes = new object[Items[i].Attributes.Count * 2];
            	var k = 0;

            	foreach (string key in Items[i].Attributes.Keys)
            	{
            		attributes[k] = key;
            		k++;
            		attributes[k] = Items[i].Attributes[key];
            		k++;
            	}
            	state[i + 1] = attributes;
            }

            return itemHasAttributes ? state : baseState;
        }

        protected override void LoadViewState(object savedState)
        {
            if (savedState == null)
                return;

            if (savedState.GetType().GetElementType() != null &&
                (savedState.GetType().GetElementType().Equals(typeof(object))))
            {
                var state = (object[])savedState;
                base.LoadViewState(state[0]);

                for (var i = 1; i < state.Length; i++)
                {
                	if (state[i] == null) continue;
                	var attributes = (object[])state[i];
                	for (var k = 0; k < attributes.Length; k += 2)
                	{
                		Items[i - 1].Attributes.Add
                			(attributes[k].ToString(), attributes[k + 1].ToString());
                	}
                }
            }
            else
            {
                base.LoadViewState(savedState);
            }
        }
        #endregion
    }
}
