using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Logictracker.DAL.Factories;
using Logictracker.Security;

namespace LogicTracker.App.Web.Api.Filters
{
    public class SecureResourceAttribute : AuthorizationFilterAttribute
    {
        public static DAOFactory DaoFactory { get; set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authorizeHeader = actionContext.Request.Headers.Authorization;
            if (authorizeHeader != null
                && authorizeHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase)
                && String.IsNullOrEmpty(authorizeHeader.Parameter) == false)
            {
                var encoding = Encoding.GetEncoding("ISO-8859-1");
                var credintials = encoding.GetString(
                                   Convert.FromBase64String(authorizeHeader.Parameter));
                string username = credintials.Split(':')[0];
                string password = credintials.Split(':')[1];
                string roleOfUser = string.Empty;

                DaoFactory = new DAOFactory();
                var usuario = DaoFactory.UsuarioDAO.FindForLogin(username, password);
                WebSecurity.ValidateLogin(usuario);

                if (usuario != null)
                {
                    var principal = new GenericPrincipal((new GenericIdentity(username)),
                                                                (new[] { roleOfUser }));
                    Thread.CurrentPrincipal = principal;
                    return;
                }
            }
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);

            actionContext.Response.Content = new StringContent("Username and password are missings or invalid");
        }
    } 
}