﻿using System.Collections.Generic;
using System.Linq;
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
            var l = new List<ItemModel>();
            l.Add(Mapper.ToItem(EntityDao.GetById(distritoId, baseId, empleadoId)));
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
            var tp = DAOFactory.GetDao<TipoEmpleadoDAO>().FindByCodigo(distritoId, baseId, tipoEmpleadoCodigo);
            if(tp != null)
                return EntityDao.GetList(new int[] { distritoId }, new int[] { baseId }, new int[] { tp.Id}, new int[] { }).Select(e => Mapper.ToItem(e)).ToList();
            
            return new List<ItemModel>();            
        }
    }
}