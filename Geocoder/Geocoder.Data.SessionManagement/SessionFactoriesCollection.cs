using System;
using System.Configuration;
namespace Geocoder.Data.SessionManagement
{
	[ConfigurationCollection(typeof(SessionFactoryElement))]
	public sealed class SessionFactoriesCollection : ConfigurationElementCollection
	{
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.AddRemoveClearMap;
			}
		}
		public SessionFactoryElement this[int index]
		{
			get
			{
				return (SessionFactoryElement)base.BaseGet(index);
			}
			set
			{
				if (base.BaseGet(index) != null)
				{
					base.BaseRemoveAt(index);
				}
				this.BaseAdd(index, value);
			}
		}
		public new SessionFactoryElement this[string name]
		{
			get
			{
				return (SessionFactoryElement)base.BaseGet(name);
			}
		}
		public SessionFactoriesCollection()
		{
			SessionFactoryElement sessionFactory = (SessionFactoryElement)this.CreateNewElement();
			this.Add(sessionFactory);
		}
		protected override ConfigurationElement CreateNewElement()
		{
			return new SessionFactoryElement();
		}
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SessionFactoryElement)element).Name;
		}
		public int IndexOf(SessionFactoryElement sessionFactory)
		{
			return base.BaseIndexOf(sessionFactory);
		}
		public void Add(SessionFactoryElement sessionFactory)
		{
			this.BaseAdd(sessionFactory);
		}
		protected override void BaseAdd(ConfigurationElement element)
		{
			base.BaseAdd(element, false);
		}
		public void Remove(SessionFactoryElement sessionFactory)
		{
			if (base.BaseIndexOf(sessionFactory) >= 0)
			{
				base.BaseRemove(sessionFactory.Name);
			}
		}
		public void RemoveAt(int index)
		{
			base.BaseRemoveAt(index);
		}
		public void Remove(string name)
		{
			base.BaseRemove(name);
		}
		public void Clear()
		{
			base.BaseClear();
		}
	}
}
