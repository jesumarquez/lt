#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.ValueObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionActualizacionParametrosDispositivosLista : SecuredListPage<TipoParametroDispositivoVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PAR_ACTUALIZACION_PARAMETROS"; } }
        protected override string RedirectUrl { get { return "ActualizacionParametrosDispositivosAlta.aspx"; } }
        protected override string GetRefference() { return "PAR_ACTUALIZACION_PARAMETROS"; }

        #endregion

        #region Protected Methods

        protected override List<TipoParametroDispositivoVo> GetListData()
        {
            return (from TipoParametroDispositivo parametro in DAOFactory.TipoParametroDispositivoDAO.FindByTipoDispositivo(ddlTipoDispositivo.Selected)
                    where parametro.Editable
                    select new TipoParametroDispositivoVo(parametro)).ToList();
        }
        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticTipoDispositivo, ddlTipoDispositivo);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticTipoDispositivo, ddlTipoDispositivo.Selected);
            return data;
        }
    }
}