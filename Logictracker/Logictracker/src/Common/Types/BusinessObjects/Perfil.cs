using System;
using System.Collections.Generic;
using System.Linq;
using Iesi.Collections;
using Logictracker.Types.BusinessObjects.Organizacion;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Perfil : IAuditable, ISecurable
    {
        #region Private Properties

        private ISet _coches;
        private ISet _funciones;

        #endregion

        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion


        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        #endregion

        public virtual string Descripcion { get; set; }

        public virtual DateTime? FechaBaja { get; set; }

        public virtual bool PorCoche { get; set; }

        public virtual bool TipoPermiso { get; set; }

        public virtual ISet Coches { get { return _coches ?? (_coches = new ListSet()); } }

        public virtual ISet Funciones { get { return _funciones ?? (_funciones = new ListSet()); } }

        private IList<AseguradoEnPerfil> _asegurados;
        public virtual IList<AseguradoEnPerfil> Asegurados
        {
            get { return _asegurados ?? (_asegurados = new List<AseguradoEnPerfil>()); }
            set { _asegurados = value; }
        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if ((obj == null) || (obj.GetType() != GetType())) return false;
            var castObj = (Perfil) obj;
            return (Id == castObj.Id) && (Id != 0);
        }

        public override int GetHashCode() { return 27*57*Id.GetHashCode(); }

        public virtual void ClearCoches() { Coches.Clear(); }

        public virtual void AddCoche(Coche coche) { Coches.Add(coche); }

        public virtual void ClearFunciones() { Funciones.Clear(); }

        public virtual void AddFuncion(MovMenu funcion) { Funciones.Add(funcion); }

        public virtual bool HasMobile(Coche mobile) { return Coches.Cast<object>().Contains(mobile); }

        #endregion
    }
}