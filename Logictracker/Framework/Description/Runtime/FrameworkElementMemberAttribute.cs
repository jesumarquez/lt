﻿using System;

namespace Urbetrack.Managment
{
    
    /// <summary>
    /// Define los atributos de un miembro, parte de FrameworkElement.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FrameworkElementMemberAttribute : FrameworkElementAttribute
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
        /// Si se establece verdadero, el miembro es obligatorio. Por defecto 
        /// es falso.
        /// </summary>
        public bool IsAttribute;

        /// <summary>
        /// Si se establece verdadero, el miembro funciona como SmartProperty
        /// lo que la hace Bindeable, Monitoreable y Demas.
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
        /// Si se establece verdadero, el miembro puede atarse a otro 
        /// elemento. Por defecto es falso.
        /// </summary>
        public bool IsBindable;

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
        public FrameworkElementMemberAttribute()
        {
            LoadOrder = -1;
            RequiresRestart = true;
            IsBindable = true;
            IsReadOnly = false;
            IsRequired = false;
            IsAttribute = true;
        }
    }
}
