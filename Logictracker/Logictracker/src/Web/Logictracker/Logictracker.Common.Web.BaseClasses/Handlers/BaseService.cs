using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Logictracker.Culture;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Organizacion;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Web.BaseClasses.Handlers
{
    public class BaseService
    {
        private DAOFactory _daof;
        private ReportFactory _reportf;

        public DAOFactory DAOFactory { get { return _daof ?? (_daof = new DAOFactory()); } }
        public ReportFactory ReportFactory { get { return _reportf ?? (_reportf = new ReportFactory(DAOFactory)); } }

        public Usuario Usuario { get; set; }
        public HttpContext Context { get { return HttpContext.Current; } }

        public Respuesta<string> Login(string username, string password)
        {
            try
            {
                var clientIp = Context.Request.UserHostAddress;

                if (string.IsNullOrEmpty(username)) throw new ApplicationException(CultureManager.GetError("NO_USERNAME"));
                if (string.IsNullOrEmpty(password)) throw new ApplicationException(CultureManager.GetError("NO_PASSWORD"));

                var user = DAOFactory.UsuarioDAO.FindForLogin(username, password);

                if (user == null) throw new ApplicationException(CultureManager.GetError("WRONG_USER_PASS"));
                //if (user.Tipo == 0) throw new ApplicationException(CultureManager.GetError("NO_ACCESS"));
                if (user.Inhabilitado || (user.FechaExpiracion != null && user.FechaExpiracion < DateTime.UtcNow))
                {
                    user.Inhabilitado = true;
                    user.FechaExpiracion = null;
                    DAOFactory.UsuarioDAO.SaveOrUpdate(user);
                    throw new ApplicationException(CultureManager.GetError("USER_DISABLED"));
                }
                if (!user.IsInIpRange(clientIp))
                {
                    throw new ApplicationException("No puede acceder al sistema desde esta ubicacion");
                }

                Usuario = user;

                var sessionId = Guid.NewGuid().ToString();

                var loginInfo = new LoginInfo
                {
                    ClientIp = clientIp,
                    SessionId = sessionId,
                    Expiration = DateTime.UtcNow.AddMinutes(20),
                    UserId = user.Id
                };

                Context.Application[sessionId] = loginInfo;

                return Respuesta<string>.Create(sessionId);
            }
            catch (Exception ex)
            {
                return Respuesta<string>.CreateError(ex.Message);
            }
        }

        public Respuesta<bool> IsActive(string sessionId)
        {
            try
            {
                ValidateLoginInfo(sessionId);

                return Respuesta<bool>.Create(true);
            }
            catch (Exception ex)
            {
                var r = Respuesta<bool>.Create(false);
                r.Mensaje = ex.Message;
                return r;
            }
        }

        public Respuesta<bool> Logout(string sessionId)
        {
            try
            {
                var clientIp = Context.Request.UserHostAddress;

                var loginInfo = GetLoginInfo(sessionId);

                if (loginInfo == null) throw new ApplicationException("El id de sesion no es válido.");

                if (loginInfo.ClientIp != clientIp) throw new ApplicationException("La direccion IP no es la misma que hizo el login. " + loginInfo.ClientIp + " == " + clientIp);

                Context.Application[sessionId] = null;

                Usuario = null;

                return Respuesta<bool>.Create(true);
            }
            catch (Exception ex)
            {
                return Respuesta<bool>.CreateError(ex.Message);
            }
        }

        public LoginInfo GetLoginInfo(string sessionId)
        {
            return Context.Application[sessionId] as LoginInfo;
        }

        public LoginInfo ValidateLoginInfo(string sessionId, string securable)
        {
            var loginInfo = ValidateLoginInfo(sessionId);
            if (!WebSecurity.IsSecuredAllowed(securable)) throw new ApplicationException("El usuario actual no tiene permisos para acceder a este servicio.");
            return loginInfo;
        }
        public LoginInfo ValidateLoginInfo(string sessionId)
        {
            var loginInfo = GetLoginInfo(sessionId);

            if (loginInfo == null) throw new ApplicationException("El id de sesion " + sessionId + " no es válido.");

            var clientIp = Context.Request.UserHostAddress;

            if (loginInfo.ClientIp != clientIp) throw new ApplicationException("La direccion IP no es la misma que hizo el login.");

            if (loginInfo.Expiration <= DateTime.UtcNow) throw new ApplicationException("Sesion expirada.");

            loginInfo.Expiration = DateTime.UtcNow.AddMinutes(20);

            Context.Application[sessionId] = loginInfo;

            var usuario = DAOFactory.UsuarioDAO.FindById(loginInfo.UserId);
            IEnumerable<MovMenu> modules;
            IEnumerable<Asegurable> securables;
            IEnumerable<int> perfiles = DAOFactory.PerfilDAO.GetProfileAccess(usuario, -1, out modules, out securables);

            WebSecurity.Login(usuario, perfiles, modules, securables);

            Usuario = usuario;

            return loginInfo;
        }

        public void ValidateExpiration(string uniqueKey, int seconds)
        {
            var key = string.Concat("App_Services.", uniqueKey);
            var lastTime = Context.Application[key];

            if (lastTime != null)
            {
                var last = Convert.ToDateTime(lastTime);
                if (DateTime.UtcNow.Subtract(last) < TimeSpan.FromSeconds(seconds))
                    throw new ApplicationException("Ultima consulta: " + last.ToString("HH:mm:ss (UTC)"));
            }

            Context.Application[key] = DateTime.UtcNow;
        }

        public Empresa GetEmpresaByCode(string company)
        {
            //if (Usuario == null) throw new ApplicationException("No hay un usuario logueado");
            var empresa = DAOFactory.EmpresaDAO.GetList().FirstOrDefault(e => e.Codigo == company);
            if (empresa == null) throw new ApplicationException("Empresa " + company + " no encontrada");
            return empresa;
        }

        public Linea GetLineaByCode(string branch, Empresa company)
        {
            //if (Usuario == null) throw new ApplicationException("No hay un usuario logueado");

            var linea = DAOFactory.LineaDAO.GetList(new[] { company != null ? company.Id : -1 }).FirstOrDefault(l => l.DescripcionCorta == branch);

            if (linea == null) throw new ApplicationException("Linea " + branch + " no encontrada");
            //if (!linea.Interfaceable) throw new ApplicationException("La Linea " + branch + " no acepta consultas de esta interface");

            return linea;
        }

        public Coche GetCocheByInterno(Empresa empresa, Linea linea, string interno)
        {
            //if (Usuario == null) throw new ApplicationException("No hay un usuario logueado");
            var coche = DAOFactory.CocheDAO.GetList(new[] { empresa.Id }, new[] { linea != null ? linea.Id : -1 }).FirstOrDefault(c => c.Interno == interno);
            if (coche == null) throw new ApplicationException("No se encontro el vehiculo " + interno);
            return coche;
        }
        public Coche GetCocheByPatente(Empresa empresa, Linea linea, string patente)
        {
            //if (Usuario == null) throw new ApplicationException("No hay un usuario logueado");
            var coche = DAOFactory.CocheDAO.GetList(new[] { empresa.Id }, new[] { linea != null ? linea.Id : -1 }).FirstOrDefault(c => c.Patente == patente);
            if (coche == null) throw new ApplicationException("No se encontro el vehiculo con patente " + patente);
            return coche;
        }
        public Coche GetCocheByPatente(int empresa, string patente)
        {
            var vehiculo = DAOFactory.CocheDAO.FindByPatente(empresa, patente);
            if (vehiculo == null) throw new ApplicationException("No se encontraro el vehiculo con patente " + patente);
            return vehiculo;
        }
        public Cliente GetClienteByCode(string codigo)
        {
            var cliente = DAOFactory.ClienteDAO.GetByCode(new[] { -1 }, new[] { -1 }, codigo);
            if (cliente == null) throw new ApplicationException("No se encontro un cliente con código " + codigo);
            return cliente;
        }

        public PuntoEntrega GetPuntoEntregaByCode(string codigo, Cliente client)
        {
            var puntoEntrega = DAOFactory.PuntoEntregaDAO.GetByCode(new[] { -1 }, new[] { -1 }, new[] { client.Id }, codigo);
            if (puntoEntrega == null) throw new ApplicationException("No se encontro un punto de entrega con código " + codigo);
            return puntoEntrega;
        }

        public Posicion GetPosicion(Coche coche)
        {
            var pos = DAOFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(coche);
            if (pos == null) return null;
            return new Posicion
            {
                Fecha = pos.FechaMensaje,
                Interno = coche.Interno,
                Patente = coche.Patente,
                Latitud = pos.Latitud,
                Longitud = pos.Longitud,
                Velocidad = pos.Velocidad,
                Altitud = pos.Altitud,
                Curso = pos.Curso
            };

        }
    }

    [Serializable]
    public class Respuesta<T>
    {
        public T Resultado { get; set; }
        public bool RespuestaOk { get; set; }
        public string Mensaje { get; set; }

        public static Respuesta<T> CreateError(string mensaje)
        {
            return new Respuesta<T> { RespuestaOk = false, Mensaje = mensaje };
        }
        public static Respuesta<T> Create(T resultado)
        {
            return Create(resultado, string.Empty);
        }
        public static Respuesta<T> Create(T resultado, string mensaje)
        {
            return new Respuesta<T> { RespuestaOk = true, Resultado = resultado, Mensaje = mensaje };
        }
    }

    public class LoginInfo
    {
        public string ClientIp { get; set; }
        public string SessionId { get; set; }
        public DateTime Expiration { get; set; }
        public int UserId { get; set; }
    }

    [Serializable]
    public class Posicion
    {
        public string Interno { get; set; }
        public string Patente { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public int Velocidad { get; set; }
        public DateTime Fecha { get; set; }
        public double Altitud { get; set; }
        public double Curso { get; set; }
    }
    [Serializable]
    public class Distancia
    {
        public string Interno { get; set; }
        public string Esquina { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public double Metros { get; set; }
    }
}
