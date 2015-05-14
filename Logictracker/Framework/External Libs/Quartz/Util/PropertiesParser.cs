#region Usings

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using Quartz.Collection;

#endregion

namespace Quartz.Util
{
	/// <summary>
	/// This is an utility class used to parse the properties.
	/// </summary>
	/// <author> James House</author>
	public class PropertiesParser
	{
        internal NameValueCollection props;

        /// <summary>
        /// Gets the underlying properties.
        /// </summary>
        /// <value>The underlying properties.</value>
		public virtual NameValueCollection UnderlyingProperties
		{
			get { return props; }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesParser"/> class.
        /// </summary>
        /// <param name="props">The props.</param>
		public PropertiesParser(NameValueCollection props)
		{
			this.props = props;
		}

        /// <summary>
        /// Gets the string property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual string GetStringProperty(string name)
		{
			var val = props.Get(name);
			if (val == null)
			{
				return null;
			}
			return val.Trim();
		}

        /// <summary>
        /// Gets the string property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public virtual string GetStringProperty(string name, string defaultValue)
		{
			var val = props[name] == null ? defaultValue : props[name];
			if (val == null)
			{
				return defaultValue;
			}
			val = val.Trim();
			if (val.Length == 0)
			{
				return defaultValue;
			}
			return val;
		}

        /// <summary>
        /// Gets the string array property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual string[] GetStringArrayProperty(string name)
		{
			return GetStringArrayProperty(name, null);
		}

        /// <summary>
        /// Gets the string array property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public virtual string[] GetStringArrayProperty(string name, string[] defaultValue)
		{

            var vals = GetStringProperty(name);
            if (vals == null)
            {
                return defaultValue;
            }

            var items = vals.Split(',');
            var strs = new ArrayList();
            try
            {
                foreach (var s in items)
                {
                    strs.Add(s.Trim());
                }

                return (string[]) strs.ToArray(typeof(string));
            }
            catch (Exception)
            {
                return defaultValue;
            }
		}

        /// <summary>
        /// Gets the boolean property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual bool GetBooleanProperty(string name)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
				return false;
			}

			return val.ToUpper(CultureInfo.InvariantCulture).Equals("TRUE");
		}

        /// <summary>
        /// Gets the boolean property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">if set to <c>true</c> [defaultValue].</param>
        /// <returns></returns>
		public virtual bool GetBooleanProperty(string name, bool defaultValue)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
				return defaultValue;
			}

            return val.ToUpper(CultureInfo.InvariantCulture).Equals("TRUE");
		}

        /// <summary>
        /// Gets the byte property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual sbyte GetByteProperty(string name)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
				throw new FormatException(" null string");
			}

			try
			{
                return SByte.Parse(val, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", val));
			}
		}

        /// <summary>
        /// Gets the byte property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public virtual sbyte GetByteProperty(string name, sbyte defaultValue)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
			    return defaultValue;
			}

			try
			{
                return SByte.Parse(val, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", val));
			}
		}

        /// <summary>
        /// Gets the char property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual char GetCharProperty(string name)
		{
			var param = GetStringProperty(name);
			if (param == null)
			{
				return '\x0000';
			}

			if (param.Length == 0)
			{
				return '\x0000';
			}

			return param[0];
		}

        /// <summary>
        /// Gets the char property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public virtual char GetCharProperty(string name, char defaultValue)
		{
			var param = GetStringProperty(name);
			if (param == null)
			{
				return defaultValue;
			}

			if (param.Length == 0)
			{
				return defaultValue;
			}

			return param[0];
		}

        /// <summary>
        /// Gets the double property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual double GetDoubleProperty(string name)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
				throw new FormatException(" null string");
			}

			try
			{
                return Double.Parse(val, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", val));
			}
		}

        /// <summary>
        /// Gets the double property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public virtual double GetDoubleProperty(string name, double defaultValue)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
				return defaultValue;
			}

			try
			{
                return Double.Parse(val, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", val));
			}
		}

        /// <summary>
        /// Gets the float property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual float GetFloatProperty(string name)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
				throw new FormatException(" null string");
			}

			try
			{
                return Single.Parse(val, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", val));
			}
		}

        /// <summary>
        /// Gets the float property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public virtual float GetFloatProperty(string name, float defaultValue)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
				return defaultValue;
			}

			try
			{
                return Single.Parse(val, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", val));
			}
		}

        /// <summary>
        /// Gets the int property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual int GetIntProperty(string name)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
				throw new FormatException(" null string");
			}

			try
			{
                return Int32.Parse(val, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", val));
			}
		}

        /// <summary>
        /// Gets the int property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public virtual int GetIntProperty(string name, int defaultValue)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
				return defaultValue;
			}

			try
			{
                return Int32.Parse(val, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", val));
			}
		}

        /// <summary>
        /// Gets the int array property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual int[] GetIntArrayProperty(string name)
		{
			return GetIntArrayProperty(name, null);
		}

        /// <summary>
        /// Gets the int array property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public virtual int[] GetIntArrayProperty(string name, int[] defaultValue)
		{
			var vals = GetStringProperty(name);
			if (vals == null)
			{
				return defaultValue;
			}

			if (!vals.Trim().Equals(""))
			{
				var stok = vals.Split(',');
				var ints = ArrayList.Synchronized(new ArrayList(10));
				try
				{
					foreach (var s in stok)
					{
						try
						{
                            ints.Add(Int32.Parse(s, CultureInfo.InvariantCulture));
						}
						catch (FormatException)
						{
							throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", vals));
						}
					}
					var outInts = new int[ints.Count];
					for (var i = 0; i < ints.Count; i++)
					{
						outInts[i] = ((Int32) ints[i]);
					}
					return outInts;
				}
				catch (Exception)
				{
					return defaultValue;
				}
			}

			return defaultValue;
		}

        /// <summary>
        /// Gets the long property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual long GetLongProperty(string name)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
				throw new FormatException(" null string");
			}

			try
			{
                return Int64.Parse(val, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", val));
			}
		}

        /// <summary>
        /// Gets the long property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="def">The def.</param>
        /// <returns></returns>
		public virtual long GetLongProperty(string name, long def)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
				return def;
			}

			try
			{
                return Int64.Parse(val, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", val));
			}
		}

        /// <summary>
        /// Gets the TimeSpan property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="def">The def.</param>
        /// <returns></returns>
        public virtual TimeSpan GetTimeSpanProperty(string name, TimeSpan def)
        {
            var val = GetStringProperty(name);
            if (val == null)
            {
                return def;
            }

            try
            {
                return TimeSpan.FromMilliseconds(Int64.Parse(val, CultureInfo.InvariantCulture));
            }
            catch (FormatException)
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", val));
            }
        }

        /// <summary>
        /// Gets the short property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual short GetShortProperty(string name)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
				throw new FormatException(" null string");
			}

			try
			{
                return Int16.Parse(val, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", val));
			}
		}

        /// <summary>
        /// Gets the short property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public virtual short GetShortProperty(string name, short defaultValue)
		{
			var val = GetStringProperty(name);
			if (val == null)
			{
				return defaultValue;
			}

			try
			{
                return Int16.Parse(val, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, " '{0}'", val));
			}
		}

        /// <summary>
        /// Gets the property groups.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
		public virtual string[] GetPropertyGroups(string prefix)
		{
            var groups = new HashSet(10);

			if (!prefix.EndsWith("."))
			{
				prefix += ".";
			}

			foreach (string key in props.Keys)
			{
				if (key.StartsWith(prefix))
				{
					var groupName = key.Substring(prefix.Length, (key.IndexOf('.', prefix.Length)) - (prefix.Length));
					groups.Add(groupName);
				}
			}

            return (string[]) groups.ToArray(typeof (string));
		}

        /// <summary>
        /// Gets the property group.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
		public virtual NameValueCollection GetPropertyGroup(string prefix)
		{
			return GetPropertyGroup(prefix, false);
		}

        /// <summary>
        /// Gets the property group.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="stripPrefix">if set to <c>true</c> [strip prefix].</param>
        /// <returns></returns>
		public virtual NameValueCollection GetPropertyGroup(string prefix, bool stripPrefix)
		{
			return GetPropertyGroup(prefix, stripPrefix, null);
		}


        /// <summary>
        /// Get all properties that start with the given prefix.  
        /// </summary>
        /// <param name="prefix">The prefix for which to search.  If it does not end in a "." then one will be added to it for search purposes.</param>
        /// <param name="stripPrefix">Whether to strip off the given <paramref name="prefix" /> in the result's keys.</param>
        /// <param name="excludedPrefixes">Optional array of fully qualified prefixes to exclude.  For example if <see paramfref="prefix" /> is "a.b.c", then <see paramref="excludedPrefixes" /> might be "a.b.c.ignore".</param>
        /// <returns>Group of <see cref="NameValueCollection" /> that start with the given prefix, optionally have that prefix removed, and do not include properties that start with one of the given excluded prefixes.</returns>
        public virtual NameValueCollection GetPropertyGroup(string prefix, bool stripPrefix, string[] excludedPrefixes)
        {
            var group = new NameValueCollection();

            if (!prefix.EndsWith("."))
            {
                prefix += ".";
            }

            foreach (string key in props.Keys)
            {
                if (key.StartsWith(prefix))
                {
                    var exclude = false;
                    if (excludedPrefixes != null)
                    {
                        for (var i = 0; (i < excludedPrefixes.Length) && (exclude == false); i++)
                        {
                            exclude = key.StartsWith(excludedPrefixes[i]);
                        }
                    }

                    if (exclude == false)
                    {
                        var value = GetStringProperty(key, "");

                        if (stripPrefix)
                        {
                            group[key.Substring(prefix.Length)] = value;
                        }
                        else
                        {
                            group[key] = value;
                        }
                    }
                }
            }

            return group;
        }


         /// <summary>
        /// Reads the properties from assembly (embedded resource).
        /// </summary>
        /// <param name="resourceName">The file name to read resources from.</param>
        /// <returns></returns>
        public static PropertiesParser ReadFromEmbeddedAssemblyResource(string resourceName)
        {
            return ReadFromStream(typeof(IScheduler).Assembly.GetManifestResourceStream(resourceName));
        }

        /// <summary>
        /// Reads the properties from file system.
        /// </summary>
        /// <param name="fileName">The file name to read resources from.</param>
        /// <returns></returns>
        public static PropertiesParser ReadFromFileResource(string fileName)
        {
            return ReadFromStream(File.OpenRead(fileName));
        }

	    private static PropertiesParser ReadFromStream(Stream stream)
	    {
	        var props = new NameValueCollection();
	        using (var sr = new StreamReader(stream))
	        {
	            string line;
	            while ((line = sr.ReadLine()) != null)
	            {
	                line = line.TrimStart();

	                if (line.StartsWith("#"))
	                {
	                    // comment line 
	                    continue;
	                }
	                if (line.StartsWith("!END"))
	                {
	                    // special end condition
	                    break;
	                }
	                var lineItems = line.Split(new[] { '=' }, 2);
	                if (lineItems.Length == 2)
	                {
	                    props[lineItems[0].Trim()] = lineItems[1].Trim();
	                }
	            }
	        }
	        return new PropertiesParser(props);
	    }
	}
}
