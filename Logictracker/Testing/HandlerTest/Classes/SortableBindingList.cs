using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace HandlerTest.Classes
{
    public class SortableBindingList<t> : BindingList<t>
    {
        private bool m_Sorted = false;
        private ListSortDirection m_SortDirection = ListSortDirection.Ascending;
        private PropertyDescriptor m_SortProperty = null;

        protected override bool SupportsSortingCore
        {
            get
            {
                return true;
            }
        }

        protected override bool IsSortedCore
        {
            get
            {
                return m_Sorted;
            }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                return m_SortDirection;
            }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get
            {
                return m_SortProperty;
            }
        }

        public SortableBindingList(IEnumerable<t> items)
        {
            foreach(var i in items) Items.Add(i);
        }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            m_SortDirection = direction;
            m_SortProperty = prop;
            var listRef = this.Items as List<t>;
            if (listRef == null)
                return;
            var comparer = new SortComparer<t>(prop, direction);

            listRef.Sort(comparer);

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
    }
    public class SortComparer<t>:IComparer<t>
    {
        public PropertyDescriptor Property {get; protected set;}
        public ListSortDirection Direction { get; protected set; }
        public SortComparer(PropertyDescriptor prop, ListSortDirection direction)
        {
            Property = prop;
            Direction = direction;
        }

        #region IComparer Members

        public int Compare(t x, t y)
        {
            var vx = Property.GetValue(x) as IComparable;
            var vy = Property.GetValue(y) as IComparable;
            if (vx == null)
            {
                if (vy == null) return 0;
                else return Direction == ListSortDirection.Ascending ? -1 : 1; 
            }
            else if (vy == null) return Direction == ListSortDirection.Ascending ? 1 : -1;
            var c = vx.CompareTo(vy);
            return Direction == ListSortDirection.Ascending ? c : -c; 
        }

        #endregion
    }
}
