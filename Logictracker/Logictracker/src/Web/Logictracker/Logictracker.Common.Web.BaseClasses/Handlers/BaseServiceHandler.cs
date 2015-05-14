using System;
using System.Web;

namespace Logictracker.Web.BaseClasses.Handlers
{
    public abstract class BaseServiceHandler : IHttpHandler
    {
        protected BaseService BaseService { get; private set; }
        protected HttpContext Context { get; private set; }

        protected abstract string Securable { get; }

        protected BaseServiceHandler()
        {
            BaseService = new BaseService();
        }

        public void Login()
        {
            var r = BaseService.Login(Context.Request.Form["username"], Context.Request.Form["password"]);
            if (r.RespuestaOk)
            {
                WriteLine("<Respuesta>");
                WriteLine("<SessionId>{0}</SessionId>", r.Resultado);
                WriteLine("</Respuesta>");
            }
            else
            {
                WriteLine("<Error>");
                WriteLine("<Mensaje>{0}</Mensaje>", r.Mensaje);
                WriteLine("</Error>");
            }
        }
        public void IsActive()
        {
            var r = BaseService.IsActive(Context.Request.Form["sessionid"]);
            if (r.RespuestaOk)
            {
                WriteLine("<Respuesta>");
                WriteLine("<Active>{0}</Active>", r.Resultado ? "true" : "false");
                WriteLine("</Respuesta>");
            }
            else
            {
                WriteLine("<Error>");
                WriteLine("<Mensaje>{0}</Mensaje>", r.Mensaje);
                WriteLine("</Error>");
            }
        }

        public void Logout()
        {
            var r = BaseService.Logout(Context.Request.Form["sessionid"]);
            if (r.RespuestaOk)
            {
                WriteLine("<Respuesta>");
                WriteLine("<Logout>{0}</Logout>", r.Resultado ? "true" : "false");
                WriteLine("</Respuesta>");
            }
            else
            {
                WriteLine("<Error>");
                WriteLine("<Mensaje>{0}</Mensaje>", r.Mensaje);
                WriteLine("</Error>");
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                Context = context;
                Context.Response.ContentType = "text/xml";
                WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                var method = context.Request.Form["method"];
                if(method == null)
                {
                    return;
                }
                method = method.ToLower();

                switch(method)
                {
                    case "login": Login(); break;
                    case "isactive": IsActive(); break;
                    case "logout": Logout(); break;
                    default:
                        {
                            if (!string.IsNullOrEmpty(Securable))
                            {
                                BaseService.ValidateLoginInfo(Context.Request.Form["sessionid"], Securable);
                            }
                            ProcessRequest(method); break;
                        }
                }
            }
            catch(ApplicationException ex)
            {
                WriteLine("<Error>");
                WriteLine("<Mensaje>{0}</Mensaje>", ex.Message);
                WriteLine("</Error>");
            }
            catch (Exception ex)
            {
                WriteLine("<Error>");
                WriteLine("<Mensaje>{0}</Mensaje>", ex.ToString());
                WriteLine("</Error>");
            }
        }

        public abstract void ProcessRequest(string method);

        public bool IsReusable
        {
            get { return false; }
        }
        public void WriteLine(string text, params object[] parameters)
        {
            Context.Response.Write(string.Format(text + "\n", parameters));
        }
    }
}