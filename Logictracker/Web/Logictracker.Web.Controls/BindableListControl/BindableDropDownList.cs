#region Usings

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

#endregion

namespace Logictracker.Web.Controls.BindableListControl
{
    public class BindableDropDownList: DropDownList, IBindableListControl
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
            get { return SelectedIndex < 0 ? new int[0] : new []{Convert.ToInt32(SelectedValue)}; }
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
            if(li != null) li.Selected = true;
        }

        #endregion
    }
}
