using Logictracker.Types.BusinessObjects;

namespace Logictracker.Web.Models
{
    public abstract class EntityModelMapper<TEntity, TModel>
    {
        public abstract TModel EntityToModel(TEntity entity, TModel model);
        public abstract TEntity ModelToEntity(TModel model, TEntity entity);
        public abstract ItemModel ToItem(TEntity entity);
    }
}