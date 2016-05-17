using System;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;

namespace Logictracker.Web.Controllers.api
{

    public class UserDataModel
    {
        const string distritoKey = "LocacionDropDownList";
        const string baseKey = "PlantaDropDownList";
            
        public int? DistritoSelected { get; set; }
        public int? BaseSelected { get; set; }

        public int EmpleadoId { get; set; }

        public static UserDataModel Create(HttpSessionState session)
        {
            var rv = new UserDataModel
            {
                DistritoSelected = GetSessionInt(session, distritoKey),
                BaseSelected = GetSessionInt(session, baseKey),
                EmpleadoId = Security.WebSecurity.AuthenticatedUser.EmpleadoId
            };
            return rv;
        }

        private static int? GetSessionInt(HttpSessionState session, string varName)
        {
            var b = session[varName];
            return b != null ? (int?)Convert.ToInt32(b) : null;
        }
        
        public UserDataModel PersistNotNull()
        {
            if (DistritoSelected.HasValue) HttpContext.Current.Session[distritoKey] = DistritoSelected;
            if (BaseSelected.HasValue) HttpContext.Current.Session[baseKey]=BaseSelected;
            return this;
        }

        public UserDataModel PersistCleanNull()
        {
            if (!DistritoSelected.HasValue) HttpContext.Current.Session.Remove(distritoKey);
            if (!BaseSelected.HasValue) HttpContext.Current.Session.Remove(baseKey);
            return this;
        }

    }


    public class UserDataController : ApiController
    {
        [Route("api/UserData")]
        [HttpGet]
        public UserDataModel Get()
        {
            return UserDataModel.Create(HttpContext.Current.Session);
        }

        [Route("api/UserData")]
        [HttpPost]
        public UserDataModel Post(UserDataModel model)
        {
            model.PersistNotNull();
            return UserDataModel.Create(HttpContext.Current.Session);
        }

    }
}
