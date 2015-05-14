#region Usings

using System.Reflection;

#endregion

namespace Logictracker.Description.Metadata
{
    /// <summary>
    /// Descripcion de un miembro de un objeto FrameworkElement.
    /// Por medio de la Metadata, dirije el acceso a los miembros de
    /// cada instancias concreta.
    /// </summary>
    public class ElementAttributeMetadata
    {
        /// <summary>
        /// Metadata de la clase que contiene al miembro.
        /// </summary>
        public ElementMetadata Parent { get; set; }

        /// <summary>
        /// Informacion del Miembro de la clase contenedora.
        /// </summary>
        public MemberInfo MemberInfo;

        /// <summary>
        /// Indica si el miembro es un arreglo de elementos XML.
        /// </summary>
        public bool IsArray { get; set; }

        /// <summary>
        /// Nombre del Tag XML que representa el atriburo en un XML.
        /// </summary>
        public string XName { get; set;  }

        /// <summary>
        /// Identificador del Tag XML que representa el elemento simple en un XML.
        /// </summary>
        public string XNamespace { get; set; }

        /// <summary>
        /// Indicador que la propiedad es tipo SmartProperty
        /// </summary>
        public bool IsSmartProperty { get; set; }

        /// <summary>
        /// Numero de orden relativo a los otros miembros en que se carga
        /// el miembro. Si no se establece, se cargan el orden que aparecen
        /// en el XML.
        /// </summary>
        /// <remarks>
        /// En metodos, no tiene efecto.
        /// </remarks>
        public int LoadOrder;

        /// <summary>
        /// Si se establece verdadero, el miembro es obligatorio. Por defecto 
        /// es falso.
        /// </summary>
        /// <remarks>
        /// En metodos, no tiene efecto.
        /// </remarks>
        public bool IsRequired;

        /// <summary>
        /// Si se establece verdadero, el miembro es lectura solamente.
        /// Se utiliza en miembros que ofrecen datos de resultado
        /// en runtime. Por defecto es falso.
        /// </summary>
        public bool IsReadOnly;

        /// <summary>
        /// Si se establece verdadero, el miembro puede atarse a otro 
        /// elemento. Por defecto es falso.
        /// </summary>
        public bool IsBindable;

        /// <summary>
        /// Si se establece falso, al modificar el valor no requiere
        /// reiniciar el servicio para que tome efecto. Por defecto es
        /// verdadero.
        /// </summary>
        public bool RequiresRestart;

        /// <summary>
        /// Valor por defecto al crear un nuevo elemento.
        /// </summary>
        public object DefaultValue;

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool showParent)
        {
            var r="";
            if (showParent)
                r += "<" + Parent.XName + " ";
            r += string.Format("{0}=\"{1}\"", XName, (IsRequired ? "{Required}" : (DefaultValue != null ? DefaultValue.ToString() : "{Null}")));
            if (showParent)
                r += "/>";
            return r;
        }
        
    }
}
