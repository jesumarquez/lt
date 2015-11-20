using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Web.Models
{
    public class DistritoModel
    {
        public int DistritoId { get; set; }
        public string RazonSocial { get; set; }
        public string Fantasia { get; set; }
    }

    public class EmpresaMapper : EntityModelMapper<Empresa, DistritoModel>
    {
        public EmpresaMapper()
        {
            var authUser = WebSecurity.AuthenticatedUser;
            if (authUser !=null)
                UseFantasia =  authUser.Fantasia;
        }

        private bool UseFantasia { get; set; }

        public override DistritoModel EntityToModel(Empresa entity, DistritoModel model)
        {
            model.DistritoId = entity.Id;
            model.RazonSocial = entity.RazonSocial;
            model.Fantasia = entity.Fantasia;
            return model;
        }

        public override Empresa ModelToEntity(DistritoModel model, Empresa entity)
        {
            if (model.DistritoId != entity.Id) throw new InvalidOperationException("Invalid Model(ID) <> Entity(ID)");
            entity.RazonSocial = model.RazonSocial;
            entity.Fantasia = model.Fantasia;
            return entity;
        }

        public override ItemModel ToItem(Empresa empresa)
        {
            return new ItemModel { Key = empresa.Id,Value = UseFantasia ? empresa.Fantasia: empresa.RazonSocial };
        }
    }
}