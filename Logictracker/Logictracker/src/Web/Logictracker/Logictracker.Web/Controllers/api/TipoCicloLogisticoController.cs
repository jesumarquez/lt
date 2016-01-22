using Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Logictracker.Web.Controllers.api
{
    public class TipoCicloLogisticoController : EntityController<TipoCicloLogistico, TipoCicloLogisticoDAO, TipoCicloLogisticoModel, TipoCicloLogisticoMapper>
    {

        [Route("api/distrito/{distritoId}/tipociclologistico/items")]
        public IEnumerable<ItemModel> GetComboItem(int distritoId)
        {
            return EntityDao.GetByEmpresa(distritoId).Select(t => Mapper.ToItem(t)).ToList();
        }
    }
}
