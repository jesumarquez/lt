﻿using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.ControlAcceso
{
    [Serializable]
    public class CategoriaAcceso: IAuditable, ISecurable
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool Baja { get; set; }
    }
}
