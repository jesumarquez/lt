using Logictracker.DAL.DAO.ReportObjects;
using Logictracker.Types.ReportObjects;
using Logictracker.Web.Controllers.api;
using Logictracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Web.Http.ModelBinding;

namespace Logictracker.Web.Controllers.api
{
    public class EventoController : ReportController<MobileEvent, EventoModel, EventoMapper>
    {
        [Route("api/distrito/{distritoId}/base/{baseId}/evento/datasource")]
        public DataSourceResult GetDataSource([ModelBinder(typeof(EventoModel))] DataSourceRequest request, int distritoId, int baseId)
        {
            var dao = EntityDao as MobileEventDAO;

            return dao.GetMobilesEventsByDistritoBase(distritoId, baseId)
                .ToDataSourceResult(request, m => Mapper.EntityToModel(m, new EventoModel()));
        }

        protected override ReportDAO EntityDao
        {
            get
            {
                return new MobileEventDAO(new DAOFactory());
            }
        }
    }
}
