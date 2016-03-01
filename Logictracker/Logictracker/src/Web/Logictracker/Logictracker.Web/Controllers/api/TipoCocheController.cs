﻿using System.Collections.Generic;
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
        public IEnumerable<ItemModel> GetComboItem(int distritoId, int baseId)
        {
            var list = ItemModel.All.ToList();

            list.AddRange(
                EntityDao.FindByEmpresasAndLineas(
                    new List<int> { distritoId },
                    new List<int> { baseId },
                    Usuario)
                    .Select(t => Mapper.ToItem(t))
                );

            return list;
        }
    }
}