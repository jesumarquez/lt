using Logictracker.DAL.DAO.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Logictracker.Web.Controllers.api
{
    public class TipoMensajeController : EntityController<TipoMensaje, TipoMensajeDAO, TipoMensajeModel, TipoMensajeMapper>
    {
        [Route("api/distrito/{distritoId}/base/{baseId}/tipomensaje/items")]
        public IEnumerable<ItemModel> GetComboItem(int distritoId, int baseId)
        {
            var list = ItemModel.All.ToList();
            var empresa = (new DAL.DAO.BusinessObjects.EmpresaDAO()).FindById(distritoId);
            var linea = (new DAL.DAO.BusinessObjects.LineaDAO()).FindById(baseId);
            list.AddRange(
                EntityDao.FindByEmpresaLineaYUsuario(empresa, linea, Usuario)
                    .Cast<TipoMensaje>()
                    .OrderBy(m => m.Descripcion)
                    .Select(t => Mapper.ToItem(t))
                );

            return list;
        }
    }
}