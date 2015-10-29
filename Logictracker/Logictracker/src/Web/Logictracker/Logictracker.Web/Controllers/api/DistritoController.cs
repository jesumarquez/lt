using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    public class DistritoController :EntityController<Empresa,EmpresaDAO,DistritoModel,EmpresaMapper>
    {
        [Route("api/Distrito/Items")]
        public override IEnumerable<ItemModel> GetComboItem()
        {
            var mapper = new EmpresaMapper();
            return EntityDao.GetEmpresasPermitidas().Select(e=>mapper.ToItem(e));
        }

      
    }

}
