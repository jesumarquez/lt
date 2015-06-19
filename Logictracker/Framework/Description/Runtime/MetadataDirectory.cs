#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.CustomMarkupExtensions;
using Logictracker.Description.Metadata;

#endregion

namespace Logictracker.Description.Runtime
{
    public sealed class MetadataDirectory
    {
	    #region Fields

	    public const String FrameworkNamespaceUri = "http://walks.ath.cx/framework";
	    public const String GenericsNamespaceUri = "http://walks.ath.cx/generics";
	    public const String StaticsNamespaceUri = "http://walks.ath.cx/statics";
	    public static readonly XNamespace GenericsNamespace = XNamespace.Get(GenericsNamespaceUri);
	    public static readonly XNamespace FrameworkNamespace = XNamespace.Get(FrameworkNamespaceUri);
	    public static readonly XNamespace StaticsNamespace = XNamespace.Get(StaticsNamespaceUri);
	    private static readonly List<ElementMetadata> Elements = new List<ElementMetadata>();
	    private static readonly Dictionary<String, ICustomMarkupExtension> CustomMarkupExtensions = new Dictionary<String, ICustomMarkupExtension>();

	    #endregion

	    #region Public Methods

	    /// <summary>
	    /// Carga al diccionario, todos los objetos FrameworkElement en los 
	    /// ensamblados del directorio Especificado.
	    /// </summary>
	    /// <exception cref="ElementException">
	    /// Es pueden producir
	    /// </exception>
	    public static void Load()
	    {
		    var target = Environment.CurrentDirectory;
            STrace.Debug(typeof(ApplicationLoader).FullName, "Loading Assemblies...");
		    LoadAssemblies(target);
		    foreach (var element in Elements)
		    {
                //STrace.Debug(typeof(ApplicationLoader).FullName, String.Format("Loading Assembly {0}...", element.XName));
			    element.LoadTypeMembers();
		    }
	    }

	    /// <summary>
	    /// Obtiene una colleccion enumerable con las extensiones sintacticas
	    /// internas.
	    /// </summary>
	    /// <returns>colleccion de objetos FrameworkElement.</returns>
	    public static ICustomMarkupExtension GetCustomMarkupExtensions(String keyword)
	    {
		    return CustomMarkupExtensions.ContainsKey(keyword) ? CustomMarkupExtensions[keyword] : null;
	    }

	    /// <summary>
	    /// Obtiene una colleccion enumerable con los elementos 
	    /// internos del tipo especificado.
	    /// </summary>
	    /// <returns>Colleccion de objetos FrameworkElement.</returns>
	    public static ElementMetadata GetMetadata(Type type)
	    {
		    return (from i in Metadatas() where i.GetType() == type select i).FirstOrDefault();
	    }

	    /// <summary>
	    /// Obtiene una colleccion enumerable con los elementos 
	    /// internos del tipo especificado.
	    /// </summary>
	    /// <returns>Colleccion de objetos FrameworkElement.</returns>
	    public static ElementMetadata GetMetadata(XElement xElement)
	    {
		    return (from i in Metadatas() where xElement.Name.Equals(XName.Get(i.XName, i.XNamespace)) select i).FirstOrDefault();
	    }

	    /// <summary>
	    /// Obtiene el tipo en runtime en base a un objeto XElement.
	    /// </summary>
	    /// <param name="xElement">Objeto XElement a analizar.</param>
	    /// <returns></returns>
	    public static Type GetRuntimeType(XElement xElement)
	    {
		    var type = (from t in Metadatas()
		                where xElement.Name.Equals(XName.Get(t.XName, t.XNamespace))
		                select t.Type).FirstOrDefault();

		    if (type == null)
			    throw new ElementException(ElementErrorCodes.UnrecognizedElement, xElement.Name + Environment.NewLine + xElement);

		    return type.ContainsGenericParameters ?
			                                          type.MakeGenericType(new[] {RequireGenericType(xElement)}) : type;
	    }

	    /// <summary>
	    /// Obtiene el tipo generico dado, segun attributo GenericType
	    /// para terminar de construir el Type necesario al instanciar.
	    /// </summary>
	    /// <param name="xElement"></param>
	    /// <returns></returns>
	    public static Type GetGenericType(XElement xElement)
	    {
		    return GetGenericType(xElement, false);
	    }

	    #endregion

		#region Private Methods

		private static void LoadAssemblies(String path)
	    {
            STrace.Debug(typeof(ApplicationLoader).FullName, "Getting DLL filenames...");
		    var files = Directory.GetFiles(path, "Logictracker*.dll");
            STrace.Debug(typeof(ApplicationLoader).FullName, String.Format("{0} DLL found...", files.Count()));
            var asms = new List<Assembly>();
		    foreach (var file in files)
		    {
			    try
			    {
                    //STrace.Debug(typeof(ApplicationLoader).FullName, String.Format("Assembly.LoadFrom({0}...", file));
				    var a = Assembly.LoadFrom(file);
                    asms.Add(a);
 			    }
			    catch (BadImageFormatException)
			    {
				    STrace.Error(typeof (MetadataDirectory).FullName, String.Format("Could not load assembly: {0}", file));
			    }
		    }

            foreach (var a in asms)
            {
                try
                {
                     LoadAssembly(a);
                }
                catch (BadImageFormatException)
                {
                    STrace.Error(typeof(MetadataDirectory).FullName, String.Format("Could not load assembly: {0}", a));
                }
            }
	    }

	    private static void LoadAssembly(Assembly assembly)
	    {
		    try
		    {
                STrace.Debug(typeof(MetadataDirectory).FullName, String.Format("Loading Assembly: {0}", assembly.FullName));
                var tps = assembly.GetTypes();
                //STrace.Debug(typeof(MetadataDirectory).FullName, String.Format("Loading Assembly: {0} Part#{1}", assembly.FullName, 0));
			    foreach (var type in tps)
			    {
                    //STrace.Debug(typeof(MetadataDirectory).FullName, String.Format("Loading Assembly: {0} Part#{1}", assembly.FullName, 1));
				    var frameworkElementAttribute = Attribute.GetCustomAttribute(type, typeof (FrameworkElementAttribute)) as FrameworkElementAttribute;

                    //STrace.Debug(typeof(MetadataDirectory).FullName, String.Format("Loading Assembly: {0} Part#{1}", assembly.FullName, 2));
				    if (frameworkElementAttribute != null)
				    {
                        //STrace.Debug(typeof(MetadataDirectory).FullName, String.Format("Loading Assembly: {0} Part#{1}", assembly.FullName, 3));
					    var elementMetadata = new ElementMetadata(frameworkElementAttribute, type);

					    if (Elements.Exists(element => element.XName == frameworkElementAttribute.XName && element.XNamespace == frameworkElementAttribute.XNamespace))
						    throw new ElementException(ElementErrorCodes.UnspecifiedException, elementMetadata.XName);

                        //STrace.Debug(typeof(MetadataDirectory).FullName, String.Format("Loading Assembly: {0} Part#{1}", assembly.FullName, 4));
					    //STrace.Debug(typeof(MetadataDirectory).FullName, "Loaded tag - {0} (namespace: {1})", frameworkElementAttribute.XName, frameworkElementAttribute.XNamespace);
					    Elements.Add(elementMetadata);
				    }

                    //STrace.Debug(typeof(MetadataDirectory).FullName, String.Format("Loading Assembly: {0} Part#{1}", assembly.FullName, 5));
				    var customMarkupExtensionAttribute = Attribute.GetCustomAttribute(type, typeof (CustomMarkupExtensionAttribute)) as CustomMarkupExtensionAttribute;

				    if (customMarkupExtensionAttribute == null) continue;
				    var constructor = type.GetConstructor(Type.EmptyTypes);
				    var customMarkupExtension = constructor.Invoke(null) as ICustomMarkupExtension;
                    //STrace.Debug(typeof(MetadataDirectory).FullName, String.Format("Loading Assembly: {0} Part#{1}", assembly.FullName, 6));
				    if (customMarkupExtension == null)
					    throw new ElementException(ElementErrorCodes.UnableToCreateFrameworkElement, type.Name);
                    //STrace.Debug(typeof(MetadataDirectory).FullName, String.Format("Loading Assembly: {0} Part#{1}", assembly.FullName, 7));
				    CustomMarkupExtensions.Add(customMarkupExtensionAttribute.Keyword, customMarkupExtension);
			    }
		    }
		    catch (ReflectionTypeLoadException e)
		    {
                STrace.Exception(typeof(MetadataDirectory).FullName, e, String.Format("Exception loading Assembly: {0}", assembly.FullName));
		    }
	    }

	    private static IEnumerable<ElementMetadata> Metadatas()
	    {
		    return Elements.AsEnumerable();
	    }

	    private static Type GetGenericType(XElement xElement, Boolean throwns)
	    {
		    try
		    {
			    var xAttribute = xElement.Attribute(GenericsNamespace + "Type");
			    if (xAttribute == null)
			    {
				    if (throwns == false) return null;
				    throw new ElementException(ElementErrorCodes.GenericTypeAttributeExpected, xElement.Name.ToString());
			    }
			    var type = Type.GetType(xAttribute.Value,true);
			    if (type == null)
			    {
				    if (throwns == false) return null;
				    throw new ElementException(ElementErrorCodes.GenericTypeNotFound, xAttribute.Value);
			    }
			    return type;
		    }
		    catch (ElementException)
		    {
			    if (throwns) throw;
			    return null;
		    }
	    }

	    private static Type RequireGenericType(XElement xElement)
	    {
		    return GetGenericType(xElement, true);
	    }

	    #endregion
    }
}