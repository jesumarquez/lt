using System;
using System.Configuration;
namespace Geocoder.Data.SessionManagement
{
	public class SessionFactoryElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true, IsKey = true, DefaultValue = "Not Supplied")]
		public string Name
		{
			get
			{
				return (string)base["name"];
			}
			set
			{
				base["name"] = value;
			}
		}
		[ConfigurationProperty("factoryConfigPath", IsRequired = true, DefaultValue = "Not Supplied")]
		public string FactoryConfigPath
		{
			get
			{
				return (string)base["factoryConfigPath"];
			}
			set
			{
				base["factoryConfigPath"] = value;
			}
		}
		[ConfigurationProperty("isTransactional", IsRequired = false, DefaultValue = false)]
		public bool IsTransactional
		{
			get
			{
				return (bool)base["isTransactional"];
			}
			set
			{
				base["isTransactional"] = value;
			}
		}
		public SessionFactoryElement()
		{
		}
		public SessionFactoryElement(string name, string configPath)
		{
			this.Name = name;
			this.FactoryConfigPath = configPath;
		}
	}
}
