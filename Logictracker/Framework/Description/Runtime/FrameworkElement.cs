#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Metadata;
using Logictracker.Model.IAgent;
using Logictracker.Utils;

#endregion

namespace Logictracker.Description.Runtime
{
    public abstract class FrameworkElement
    {
        #region Smart Properties Managment

	    private void RegisterProperty(ElementAttributeMetadata metadata, String value)
        {
            var property = metadata.XName;
            if (_smartProperties.ContainsKey(property)) throw new ElementException(ElementErrorCodes.PropertyAlreadyRegistered, property);
            
            var propertyInfo = RuntimeType.GetProperty(metadata.MemberInfo.Name);
            
            if (value.StartsWith("{") && value.EndsWith("}"))
            {
                var cmd = value.Trim("{} ".ToCharArray());
                var pair = cmd.Split(" ".ToCharArray(), 2);
                var keyword = pair[0];
                var instruccion = pair.GetLength(0)>1 ? pair[1] : String.Empty;
                
                var extension = MetadataDirectory.GetCustomMarkupExtensions(keyword);
                if (extension == null) throw new ElementException(ElementErrorCodes.CustomMarkupExtensionNotFound, keyword);

                var @object = extension.ProvideValue(this, metadata, keyword, instruccion);
                _smartProperties.Add(property, new SmartProperty(@object));
                return;
            }

            _smartProperties.Add(property, new SmartProperty(Cast(value, propertyInfo.PropertyType)));
        }

	    private void RegisterProperty(String property, Object value)
        {
            if (_smartProperties.ContainsKey(property)) throw new ElementException(ElementErrorCodes.PropertyAlreadyRegistered, property);

            _smartProperties.Add(property, new SmartProperty(value));
        }

        internal SmartProperty GetProperty(String property)
        {
            try
            {
                return _smartProperties[property];
            }
            catch (Exception e)
            {
                throw new ElementException(ElementErrorCodes.PropertyNotRegistered, property, e);
            }
        }

		protected void SetValue(String property, Object value)
        {
            try
            {
                _smartProperties[property].SetValue(value);
            }
            catch (Exception e)
            {
                throw new ElementException(ElementErrorCodes.PropertyNotRegistered, property, e);
            }
        }

		protected Object GetValue(String property)
        {
            try
            {
                return _smartProperties[property].GetValue();
            }
            catch (Exception e)
            {
                throw new ElementException(ElementErrorCodes.PropertyNotRegistered, property, e);
            }
        }

		private readonly Dictionary<String, SmartProperty> _smartProperties = new Dictionary<String, SmartProperty>();

        #endregion

        #region OnPropertyChange

        public delegate void OnPropertyChangeHandler(string propertyName);

        #endregion

		#region Public Members

		public static FrameworkElement Factory(XElement xElement, FrameworkApplication application, FrameworkElement parent)
		{
			var metadata = MetadataDirectory.GetMetadata(xElement);
			var constructor = MetadataDirectory.GetRuntimeType(xElement).GetConstructor(Type.EmptyTypes);

			var item = constructor.Invoke(null) as FrameworkElement;
			if (item == null)
				throw new ElementException(ElementErrorCodes.UnableToCreateFrameworkElement, MetadataDirectory.GetRuntimeType(xElement).Name);

			item._metadata = metadata;
			item.Application = application;
			item.RegisterProperty(xElement.Name.LocalName, item);
			item.Parent = parent;

			var staticKey = xElement.Attribute(MetadataDirectory.StaticsNamespace + "Key");
			if (staticKey != null)
			{
				item.StaticKey = staticKey.Value;
			}
			else
			{
				var att = String.Format("{0}_{1:D4}_{2:D3}", xElement.Name.LocalName, xElement.GetHashCode(), RandomUtils.RandomNumber(1, 999));
				xElement.SetAttributeValue(MetadataDirectory.StaticsNamespace + "Key", att);
				item.StaticKey = att;
			}
			application.RegisterFrameworkElement(item);

			item.GenericType = MetadataDirectory.GetGenericType(xElement);
			item._xElement = xElement;
			item._xElement.AddAnnotation(new AnnotationWrapper { Element = item });

			// Creo los hijos
			if (item._metadata.IsContainer)
			{
				foreach (var xChild in xElement.Elements())
				{
					item._items.Add(Factory(xChild, item.Application, item));
				}
			}
			//foreach (var se in extensions) se.AfterCreate(item);
			return item;
		}

		public void ResolveDependencies()
		{
			// Atributos: cargo los valores y si procede los valores por defecto
			foreach (var attribute in _metadata.Attributes())
			{
				var xAttribute = _xElement.Attribute(attribute.XName);
				if ((xAttribute == null) && attribute.IsSmartProperty && (Parent != null))// && attribute.IsRequired)
				{
					var type = RuntimeType.GetProperty(attribute.MemberInfo.Name).PropertyType;
					IEnumerable<FrameworkElement> elems;
					var act = Parent;
					do
					{
						elems = act.Descendants().Where(type.IsInstanceOfType);
						act = act.Parent;
					} while ((!elems.Any()) && (act != null));

					if (elems.Count() == 1)
					{
						RegisterProperty(attribute, String.Format("{{StaticResource {0}}}", elems.Single()._xElement.Attribute(MetadataDirectory.StaticsNamespace + "Key").Value));
						continue;
					}
				}

				if (xAttribute == null)
				{
					if (attribute.IsRequired)
					{
						throw new ElementException(ElementErrorCodes.RequiredAttributeNotFound, String.Format(@"<{0} {1}=""?""", _xElement.Name.LocalName, attribute.XName));
					}
					if (attribute.IsSmartProperty)
						RegisterProperty(attribute, "{Default}");
					else
						SetValue(attribute, attribute.DefaultValue);
					continue;
				}
				if (attribute.IsSmartProperty)
					RegisterProperty(attribute, xAttribute.Value);
				else
					SetValue(attribute, xAttribute.Value);
			}
		}

		public virtual bool LoadResources()
		{
			return true;
		}

		public void SafeLoadResources()
		{
			try
			{
				if (!LoadResources())
				{
					STrace.Error(typeof(FrameworkElement).FullName, String.Format("Error on LoadResources() on XmlElement: <{0} x:Key={1} />", GetName(), StaticKey ?? "null"));
				}
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(FrameworkElement).FullName, e, String.Format("Exception on LoadResources() on XmlElement: <{0} x:Key={1} />", GetName(), StaticKey ?? "null"));
			}
		}

	    protected virtual bool UnloadResources()
		{
			return true;
		}

		public void SafeUnloadResources()
		{
			try
			{
				if (!UnloadResources())
				{
					STrace.Error(GetType().FullName, String.Format("Error on UnloadResources() on XmlElement: <{0} x:Key={1} />", GetName(), StaticKey ?? "null"));
				}
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e, String.Format("Exception on UnloadResources() on XmlElement: <{0} x:Key={1} />", GetName(), StaticKey ?? "null"));
			}
		}

		public FrameworkApplication Application { get; private set; }

		public IEnumerable<FrameworkElement> Descendants()
		{
			return _xElement
				.Descendants()
				.Select(elem => elem.Annotation<AnnotationWrapper>())
				.Where(elem => elem != null)
				.Select(elem => elem.Element);
		}

		protected IEnumerable<FrameworkElement> Elements() { return _items.AsEnumerable(); }

        /// <summary>
        /// Idetificacion univoca del objeto, para asignaciones estaticas.
        /// En el xml se declara x:Key="identificador"
        /// </summary>
        public string StaticKey { get; private set; }

        /// <summary>
        /// Un FrameworkElement puede implementarse como tipo generico, enlazando
        /// el tipo concreto en Runtime.  En este caso, se utiliza el atributo
        /// GenericType y 
        /// </summary>
        public Type GenericType { get; set; }

		public String GetName() { return _metadata.XName; }
		public String GetStaticKey() { return StaticKey; }

		public void StartServiceIfIService()
		{
			var rs = this as IService;
			if (rs != null) rs.SafeServiceStart();
		}

		public void StopServiceIfIService()
		{
			var rs = this as IService;
			if (rs != null) rs.SafeServiceStop();
		}

		#endregion

		#region Private Members

	    private FrameworkElement Parent { get; set; }
		private XElement _xElement;
		private readonly List<FrameworkElement> _items = new List<FrameworkElement>();
		private ElementMetadata _metadata;

		private class AnnotationWrapper
		{
			public FrameworkElement Element;
		}

		private static Object Cast(Object value, Type targetType)
		{
			if (!targetType.IsEnum) return Convert.ChangeType(value, targetType);

			var s = value as String;
			return String.IsNullOrEmpty(s) ? 0 : Enum.Parse(targetType, s);
		}

		private void SetValue(ElementAttributeMetadata attribute, Object value)
		{
			if (attribute.IsReadOnly && !Application.IsResolvingDependencies)
				throw new ElementException(ElementErrorCodes.InvalidOperation, "SetValue (" + attribute.ToString(true) + ") no puede llamarse, esta definido IsReadOnly=true.");

			switch (attribute.MemberInfo.MemberType)
			{
				case MemberTypes.Field:
					var field = RuntimeType.GetField(attribute.MemberInfo.Name);
					field.SetValue(this, Cast(value, field.FieldType));
					break;
				case MemberTypes.Property:
					var property = RuntimeType.GetProperty(attribute.MemberInfo.Name);
					if (property.GetSetMethod(true) != null)
						property.SetValue(this, Cast(value, property.PropertyType), null);
					break;
				default:
					throw new ElementException(ElementErrorCodes.InvalidOperation, "SetValue (" + attribute.ToString(true) + ") solo puede llamarse sobre miembros del tipo Field o Property.");
			}
		}

		/// <summary>
		/// Obtiene el tipo en tiempo de ejecucion de una instancia de FrameworkElement.
		/// En los tipos no genericos obtiene el mismo valor que obtiene GetType().
		/// En los tipos genericos obtiene el tipo construido en tiempo de ejecucion.
		/// </summary>
		private Type RuntimeType
		{
			get
			{
				return _runtimeTypeCache ?? (_runtimeTypeCache = (GenericType != null) && GetType().ContainsGenericParameters
																? GetType().MakeGenericType(GenericType)
																: GetType());
			}
		}
		private Type _runtimeTypeCache;
		
		#endregion

		#region Constructors

		protected FrameworkElement()
		{
			RegisterProperty("Id", null);
			RegisterProperty("Value", null);
		}

		#endregion
    }
}