using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Organizacion;

namespace Logictracker.Types.SecurityObjects
{
    /// <summary>
    /// Summary description for UserSessionData
    /// </summary>
    [Serializable]
    public class UserSessionData
    {
        #region Private Properties

        private readonly Dictionary<string, Module> _modules;
        private readonly Dictionary<string, bool> _securables;

        #endregion

        #region Constructors

        /// <summary>
        /// Generates usefull information of the current user to be stored on session for future use.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="perfIds"></param>
        public UserSessionData(Usuario user, List<int> perfIds)
        {
            _modules = new Dictionary<string, Module>();
            _securables = new Dictionary<string, bool>();

            Id = user.Id;
            
            Update(user);

            IdPerfiles = perfIds;
        }

        public void Update(Usuario user)
        {
            Name = user.NombreUsuario;
            AccessLevel = user.Tipo;
            Theme = user.Theme;
            Logo = user.Logo;
            MenuStartCollapsed = user.MenuStartCollapsed;
            CsvSeparator = user.CsvSeparator;
            ShowDriverName = user.ShowDriverName;
            Client = user.Client;
            Culture = !string.IsNullOrEmpty(user.Culture) ? CultureInfo.GetCultureInfo(user.Culture) : Thread.CurrentThread.CurrentCulture;
            ExcelFolder = user.ExcelFolder;
            Fantasia = user.Fantasia;

            try
            {
                GmtModifier = !string.IsNullOrEmpty(user.TimeZoneId) ? TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneId).BaseUtcOffset.TotalHours : 0;
            }
            catch
            {
                GmtModifier = 0;
                //Si no existe el TimeZoneId Explota t_odo (por ejemplo cuando se instala en otro windows)
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// User Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Perfil ID
        /// </summary>
        public List<int> IdPerfiles { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// User selected Theme
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// User Selected Logo
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// The current user culture.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Modules Enabled for Current Role
        /// </summary>
        public IDictionary<string, Module> Modules { get { return _modules; } }

        public IDictionary<string, bool> Securables { get { return _securables; } }

        /// <summary>
        /// Determines the security access level allowed for this user.
        /// </summary>
        public short AccessLevel { get; set; }

        /// <summary>
        /// Determines the time modifier to be applyied for displaying datetime values for the current user.
        /// </summary>
        public double GmtModifier { get; set; }

        /// <summary>
        /// The client assigned to the current user.
        /// </summary>
        public string Client { get; set; }

        public bool MenuStartCollapsed { get; set; }

        public char CsvSeparator { get; set; }

        public bool ShowDriverName { get; set; }

        public string ExcelFolder { get; set; }

        public bool Fantasia { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the Module list
        /// </summary>
        /// <param name="items"></param>
        public void SetModules(IEnumerable items)
        {
            _modules.Clear();
            foreach (Module m in items)
            {
                var key = m.RefName.Trim();
                if (!_modules.ContainsKey(key))
                {
                    _modules.Add(key, m);
                }
            }
            //foreach (var mod in items.Cast<MovMenu>().Select(item => new Module(item)).Where(mod => !_modules.ContainsKey(mod.RefName.Trim()))) _modules.Add(mod.RefName.Trim(), mod);
        }

        public void SetSecurables(IEnumerable<Asegurable> items)
        {
            _securables.Clear();
            items.ToList().ForEach( i=> _securables.Add(i.Referencia,true));
        }

        /// <summary>
        /// Gets a Module by its Reference string
        /// </summary>
        /// <param name="refname">Reference string</param>
        /// <returns>The Module with the given Reference string or null if not found</returns>
        public Module GetModuleByRef(string refname)
        {
            var refs = refname.Split(',');

            return refs.Length.Equals(0) ? null : (from refference in refs where _modules.ContainsKey(refference.Trim()) select _modules[refference.Trim()]).FirstOrDefault();
        }

        public bool IsSecuredAllowed(string securable)
        {
            return Securables.ContainsKey(securable) && Securables[securable];
        }

        #endregion
    }
}