using System;
using System.Configuration;
namespace Geocoder.Data.SessionManagement
{
	public class OpenSessionInViewSection : ConfigurationSection
	{
		[ConfigurationCollection(typeof(SessionFactoriesCollection), AddItemName = "sessionFactory", ClearItemsName = "clearFactories"), ConfigurationProperty("sessionFactories", IsDefaultCollection = false)]
		public SessionFactoriesCollection SessionFactories
		{
			get
			{
				return (SessionFactoriesCollection)base["sessionFactories"];
			}
		}
	}
}
