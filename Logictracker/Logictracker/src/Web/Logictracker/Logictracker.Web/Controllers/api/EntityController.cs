using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Kendo.Mvc;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Web.Filters;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    [MvcNhibernateFilter]
    public abstract class EntityController<TEntity, TDao, TModel, TEntityMapper> : ApiController
        where TDao : GenericDAO<TEntity>
        where TEntity : IAuditable
        where TEntityMapper : EntityModelMapper<TEntity, TModel>, new()
        where TModel : new()
    {
        private TDao _dao;
        private TEntityMapper _mapper;
        protected TDao EntityDao { get { return _dao ?? (_dao = DAOFactory.GetDao<TDao>()); } }

        protected TEntityMapper Mapper { get { return _mapper ?? (_mapper = new TEntityMapper()); } }

        [HttpGet]
        public virtual TModel Get(int id)
        {
            return new TEntityMapper().EntityToModel(EntityDao.FindById(id), new TModel());
        }


        public static object GetFilterValue(IList<IFilterDescriptor> filters, string fieldName)
        {
            if (!filters.Any()) return null;
            foreach (var filter in filters)
            {
                var descriptor = filter as FilterDescriptor;
                if (descriptor != null && descriptor.Member == fieldName)
                {
                    return descriptor.Value;
                }
                var filterDescriptor = filter as CompositeFilterDescriptor;
                if (filterDescriptor != null)
                {
                    return GetFilterValue(filterDescriptor.FilterDescriptors, fieldName);
                }
            }
            return null;
        }
    }
}