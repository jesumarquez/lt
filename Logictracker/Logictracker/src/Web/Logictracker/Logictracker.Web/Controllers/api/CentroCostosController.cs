using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    public class CentroCostosController : EntityController<CentroDeCostos, CentroDeCostosDAO, CentroDeCostosModel, CentroDeCostosMapper>
    {

        [Route("api/distrito/{distritoId}/base/{baseId}/centrodecostos/items")]
        public IEnumerable<ItemModel> GetComboItem(int distritoId,int baseId,[FromUri]int[] deptoId)
        {
            return EntityDao.GetList(new[] { distritoId }, new[] { baseId }, deptoId).Select(c => Mapper.ToItem(c)).ToList();
        }

    }
}
