#region Usings

using System;
using System.Collections;
using Quartz.Collection;

#endregion

namespace Quartz.Util
{
    /// <summary>
    /// An implementation of <see cref="IDictionary" /> that wraps another <see cref="IDictionary" />
    /// and flags itself 'dirty' when it is modified, enforces that all keys are
    /// strings. 
    ///  </summary>
    [Serializable]
    public class StringKeyDirtyFlagMap : DirtyFlagMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringKeyDirtyFlagMap"/> class.
        /// </summary>
        public StringKeyDirtyFlagMap()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringKeyDirtyFlagMap"/> class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        public StringKeyDirtyFlagMap(int initialCapacity) : base(initialCapacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringKeyDirtyFlagMap"/> class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <param name="loadFactor">The load factor.</param>
        public StringKeyDirtyFlagMap(int initialCapacity, float loadFactor) : base(initialCapacity, loadFactor)
        {
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetKeys()
        {
            return (string[]) new ArrayList(KeySet()).ToArray(typeof (string));
        }

        /// <summary>
        /// Adds the name-value pairs in the given <see cref="IDictionary" /> to the <see cref="JobDataMap" />.
        /// <p>
        /// All keys must be <see cref="string" />s, and all values must be serializable.
        /// </p>
        /// </summary>
        public override void PutAll(IDictionary map)
        {
            var itr = new HashSet(map.Keys).GetEnumerator();
            while (itr.MoveNext())
            {
                var key = itr.Current;
                var val = map[key];

                Put(key, val);
                // will throw ArgumentException if value not serializable
            }
        }

        /// <summary>
        /// Adds the given <see cref="int" /> value to the <see cref="IJob" />'s
        /// data map.
        /// </summary>
        public virtual void Put(string key, int value)
        {
            base.Put(key, value);
        }

        /// <summary>
        /// Adds the given <see cref="long" /> value to the <see cref="IJob" />'s
        /// data map.
        /// </summary>
        public virtual void Put(string key, long value)
        {
            base.Put(key, value);
        }

        /// <summary>
        /// Adds the given <see cref="float" /> value to the <see cref="IJob" />'s
        /// data map.
        /// </summary>
        public virtual void Put(string key, float value)
        {
            base.Put(key, value);
        }

        /// <summary>
        /// Adds the given <see cref="double" /> value to the <see cref="IJob" />'s
        /// data map.
        /// </summary>
        public virtual void Put(string key, double value)
        {
            base.Put(key, value);
        }

        /// <summary> 
        /// Adds the given <see cref="bool" /> value to the <see cref="IJob" />'s
        /// data map.
        /// </summary>
        public virtual void Put(string key, bool value)
        {
            base.Put(key, value);
        }

        /// <summary>
        /// Adds the given <see cref="char" /> value to the <see cref="IJob" />'s
        /// data map.
        /// </summary>
        public virtual void Put(string key, char value)
        {
            base.Put(key, value);
        }

        /// <summary>
        /// Adds the given <see cref="string" /> value to the <see cref="IJob" />'s
        /// data map.
        /// </summary>
        public virtual void Put(string key, string value)
        {
            base.Put(key, value);
        }

        /// <summary>
        /// Adds the given serializable object value to the <see cref="JobDataMap" />.
        /// </summary>
        public override object Put(object key, object value)
        {
            if (!(key is string))
            {
                throw new ArgumentException("Keys in map must be Strings.");
            }
            return base.Put(key, value);
        }

        /// <summary> 
        /// Retrieve the identified <see cref="int" /> value from the <see cref="JobDataMap" />.
        /// </summary>
        public virtual int GetInt(string key)
        {
            var obj = this[key];

            try
            {
                return (int) obj;
            }
            catch (Exception)
            {
                throw new InvalidCastException("Identified object is not an Integer.");
            }
        }

        /// <summary>
        /// Retrieve the identified <see cref="long" /> value from the <see cref="JobDataMap" />.
        /// </summary>
        public virtual long GetLong(string key)
        {
            var obj = this[key];

            try
            {
                return (long) obj;
            }
            catch (Exception)
            {
                throw new InvalidCastException("Identified object is not a Long.");
            }
        }

        /// <summary>
        /// Retrieve the identified <see cref="float" /> value from the <see cref="JobDataMap" />.
        /// </summary>
        public virtual float GetFloat(string key)
        {
            var obj = this[key];

            try
            {
                return (float) obj;
            }
            catch (Exception)
            {
                throw new InvalidCastException("Identified object is not a Float.");
            }
        }

        /// <summary>
        /// Retrieve the identified <see cref="double" /> value from the <see cref="JobDataMap" />.
        /// </summary>
        public virtual double GetDouble(string key)
        {
            var obj = this[key];

            try
            {
                return ((double) obj);
            }
            catch (Exception)
            {
                throw new InvalidCastException("Identified object is not a Double.");
            }
        }

        /// <summary> 
        /// Retrieve the identified <see cref="bool" /> value from the <see cref="JobDataMap" />.
        /// </summary>
        public virtual bool GetBoolean(string key)
        {
            var obj = this[key];

            try
            {
                return ((bool) obj);
            }
            catch (Exception)
            {
                throw new InvalidCastException("Identified object is not a Boolean.");
            }
        }

        /// <summary>
        /// Retrieve the identified <see cref="char" /> value from the <see cref="JobDataMap" />. 
        /// </summary>
        public virtual char GetChar(string key)
        {
            var obj = this[key];

            try
            {
                return ((char) obj);
            }
            catch (Exception)
            {
                throw new InvalidCastException("Identified object is not a Character.");
            }
        }

        /// <summary>
        /// Retrieve the identified <see cref="string" /> value from the <see cref="JobDataMap" />.
        /// </summary>
        public virtual string GetString(string key)
        {
            var obj = this[key];

            try
            {
                return (string) obj;
            }
            catch (Exception)
            {
                throw new InvalidCastException("Identified object is not a String.");
            }
        }
    }
}