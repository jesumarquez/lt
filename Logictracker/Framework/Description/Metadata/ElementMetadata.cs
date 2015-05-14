#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;

#endregion

namespace Logictracker.Description.Metadata
{
    /// <summary>
    /// Coleccion de Metadata que describe la estructura de un Tipo
    /// concreto de FrameworkElement. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Por cada tipo de <see cref="FrameworkElement"/> que se descubre durante la fase 
    /// de discovery, se construye un <see cref="ElementMetadata"/> y se lo
    /// agrega en el <see cref="MetadataDirectory"/>.
    /// </para>
    /// <para>
    /// Al construir el <see cref="ElementMetadata"/>, este analiza 
    /// la clase concreta, sus miembros y las interfaces que implementa.
    /// Busca todos los miembros que esten etiquetados con el atributo 
    /// <see cref="ElementAttributeAttribute"/> y construye un
    /// grafico de la clase.
    /// </para>
    /// </remarks>
    public class ElementMetadata
    {
        private readonly List<ElementAttributeMetadata> _attributes = new List<ElementAttributeMetadata>();
        private readonly List<ItemConstraintMetadata> _itemConstraints = new List<ItemConstraintMetadata>();
        private readonly List<ItemManagmentCommandMetadata> _itemManagmentCommands = new List<ItemManagmentCommandMetadata>();

		/// <summary>
        /// Construye la descripcion y de un elemento.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="type"></param>
        public ElementMetadata(FrameworkElementAttribute attribute, Type type)
        {
            Type = type;
            XName = attribute.XName;
            XNamespace = attribute.XNamespace;
            IsContainer = attribute.IsContainer;
            IsBindable = attribute.IsBindable;
        }

        /// <summary>
        /// Carga la metadata de los miembros 
        /// </summary>
        public void LoadTypeMembers()
        {
            // ya se hizo?
            if (_attributes.Count != 0) return;

            // enumero los tipos calidos 
            
            if (IsContainer)
            {
                var icList = Attribute.GetCustomAttributes(Type, typeof(ItemConstraintAttribute));
                foreach (var itemConstraint in icList.OfType<ItemConstraintAttribute>())
                {
                    _itemConstraints.Add(new ItemConstraintMetadata(itemConstraint));
                }

                var imcaList = Attribute.GetCustomAttributes(Type, typeof (ItemManagmentCommandAttribute));
                foreach (var itemManagmentCommand in imcaList.OfType<ItemManagmentCommandAttribute>())
                {
                    _itemManagmentCommands.Add(new ItemManagmentCommandMetadata(itemManagmentCommand));
                }
            }

            // proceso los miembros de la clase.
            var typeMembers = Type.GetMembers();
        	var membersMetadata = typeMembers
				.Select(infoMember => new { infoMember, memberAttribute = GetCustomAttribute(infoMember) })
				.Where(@t => @t.memberAttribute != null)
				.Select(@t => new ElementAttributeMetadata
					{
						Parent = this,
						MemberInfo = @t.infoMember,
						XName = @t.memberAttribute.XName,
						XNamespace = @t.memberAttribute.XNamespace,
						DefaultValue = @t.memberAttribute.DefaultValue,
						IsSmartProperty = @t.memberAttribute.IsSmartProperty,
						IsBindable = @t.memberAttribute.IsBindable,
						IsReadOnly = @t.memberAttribute.IsReadOnly,
						IsRequired = @t.memberAttribute.IsRequired,
						LoadOrder = @t.memberAttribute.LoadOrder,
						RequiresRestart = @t.memberAttribute.RequiresRestart,
					});
			
			foreach (var membermetadata in membersMetadata)
            {
            	_attributes.Add(membermetadata);
            }

            // corrijo los ordenes de carga 
            var maxLoadOrder = _attributes.Select(m => m.LoadOrder).Concat(new[]{-1}).Max() + 1;
            foreach(var mm in (from m in _attributes where m.LoadOrder < 0 select m))
            {
                mm.LoadOrder = maxLoadOrder++;
            }
        }

		private static ElementAttributeAttribute GetCustomAttribute(MemberInfo infoMember)
		{
			return Attribute.GetCustomAttribute(infoMember, typeof (ElementAttributeAttribute)) as ElementAttributeAttribute;
		}

        /// <summary>
        /// Obtiene una coleccion enumerable con la metadata de los miembros
        /// internos.
        /// </summary>
        /// <returns>coleccion de objetos ElementAttributeMetadata.</returns>
        public IEnumerable<ElementAttributeMetadata> Members()
        {
            return _attributes;
        }

        /// <summary>
        /// Obtiene una coleccion enumerable con la metadata de los miembros
        /// internos que en el XML sean attributes.
        /// </summary>
        /// <returns>coleccion de objetos ElementAttributeMetadata.</returns>
        public IEnumerable<ElementAttributeMetadata> Attributes()
        {
            return from m in _attributes 
                   orderby m.LoadOrder ascending 
                   select m;
        }

        /// <summary>
        /// Obtiene la metadata del miembro 
        /// internos del tipo especificado.
        /// </summary>
        /// <returns>Colleccion de objetos ElementAttributeMetadata.</returns>
        public ElementAttributeMetadata Member(string memberName)
        {
            return (from i in Members() where i.MemberInfo.Name.Equals(memberName) select i).First();
        }
        
        /// <summary>
        /// Tipo de la Clase que implementa el elemento.
        /// </summary>
        public Type Type { get; protected set; }

        /// <summary>
        /// Nombre del Tag XML que representa el elemento en un XML.
        /// </summary>
        public string XName { get; protected set; }

        /// <summary>
        /// Nombre del Tag XML que representa el elemento en un XML.
        /// </summary>
        public string XNamespace { get; protected set; }

        /// <summary>
        /// Si se establece verdadero, el miembro es contenedor de otros
        /// FrameworkElement. Por defecto es verdadero.
        /// </summary>
        public bool IsContainer { get; protected set; }

        /// <summary>
        /// Si se establece verdadero, el miembro es contenedor de otros
        /// FrameworkElement. Por defecto es verdadero.
        /// </summary>
        public bool IsBindable { get; protected set; }

        public override string ToString()
        {
            var r = new StringBuilder("<").Append(XName);

            foreach (var a in Attributes())
            {
                r.AppendFormat(" {0}=\"{1}\"", a.XName, (a.IsRequired ? "{Required}" : (a.DefaultValue ?? "{Null}").ToString()));
            }
            return r.Append("/>").ToString();
        }

    }
}
