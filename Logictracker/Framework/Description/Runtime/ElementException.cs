#region Usings

using System;
using System.ComponentModel;

#endregion

namespace Logictracker.Description.Runtime
{
    /// <summary>
    /// Enumeracion de las posibles causas por la que puede producirse 
    /// una excepcion en el Framework. 
    /// </summary>
    public enum ElementErrorCodes
    {
        ///<summary>La operacion no puede realizarse.</summary>
        [Description("La operacion no puede realizarse.")]
        InvalidOperation,
        ///<summary>Elemento no reconocido, No esta en el directorio.</summary>
        [Description("Elemento no reconocido, No esta en el directorio")]
        UnrecognizedElement,
        ///<summary>Elemento duplicado, No esta en el directorio.</summary>
        [Description("Elemento duplicado, No esta en el directorio.")]
        DuplicatedElement,
        ///<summary>Atributo duplicado, No esta en el directorio.</summary>
        [Description("Atributo duplicado, No esta en el directorio.")]
        DuplicatedAttribute,
        ///<summary>Excepcion no especificada, ver InnerException.</summary>
        [Description("Excepcion no especificada, vea InnerException.")]
        UnspecifiedException,
        ///<summary>Un elemento generico no tiene definido el atributo GenericType que lo instancia.</summary>
        [Description("Un elemento generico no tiene definido el atributo GenericType que lo instancia.")]
        GenericTypeAttributeExpected,
        ///<summary>Un elemento generico define un tipo no se encuentra.</summary>
        [Description("Un elemento generico define un tipo no se encuentra.")]
        GenericTypeNotFound,
        ///<summary>Un atributo definido como requerido no esta presente.</summary>
        [Description("Un atributo definido como requerido no esta presente.")]
        RequiredAttributeNotFound,
        ///<summary>Un elemento definido como requerido no esta presente.</summary>
        [Description("Un elemento definido como requerido no esta presente.")]
        RequiredElementNotFound,
        ///<summary>La propiedad ya esta registrada en el elemento.</summary>
        [Description("La propiedad ya esta registrada en el elemento.")]
        PropertyAlreadyRegistered,
        ///<summary>La propiedad no esta registrada en el elemento.</summary>
        [Description("La propiedad no esta registrada en el elemento.")]
        PropertyNotRegistered,
        ///<summary>Imposibe crear el elemento.</summary>
        [Description("Imposibe crear el elemento.")]
        UnableToCreateFrameworkElement,
        ///<summary>Error de formato procesando un Binding.</summary>
        [Description("Error procesando un Binding.")]
        BindingError,
        ///<summary>La extension de marca propietaria no se encuentra en diccionario.</summary>
        [Description("La extension no se encuentra en diccionario.")]
        CustomMarkupExtensionNotFound,
    }

    public sealed class ElementException : ApplicationException
    {
        public ElementErrorCodes ErrorCode { get; private set; }

		public ElementException(ElementErrorCodes errorCode, String message, Exception exception) : base(errorCode.Description(message), exception)
        {
            ErrorCode = errorCode;
        }

		public ElementException(ElementErrorCodes errorCode, String message) : base(errorCode.Description(message))
        {
            ErrorCode = errorCode;
        }
    }

	public static class ElementErrorCodesX
	{
		public static String Description(this ElementErrorCodes value)
		{
			try
			{
				var fi = value.GetType().GetField(value.ToString());
				var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
				return attributes.Length > 0 ? attributes[0].Description : value.ToString();
			}
			catch
			{
				return value.ToString();
			}
		}
		public static String Description(this ElementErrorCodes value, String message)
		{
			return String.Format("{0}{1}{2}", Description(value), Environment.NewLine, message);
		}
    }
}