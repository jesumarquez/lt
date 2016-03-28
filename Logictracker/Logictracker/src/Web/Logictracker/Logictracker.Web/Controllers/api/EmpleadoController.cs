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
        [Route("api/Distrito/{distritoId}/base/{baseId}/empleado/{empleadoId}/item")]
        public IEnumerable<ItemModel> GetEmpleado(int distritoId, int baseId, int empleadoId)
        {
            var empleado = EntityDao.GetById(distritoId, baseId, empleadoId);
            if (empleado == null) return Enumerable.Empty<ItemModel>();
            var l = new List<ItemModel> { Mapper.ToItem(empleado) };
            return l;
        }

        [Route("api/Distrito/{distritoId}/base/{baseId}/empleado/items")]
        public IEnumerable<ItemModel> GetEmpleados(int distritoId, int baseId)
        {
            var empleados = EntityDao.GetList(new List<int>{ distritoId}, new List<int> { baseId },new List<int>(), new List<int>());
            return empleados.Select(e =>  Mapper.ToItem(e)).OrderBy(e => e.Value).ToList();
        }

        [Route("api/Distrito/{distritoId}/base/{baseId}/empleado/{empleadoId}/model")]
        public IEnumerable<EmpleadoModel> GetEmpleadoModel(int distritoId, int baseId, int empleadoId)
        {
            var empleado = EntityDao.GetById(distritoId, -1, empleadoId);
            if (empleado == null) return Enumerable.Empty<EmpleadoModel>();
            var l = new List<EmpleadoModel> { Mapper.EntityToModel(empleado, new EmpleadoModel()) };
            return l;
        }


        [Route("api/Distrito/{distritoId}/base/{baseId}/empleado/{id}/reporta/models")]
        public IEnumerable<EmpleadoModel> GetReportaModel(int distritoId, int baseId, int id)
        {
            return EntityDao.GetReporta(distritoId, baseId, id).Select(e => Mapper.EntityToModel(e,new EmpleadoModel())).ToList();
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
                EntityDao.GetByCodigoTipoEmpleado(new[] { distritoId }, new[] { baseId }, tipoEmpleadoCodigo)
                    .Select(e => Mapper.ToItem(e))
                    .ToList();
            //var tp = DAOFactory.GetDao<TipoEmpleadoDAO>().FindByCodigo(distritoId, baseId, tipoEmpleadoCodigo);
            //return tp != null ? EntityDao.GetList(new[] { distritoId }, new[] { baseId }, new[] { tp.Id}, new int[] { }).Select(e => Mapper.ToItem(e)).ToList() : new List<ItemModel>();
        }


        [Route("api/Distrito/{distritoId}/base/{baseId}/tipoEmpleadoCodigo/{tipoEmpleadoCodigo}/models")]
        public IEnumerable<EmpleadoModel> GetEmpleadosModel(int distritoId, int baseId, string tipoEmpleadoCodigo)
        {
            return
                EntityDao.GetByCodigoTipoEmpleado(new[] { distritoId }, new[] { baseId }, tipoEmpleadoCodigo)
                    .Select(e => Mapper.EntityToModel(e,new EmpleadoModel()))
                    .ToList();
        }

    }
}