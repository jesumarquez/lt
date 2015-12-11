using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.Models;
using System.Web.Http;

namespace Logictracker.Web.Controllers.api
{
    public class TransportistaController : EntityController<Transportista,TransportistaDAO,TransportistaModel,TransportistaMapper>
    {
        [Route("api/distrito/{distritoId}/base/{baseId}/transportista/items")]
        public IEnumerable<ItemModel> GetComboItem(int distritoId, int baseId)
        {
            return EntityDao.GetTransportistasPermitidosPorUsuario(new[] { distritoId }, new[] { baseId }).Select(d => Mapper.ToItem(d)).ToList();
        }
    }

    public class TransportistaModel
    {
    }

    public class TransportistaMapper : EntityModelMapper<Transportista,TransportistaModel>
    {
        public override TransportistaModel EntityToModel(Transportista entity, TransportistaModel model)
        {
            throw new NotImplementedException();
        }

        public override Transportista ModelToEntity(TransportistaModel model, Transportista entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(Transportista entity)
        {
            return new ItemModel {Key = entity.Id, Value = entity.Descripcion};
        }
    }
}
