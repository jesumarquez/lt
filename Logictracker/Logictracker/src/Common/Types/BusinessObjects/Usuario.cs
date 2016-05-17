using System;
using System.Collections.Generic;
using System.Linq;
using Iesi.Collections;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Organizacion;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    /// <summary>
    /// Entity that represents a user of the application.
    /// </summary>
    [Serializable]
    public class Usuario : IAuditable
    {
        public static class Params
        {
            public const string Email = "email";
            public const string Fantasia = "fantasia";
            public const string Homepage = "homepage";
            public const string MenuStarCollapsed = "menu.startcollapsed";
            public const string RegionalListSeparator = "regional.listseparator";
            public const string ExcelFolder = "excel.folder";
            public const string MonitorShowDriverName = "monitor.showdrivername";
        }

        public static class NivelAcceso
        {
            public const short NoAccess = 0;
            public const short Public = 1;

            public const short User = 20;
            public const short Installer = 30;
            public const short AdminUser = 50;

            public const short SysAdmin = 60;

            public const short Developer = 80;
            public const short SuperAdmin = 100;
        }

        private ISet<Perfil> _perfiles;
        private ISet<Coche> _coches;
        private ISet<Empresa> _empresas;
        private ISet<Linea> _lineas;
        private ISet<Transportista> _transportistas;
        private ISet<CentroDeCostos> _centrosCostos;
        private ISet<TipoMensaje> _tiposMensaje;
        private ISet<IpRange> _ipRanges;

        Type IAuditable.TypeOf() { return GetType(); }

        public virtual int Id { get; set; }
        /// <summary>
        /// Information about the user name, address, etc.
        /// </summary>
        public virtual Entidad Entidad { get; set; }

        public virtual Empleado Empleado { get; set; }
        /// <summary>
        /// Creation date.
        /// </summary>
        public virtual DateTime? FechaAlta { get; set; }
        /// <summary>
        /// Expiration date.
        /// </summary>
        public virtual DateTime? FechaBaja { get; set; }
        /// <summary>
        /// The type of user (access level).
        /// </summary>
        public virtual short Tipo { get; set; }
        /// <summary>
        /// A list of assigned profiles.
        /// </summary>
        public virtual ISet<Perfil> Perfiles { get { return _perfiles ?? (_perfiles = new HashSet<Perfil>()); } }
        /// <summary>
        /// The current theme to use for this user.
        /// </summary>
        public virtual string Theme { get; set; }
        /// <summary>
        /// The logo to be displayed to the user.
        /// </summary>
        public virtual string Logo { get; set; }
        /// <summary>
        /// The user configured culture.
        /// </summary>
        public virtual string Culture { get; set; }
        /// <summary>
        /// TimeZone for displaying DateTimes to the user.
        /// </summary>
        public virtual string TimeZoneId { get; set; }
        /// <summary>
        /// The client wich the user is assigned to.
        /// </summary>
        public virtual string Client { get; set; }
        /// <summary>
        /// The user name.
        /// </summary>
        public virtual string NombreUsuario { get; set; }
        /// <summary>
        /// The user password.
        /// </summary>
        public virtual string Clave { get; set; }
        /// <summary>
        /// Determines if the user filters by transport companies.
        /// </summary>
        public virtual bool PorTransportista { get; set; }
        /// <summary>
        /// Determines if the user filters by transport companies.
        /// </summary>
        public virtual bool PorCentroCostos { get; set; }
        
        public virtual bool PorTipoMensaje { get; set; }
        /// <summary>
        /// Determines if the user filters by company.
        /// </summary>
        public virtual bool PorEmpresa { get; set;}
        /// <summary>
        /// Determines if the user filters by location.
        /// </summary>
        public virtual bool PorLinea { get; set; }
        /// <summary>
        /// Determines if the user filter by vehicle.
        /// </summary>
        public virtual bool PorCoche { get; set; }
        /// <summary>
        /// Determines if the user is enabled or not.
        /// </summary>
        public virtual bool Inhabilitado { get; set; }
        /// <summary>
        /// Determines if the user is enabled or not to change his password.
        /// </summary>
        public virtual bool InhabilitadoCambiarPass { get; set; }
        /// <summary>
        /// Determines if the user is enabled or not to change his Time Zone.
        /// </summary>
        public virtual bool InhabilitadoCambiarUso { get; set; }
        /// <summary>
        /// Determines date until the user is enabled.
        /// </summary>
        public virtual DateTime? FechaExpiracion { get; set; }
        /// <summary>
        /// Shows or not the mobiles that are not assigned to an specific transportista.
        /// </summary>
        public virtual Boolean MostrarSinTransportista { get; set; }
        /// <summary>
        /// Gets the vehicles assigned to the user.
        /// </summary>
        public virtual ISet<Coche> Coches { get { return _coches ?? (_coches = new HashSet<Coche>()); } }
        /// <summary>
        /// Gets the transports companies assigned to the user.
        /// </summary>
        public virtual ISet<Transportista> Transportistas { get { return _transportistas ?? (_transportistas = new HashSet<Transportista>()); } }
        /// <summary>
        /// Gets the cost centers assigned to the user.
        /// </summary>
        public virtual ISet<CentroDeCostos> CentrosCostos { get { return _centrosCostos ?? (_centrosCostos = new HashSet<CentroDeCostos>()); } }
        
        public virtual ISet<TipoMensaje> TiposMensaje { get { return _tiposMensaje ?? (_tiposMensaje = new HashSet<TipoMensaje>()); } }
        /// <summary>
        /// Gets the companies assigned to the user.
        /// </summary>
        public virtual ISet<Empresa> Empresas { get { return _empresas ?? (_empresas = new HashSet<Empresa>()); } }
        /// <summary>
        /// Gets the locations assigned to the user.
        /// </summary>
        public virtual ISet<Linea> Lineas { get { return _lineas ?? (_lineas = new HashSet<Linea>()); } }
        /// <summary>
        /// Determines if the user filters by any criteria.
        /// </summary>
        public virtual bool AppliesFilters { get { return PorEmpresa || PorLinea || PorTransportista || PorCoche || PorCentroCostos; } }

        public virtual ISet<IpRange> IpRanges
        {
            get { return _ipRanges ?? (_ipRanges = new HashSet<IpRange>()); } 
        }

        private IList<ParametroUsuario> _parametros;
        public virtual IList<ParametroUsuario> Parametros
        {
            get { return _parametros ?? (_parametros = new List<ParametroUsuario>()); }
            set { _parametros = value; }
        }

        /// <summary>
        /// Determines if the givenn object is equal to the user.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            var castObj = obj as Usuario;

            return castObj != null && (Id == castObj.Id) && (Id != 0);
        }

        /// <summary>
        /// Gets the hash code associated to the user.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() { return 27*57*Id.GetHashCode(); }

        /// <summary>
        /// Clear all the profiles assigned to the user.
        /// </summary>
        public virtual void ClearPerfiles() { Perfiles.Clear(); }

        /// <summary>
        /// Adds the givenn profile to the user.
        /// </summary>
        /// <param name="perfil"></param>
        public virtual void AddPerfil(Perfil perfil) { Perfiles.Add(perfil); }

        public virtual void ClearCoches() { Coches.Clear(); }

        public virtual void AddCoche(Coche coche) { Coches.Add(coche); }

        public virtual void AddTransportista(Transportista transportista) { Transportistas.Add(transportista); }

        public virtual void AddCentro(CentroDeCostos centroDeCostos) { CentrosCostos.Add(centroDeCostos); }

        public virtual void AddTipoMensaje(TipoMensaje tipoMensaje) { TiposMensaje.Add(tipoMensaje); }

        public virtual void ClearTransportistas() { Transportistas.Clear(); }

        public virtual void ClearCentros() { CentrosCostos.Clear(); }

        public virtual void ClearTiposMensaje() { TiposMensaje.Clear(); }

        public virtual void AddEmpresa(Empresa e) { Empresas.Add(e); }

        public virtual void ClearEmpresas() { Empresas.Clear(); }

        public virtual void AddLinea(Linea l) { Lineas.Add(l); }

        public virtual void ClearLineas() { Lineas.Clear(); }

        public virtual bool IsInIpRange(string ip)
        {
            return IpRanges.Count == 0 || IpRanges.Cast<IpRange>().Any(ipRange => ipRange.IsInRange(ip));
        }

        public virtual bool IsTrueValue(string value)
        {
            var val = value.ToLower();
            return val == "1" || value == "true" || value == "si" || value == "t";
        }
        public virtual bool IsFalseValue(string value)
        {
            var val = value.ToLower();
            return val == "0" || value == "false" || value == "no" || value == "f";
        }

        #region Parametros
        
        public virtual string GetParameter(string nombre)
        {
            var param = Parametros.FirstOrDefault(p => p.Nombre.ToLower() == nombre);
            return param != null ? param.Valor : null;
        }
        public virtual string Email { get { return GetParameter(Params.Email); } }
        public virtual string HomePage { get { return GetParameter(Params.Homepage); } }
        public virtual string ExcelFolder { get { return GetParameter(Params.ExcelFolder); } }
        public virtual char CsvSeparator
        {
            get
            {
                var csvSeparator = GetParameter(Params.RegionalListSeparator);
                if (csvSeparator == null || string.IsNullOrEmpty(csvSeparator)) return ';';
                return csvSeparator == "\t" ? '\t' : csvSeparator[0];
            }
        }
        public virtual bool MenuStartCollapsed
        {
            get
            {
                var menuStartCollapsedParam = GetParameter(Params.MenuStarCollapsed);
                return menuStartCollapsedParam != null && IsTrueValue(menuStartCollapsedParam);
            }
        }
        public virtual bool ShowDriverName
        {
            get
            {
                var showDriverName = GetParameter(Params.MonitorShowDriverName);
                return showDriverName != null && IsTrueValue(showDriverName);
            }
        }
        public virtual bool Fantasia
        {
            get
            {
                var fantasia = GetParameter(Params.Fantasia);
                return fantasia != null && IsTrueValue(fantasia);
            }
        }

        #endregion
    }
}