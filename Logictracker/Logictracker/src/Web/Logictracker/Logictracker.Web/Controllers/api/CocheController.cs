﻿using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.Controllers.api.Models;
using Logictracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Logictracker.Web.Controllers.api
{
    public class CocheController : EntityController<Coche, CocheDAO, CocheModel, CocheMapper>
    {
        [Route("api/distrito/{distritoId}/base/{baseId}/coche/items")]
        public IEnumerable<ItemModel> GetComboItem(int distritoId, int baseId, bool excludeNone = false)
        {
            var list = ItemModel.None.ToList();

            if (excludeNone)
            {
                list = new List<ItemModel>();
            }

            list.AddRange(
                EntityDao.GetList(new[] { distritoId }, new[] { baseId })
                .Select(t => Mapper.ToItem(t)));

            return list;
        }
    }
}
