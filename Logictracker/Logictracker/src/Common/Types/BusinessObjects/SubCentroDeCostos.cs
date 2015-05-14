using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class SubCentroDeCostos : IAuditable, ISecurable, IHasCentroDeCosto
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual CentroDeCostos CentroDeCostos { get; set; }

        public virtual string Descripcion { get; set; }
        public virtual string Codigo { get; set; }
        public virtual int Objetivo { get; set; }
        public virtual bool Baja { get; set; }

        #region Public Methods

        public override bool Equals(object obj)
        {
            var castObj = obj as CentroDeCostos;

            return castObj != null && castObj.Id.Equals(Id) && Id != 0;
        }

        public override int GetHashCode() { return Id.GetHashCode(); }

        #endregion
    }
}

