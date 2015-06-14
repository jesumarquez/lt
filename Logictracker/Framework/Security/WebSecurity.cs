#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;
using Logictracker.Security.Exceptions;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Organizacion;
using Logictracker.Types.SecurityObjects;
using Module = Logictracker.Types.SecurityObjects.Module;

#endregion

namespace Logictracker.Security
{
    public static class WebSecurity
    {
        private const string AuthUserSessionVar = "AuthUser";

        /// <summary>
        /// Determines if there is a logged in user.
        /// </summary>
        /// <returns></returns>
        public static bool Authenticated { get { return AuthenticatedUser != null; } }

        /// <summary>
        /// Gets the logged in user.
        /// </summary>
        /// <returns></returns>
        public static UserSessionData AuthenticatedUser
        {
            get
            {
                return HttpContext.Current != null ? GetAuthenticatedUser(HttpContext.Current.Session) : null;
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    var request = new SimpleWorkerRequest("/dummy", @"c:\inetpub\wwwroot\dummy", "dummy.html", null, new StringWriter());
                    HttpContext.Current = new HttpContext(request);
                }
                if (HttpContext.Current.Session == null)
                {
                    SessionStateUtility.AddHttpSessionStateToContext(HttpContext.Current, new HttpSessionState2());
                }
                if (HttpContext.Current.Session != null) HttpContext.Current.Session[AuthUserSessionVar] = value;
            }
        }

        /// <summary>
        /// Gets the logged in user on an httpsession.
        /// </summary>
        /// <returns></returns>
        public static UserSessionData GetAuthenticatedUser(HttpSessionState session)
        {
            return session != null && session[AuthUserSessionVar] != null ? (UserSessionData)session[AuthUserSessionVar] : null;
        }

        /// <summary>
        /// Returns the user previlegies for the givenn module or null in case the user is not logged in.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public static Module GetUserModuleByRef(string module)
        {
            return Authenticated ? AuthenticatedUser.GetModuleByRef(module) : null;
        }

        public static bool ShowDriver
        {
            get
            {
                var user = AuthenticatedUser;
                return user != null && user.ShowDriverName;
            }
        }

        public static bool IsSecuredAllowed(string securable)
        {
            return string.IsNullOrEmpty(securable) || (AuthenticatedUser != null && AuthenticatedUser.IsSecuredAllowed(securable));
        }

        public static void Login(Usuario usuario, IEnumerable<int> perfiles, IEnumerable<Module> modules, IEnumerable<Asegurable> asegurables)
        {
            var user = new UserSessionData(usuario, perfiles.ToList());

            user.SetModules(modules);
            user.SetSecurables(asegurables);

            AuthenticatedUser = user;
        }

        public static void ValidateLogin(Usuario usuario)
        {
            if (usuario == null) throw new WrongUserPassException();
            if (usuario.Tipo == 0) throw new NoAccessException();
            if (usuario.Inhabilitado || (usuario.FechaExpiracion != null && usuario.FechaExpiracion < DateTime.UtcNow)) throw new UserDisabledException();
            if (!usuario.IsInIpRange(HttpContext.Current.Request.UserHostAddress)) throw new RestrictedIpException();

            if (usuario.PorEmpresa && usuario.Empresas.Count == 1)
            {
                var empresa = usuario.Empresas.Cast<Empresa>().First();
                var pk = empresa.ProductKey;
                if (pk != string.Empty)
                {
                    if (pk.Length != 16) throw new ExpiredProductException();
                    
                    var m = pk.Substring(3, 2);
                    var a = pk.Substring(9, 2);
                    int mes, anio;
                    if (!int.TryParse(m, out mes) || !int.TryParse(a, out anio)) throw new ExpiredProductException();
                    if (mes < 1 || mes > 12 || anio < 1) throw new ExpiredProductException();

                    var dt = new DateTime(2000 + anio, mes, 1);
                    if (dt < DateTime.Today) throw new ExpiredProductException();
                }
            }
        }

        public static void Logout()
        {
            if (HttpContext.Current == null) return;
            if (HttpContext.Current.Session != null) HttpContext.Current.Session.Abandon();
        }

		public static void UpdateUser(Usuario usuario)
        {
            if (!Authenticated) return;
            if (AuthenticatedUser.Id != usuario.Id) return;

            AuthenticatedUser.Update(usuario);
        }

        public class HttpSessionState2 : IHttpSessionState
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            #region Implementation of IHttpSessionState

            public void Abandon() { Clear(); }

            public void Add(string name, object value)
            {
                Remove(name);
                dic.Add(name, value);
            }

            public void Remove(string name)
            {
                if (dic.ContainsKey(name)) dic.Remove(name);
            }

            public void RemoveAt(int index)
            {
                Remove(dic.Keys.ElementAt(index));
            }

            public void Clear() { dic.Clear(); }

            public void RemoveAll() { Clear(); }

            public IEnumerator GetEnumerator() { return dic.GetEnumerator(); }

            public void CopyTo(Array array, int index) { throw new NotImplementedException(); }

            public string SessionID { get { return "1234"; } }

            public int Timeout { get; set; }

            public bool IsNewSession { get { return false; } }

            public SessionStateMode Mode { get { return SessionStateMode.Custom; } }

            public bool IsCookieless { get { return true; } }

            public HttpCookieMode CookieMode { get { return HttpCookieMode.AutoDetect; } }

            public int LCID { get; set; }

            public int CodePage { get; set; }

            public HttpStaticObjectsCollection StaticObjects { get { return new HttpStaticObjectsCollection(); } }

            object IHttpSessionState.this[string name]
            {
                get { return dic.ContainsKey(name) ? dic[name] : null; }
                set { Add(name, value); }
            }

            object IHttpSessionState.this[int index]
            {
                get { return dic[dic.Keys.ElementAt(index)]; }
                set { Add(dic.Keys.ElementAt(index), value); }
            }

            public int Count { get { return dic.Count; } }

            public NameObjectCollectionBase.KeysCollection Keys { get { return null; } }

            public object SyncRoot { get; set; }

            public bool IsReadOnly { get { return false; } }

            public bool IsSynchronized { get { return false; } }

            #endregion
        }
    }
}