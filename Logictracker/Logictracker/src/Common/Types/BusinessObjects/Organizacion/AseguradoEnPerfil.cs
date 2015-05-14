using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Organizacion
{
    [Serializable]
    public class AseguradoEnPerfil : IAuditable
    {
        public virtual Type TypeOf() { return GetType(); }

        public virtual int Id { get; set; }

        public virtual Perfil Perfil { get; set; }

        public virtual Asegurable Asegurable { get; set; }

        public virtual bool Permitido { get; set; }
    }
}
