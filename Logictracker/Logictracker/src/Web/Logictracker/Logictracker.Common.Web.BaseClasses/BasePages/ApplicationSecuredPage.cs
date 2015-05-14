using System;
using System.Globalization;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.SecurityObjects;

namespace Logictracker.Web.BaseClasses.BasePages
{
    /// <summary>
    /// Base page that implements all the security steps associated to the application functions.
    /// </summary>
    public abstract class ApplicationSecuredPage : SessionSecuredPage
    {
        #region Public Properties

        /// <summary>
        /// Gets the page associated module.
        /// </summary>
        public Module Module { get { return WebSecurity.GetUserModuleByRef(GetRefference()); } }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the page title.
        /// </summary>
        protected override string PageTitle { get { return Module != null ? string.Format("{0} - {1}", ApplicationTitle, CultureManager.GetMenu(Module.Name)) : ApplicationTitle; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Checks the user privileges for accessing the page.
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ApplyModuleSecurity();
        }

        /// <summary>
        /// Applies all the needed security associated to the application module security.
        /// </summary>
        protected virtual void ApplyModuleSecurity()
        {
            if (!WebSecurity.Authenticated) OnSessionLoss();
            else if (Module == null || !Module.View)
            {
                Session.Add("module_name", GetRefference());
                Response.Redirect("~/SinAcceso.aspx");
            }
        }

        /// <summary>
        /// Gets the module refference.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetRefference() { throw new NotImplementedException(string.Format(CultureManager.GetError("MUST_IMPLEMENT_MEMBER"), "GetRefference")); }

        protected void ThrowError(string errorName, params object[] args)
        {
            throw new ApplicationException(string.Format(CultureManager.GetError(errorName), args));
        }

		protected void ThrowError(Exception e, string errorName, params object[] args)
		{
			throw new ApplicationException(string.Format(CultureManager.GetError(errorName), args), e);
		}
		
		protected void ThrowMustEnter(string resource, string variable)
        {
            throw new ApplicationException(string.Format(CultureManager.GetError("MUST_ENTER_VALUE"), CultureManager.GetString(resource, variable)));
        }

        protected void ThrowMustEnter(string labelVariable) { ThrowMustEnter("Labels", labelVariable); }

        protected void ThrowDuplicated(string resource, string variable)
        {
            throw new ApplicationException(string.Format(CultureManager.GetError("DUPLICATED"), CultureManager.GetString(resource, variable)));
        }

        protected void ThrowDuplicated(string labelVariable) { ThrowDuplicated("Labels", labelVariable); }

        protected void ThrowInvalidValue(string resource, string variable)
        {
            throw new ApplicationException(string.Format(CultureManager.GetError("INVALID_VALUE"), CultureManager.GetString(resource, variable)));
        }

        protected void ThrowInvalidValue(string labelVariable) { ThrowInvalidValue("Labels", labelVariable); }

        protected void ThrowCantDelete(string resource, string variable)
        {
            throw new ApplicationException(string.Format(CultureManager.GetError("CANT_DEL"), CultureManager.GetString(resource, variable)));
        }

        protected void ThrowCantDelete(string labelVariable) { ThrowDuplicated("Labels", labelVariable); }
        
        #endregion

        #region Validation
        protected int ValidateEntity(int value, string variableName)
        {
            return ValidateEntity(value, "Entities", variableName);
        }
        protected int ValidateEntity(int value, string resourceName, string variableName)
        {
            if (value <= 0) ThrowMustEnter(resourceName, variableName);
            return value;
        }
        protected string ValidateEmpty(string value, string variableName)
        {
            return ValidateEmpty(value, "Labels", variableName);
        }
        protected string ValidateEmpty(string value, string resourceName, string variableName)
        {
            var codigo = value.Trim();
            if (string.IsNullOrEmpty(codigo)) ThrowMustEnter(resourceName, variableName);
            return codigo;
        }
        protected DateTime ValidateEmpty(DateTime? value, string variableName)
        {
            return ValidateEmpty(value, "Labels", variableName);
        }

        protected DateTime ValidateEmpty(DateTime? value, string resourceName, string variableName)
        {
            if (!value.HasValue) ThrowMustEnter(resourceName, variableName);
            return value.Value;
        }
        protected TimeSpan ValidateEmpty(TimeSpan? value, string variableName)
        {
            return ValidateEmpty(value, "Labels", variableName);
        }

        protected TimeSpan ValidateEmpty(TimeSpan? value, string resourceName, string variableName)
        {
            if (!value.HasValue) ThrowMustEnter(resourceName, variableName);
            return value.Value;
        }
        
        protected short ValidateInt16(string value, string variableName)
        {
            return ValidateInt16(value, "Labels", variableName);
        }

        protected short ValidateInt16(string value, string resourceName, string variableName)
        {
            short ival;
            var sval = value.Trim();
            if (!short.TryParse(sval, out ival)) ThrowInvalidValue(resourceName, variableName);
            return ival;
        }

        protected int ValidateInt32(string value, string variableName)
        {
            return ValidateInt32(value, "Labels", variableName);
        }
        protected int ValidateInt32(string value, string resourceName, string variableName)
        {
            int ival;
            var sval = value.Trim();
            if (!int.TryParse(sval, out ival)) ThrowInvalidValue(resourceName, variableName);
            return ival;
        }

        protected double ValidateDouble(string value, string variableName)
        {
            return ValidateDouble(value, "Labels", variableName);
        }
        protected double ValidateDouble(string value, string resourceName, string variableName)
        {
            double ival;
            var sval = value.Trim().Replace(',', '.');
            if (!double.TryParse(sval, NumberStyles.Any, CultureInfo.InvariantCulture, out ival))
                ThrowInvalidValue(resourceName, variableName);
            return ival;
        }

        protected int ValidateHigher(int higher, int lower, string variableName)
        {
            return ValidateHigher(higher, lower, "Labels", variableName);
        }
        protected int ValidateHigher(int higher, int lower, string resourceName, string variableName)
        {
            if (higher < lower) ThrowInvalidValue(resourceName, variableName);
            return higher;
        }
        protected T ValidateHigher<T>(T higher, T lower, string variableName)
            where T : IComparable
        {
            return ValidateHigher(higher, lower, "Labels", variableName);
        }
        protected T ValidateHigher<T>(T higher, T lower, string resourceName, string variableName)
            where T:IComparable
        {
            if (lower.CompareTo(higher) > 0) ThrowInvalidValue(resourceName, variableName);
            return higher;
        }
        #endregion
    }
}