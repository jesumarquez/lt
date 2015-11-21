using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    public class EmpleadoController : EntityController<Empleado, EmpleadoDAO, EmpleadoModel, EmpleadoMapper>
    {
        [Route("api/Distrito/{distritoId}/base/{baseId}/empleado/{legajo}/item")]
        public EmpleadoModel GetEmpleado(int distritoId, int baseId, string legajo)
        {
            return Mapper.EntityToModel(EntityDao.GetByLegajo(distritoId, baseId, legajo), new EmpleadoModel());            
        }

        [Route("api/Distrito/{distritoId}/base/{baseId}/empleado/{id}/reporta/items")]
        public IEnumerable<ItemModel> GetReporta(int distritoId, int baseId, int id)
        {
            return EntityDao.GetReporta(distritoId, baseId, id).Select(e => Mapper.ToItem(e)).ToList();
        }
    }
}