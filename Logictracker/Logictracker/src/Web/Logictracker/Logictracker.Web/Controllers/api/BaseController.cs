using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    public class BaseController : EntityController<Linea, LineaDAO, BaseModel, LineaMapper>
    {

        [Route("api/Distrito/{distritoId}/base/items")]
        public IEnumerable<ItemModel> GetBases(int distritoId)
        {
            var mapper = new LineaMapper();
            return EntityDao.GetLineasPermitidasPorUsuario(new []{distritoId}).Select(l=>mapper.ToItem(l));
        }

    }

}