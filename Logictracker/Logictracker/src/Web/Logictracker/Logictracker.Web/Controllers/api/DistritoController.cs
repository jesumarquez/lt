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
        [Route("api/Distrito/items")]
        public IEnumerable<ItemModel> GetComboItem()
        {
            return EntityDao.GetEmpresasPermitidas().Select(e=>Mapper.ToItem(e));
        }

        [Route("api/Distrito/{distritoId}/parametros/items")]
        public IEnumerable<KeyValuePair<string, string>> GetParametros(int distritoId)
        {
            return EntityDao.GetById(distritoId).Parametros.ToList().Select(p => new KeyValuePair<string, string>(p.Nombre, p.Valor));
        }
    }
}
