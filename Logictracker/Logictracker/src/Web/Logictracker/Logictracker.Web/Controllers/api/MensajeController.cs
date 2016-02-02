using Logictracker.DAL.DAO.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Logictracker.Web.Controllers.api
{
    public class MensajeController : EntityController<Mensaje, MensajeDAO, MensajeModel, MensajeMapper>
    {
        [Route("api/distrito/{distritoId}/base/{baseId}/mensaje/items")]
        public IEnumerable<ItemModel> GetComboItem(int distritoId, int baseId)
        {
            var empresa = (new DAL.DAO.BusinessObjects.EmpresaDAO()).FindById(distritoId);
            var linea = (new DAL.DAO.BusinessObjects.LineaDAO()).FindById(baseId);

            return EntityDao.FindByEmpresaYLineaAndUser(empresa, linea, Usuario)
                .Select(t => Mapper.ToItem(t)).OrderBy(m => m.Value).ToList();
        }
    }
}
