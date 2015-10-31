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

        [Route("api/centrodecostos/items")]
        public override IEnumerable<ItemModel> GetComboItem()
        {
            return EntityDao.GetList(new int[] {}, new int[] {}, new int[] {}).Select(c => Mapper.ToItem(c));
        }
    }
}
