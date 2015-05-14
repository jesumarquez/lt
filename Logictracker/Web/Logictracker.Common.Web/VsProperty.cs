#region Usings

using System;
using System.Reflection;
using System.Web.UI;

#endregion

namespace Logictracker.Web
{
    public class FullVsProperty<T> : VsProperty<T>
    {
        public FullVsProperty(Control parent, string name)
            : base(parent, name, default(T))
        {
            SetFromSession(parent);
        }
        public FullVsProperty(Control parent, string name, T defaultValue)
            : base(parent, name, defaultValue)
        {
            SetFromSession(parent);
        }
        private void SetFromSession(Control parent)
        {
            if (ViewState[Name] != null) return;
            if(parent.Page.Request.QueryString[Name] != null)
            {
                ViewState[Name] = Convert.ChangeType(parent.Page.Request.QueryString[Name], typeof (T));
            }
            else
            {
                ViewState[Name] = parent.Page.Session[Name];    
            }
            parent.Page.Session[Name] = null;
        }
    }

    public class SessionVsProperty<T> : VsProperty<T>
    {
        public SessionVsProperty(Control parent, string name)
            : base(parent, name,default(T))
        {
            SetFromSession(parent);
        }
        public SessionVsProperty(Control parent, string name, T defaultValue)
            : base(parent, name, defaultValue)
        {
            SetFromSession(parent);
        }
        private void SetFromSession(Control parent)
        {
            if (ViewState[Name] != null) return;
            ViewState[Name] = parent.Page.Session[Name];
            parent.Page.Session[Name] = null;
        }
    }

    public class VsProperty<T>
    {
        protected Control Parent { get; set; }
        protected string Name { get; set; }
        protected T DefaultValue { get; set; }

        public VsProperty(Control parent, string name)
            : this(parent, name,default(T))
        {
            
        }
        public VsProperty(Control parent, string name, T defaultValue)
        {
            Parent = parent;
            Name = name;
            DefaultValue = defaultValue;
        }

        public void Set(T value)
        {
            ViewState[Name] = value;
        }

        public T Get(T defaultValue)
        {
            return ViewState[Name] != null ? (T)ViewState[Name] : defaultValue;
        }
        public T Get()
        {
            return Get(DefaultValue);
        }

        protected StateBag ViewState
        {
            get
            {
                return Parent.GetType().GetProperty("ViewState", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Parent, null) as StateBag;
            }
        }
    }
}
