using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    public class DepartamentoController : EntityController<Departamento, DepartamentoDAO, DepartamentoModel, DepartamentoMapper>
    {

        [Route("api/distrito/{distritoId}/base/{baseId}/departamentos/items")]
        public IEnumerable<ItemModel> GetComboItem(int distritoId, int baseId)
        {
            return EntityDao.GetList(new[] {distritoId}, new[] {baseId}).Select(d => Mapper.ToItem(d));
        }
        
        [Route("api/departamento/items")]
        public override IEnumerable<ItemModel> GetComboItem()
        {
            return EntityDao.GetList(new int[]{}, new int[] {}).Select(d => Mapper.ToItem(d));
        }
    }
}
