using System.Collections.Generic;
using System.Web.Http;
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
        public abstract IEnumerable<ItemModel> GetComboItem();

        private TDao _dao;
        protected TDao EntityDao { get { return _dao ?? (_dao = DAOFactory.GetDao<TDao>()); } }


        [HttpGet]
        public virtual TModel Get(int id)
        {
            return new TEntityMapper().EntityToModel(EntityDao.FindById(id), new TModel());
        }

    }
}