using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web.Http;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.Models;
using Logictracker.DAL.Factories;

namespace Logictracker.Web.Controllers.api
{
    public class EmpleadoController : EntityController<Empleado, EmpleadoDAO, EmpleadoModel, EmpleadoMapper>
    {
        [Route("api/Distrito/{distritoId}/base/{baseId}/empleado/{empleadoId}/item")]
        public IEnumerable<ItemModel> GetEmpleado(int distritoId, int baseId, int empleadoId)
        {
            var l = new List<ItemModel> {Mapper.ToItem(EntityDao.GetById(distritoId, baseId, empleadoId))};
            return l;            
        }

        [Route("api/Distrito/{distritoId}/base/{baseId}/empleado/{id}/reporta/items")]
        public IEnumerable<ItemModel> GetReporta(int distritoId, int baseId, int id)
        {
            return EntityDao.GetReporta(distritoId, baseId, id).Select(e => Mapper.ToItem(e)).ToList();
        }

        [Route("api/Distrito/{distritoId}/base/{baseId}/tipoEmpleadoCodigo/{tipoEmpleadoCodigo}/items")]
        public IEnumerable<ItemModel> GetEmpleados(int distritoId, int baseId, string tipoEmpleadoCodigo)
        {
            return
                EntityDao.GetByCodigoTipoEmpleado(new[] {distritoId}, new[] {baseId}, tipoEmpleadoCodigo)
                    .Select(e => Mapper.ToItem(e))
                    .ToList();
            //var tp = DAOFactory.GetDao<TipoEmpleadoDAO>().FindByCodigo(distritoId, baseId, tipoEmpleadoCodigo);
            //return tp != null ? EntityDao.GetList(new[] { distritoId }, new[] { baseId }, new[] { tp.Id}, new int[] { }).Select(e => Mapper.ToItem(e)).ToList() : new List<ItemModel>();
        }
    }
}