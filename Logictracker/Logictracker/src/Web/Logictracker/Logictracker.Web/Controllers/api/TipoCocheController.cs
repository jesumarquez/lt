using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    public class TipoCocheController : EntityController<TipoCoche, TipoCocheDAO, TipoCocheModel, TipoCocheMapper>
    {
        [Route("api/distrito/{distritoId}/base/{baseId}/tipocoche/items")]
        public IEnumerable<TipoCocheModel> GetComboItem(int distritoId, int baseId)
        {
            return new List<TipoCocheModel>(EntityDao.FindByEmpresasAndLineas(
                    new List<int> { distritoId },
                    new List<int> { baseId },
                    Usuario)
                    .Select(t => Mapper.EntityToModel(t, new TipoCocheModel())));
        }
    }
}