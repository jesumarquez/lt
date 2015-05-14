#region Usings

using System;

#endregion

namespace Logictracker.Description.Attributes
{
    
    /// <summary>
    /// Define los atributos de un miembro, parte de FrameworkElement.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ElementAttributeAttribute : FrameworkElementAttribute
    {
        /// <summary>
        /// Identificador del elemento XML que describe se asocia al 
        /// miembro que declara el atributo.
        /// </summary>
        public string XKey;

        /// <summary>
        /// Numero de orden relativo a los otros miembros en que se carga
        /// el miembro. Si no se establece, se cargan el orden que aparecen
        /// en el XML.
        /// </summary>
        public int LoadOrder;

        /// <summary>
        /// Si se establece verdadero, el miembro funciona como SmartProperty
        /// lo que la hace Bindeable, Monitoreable y Demas, y los getter y setter
        /// se deben implementar asi:
		/// [ElementAttribute(XName = "Ejemplo", IsSmartProperty = true, IsRequired = true)]
		/// public String Ejemplo
		/// {
		///		get { return (String)GetValue("Ejemplo"); }
		///		set { SetValue("Ejemplo", value); }
		///	}
		/// </summary>
        public bool IsSmartProperty;

        /// <summary>
        /// Si se establece verdadero, el miembro es obligatorio. Por defecto 
        /// es falso.
        /// </summary>
        public bool IsRequired;

        /// <summary>
        /// Si se establece verdadero, el miembro es lectura solamente.
        /// Se utiliza en miembros que ofrecen datos de resultado
        /// en runtime. Por defecto es falso.
        /// </summary>
        public bool IsReadOnly;

        /// <summary>
        /// Si se establece verdadero, al modificar el valor no requiere
        /// reiniciar el servicio para que tome efecto. Por defecto es
        /// verdadero.
        /// </summary>
        public bool RequiresRestart;

        /// <summary>
        /// Valor por defecto al crear un nuevo elemento.
        /// </summary>
        public object DefaultValue;

        /// <summary>
        /// Construye el Atributo con su parametros por defecto.
        /// </summary>
        public ElementAttributeAttribute()
        {
            LoadOrder = -1;
            RequiresRestart = true;
            IsBindable = true;
            IsReadOnly = false;
            IsRequired = false;
        }
    }
}
