using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Vehiculos
{
    [Serializable]
    public class Modelo : IAuditable, ISecurable, IHasMarca
    {
        public virtual Type TypeOf() { return GetType(); }
        
        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual Marca Marca { get; set; }
        public virtual Insumo Insumo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Codigo { get; set; }
        public virtual double Rendimiento { get; set; }
        public virtual double Capacidad { get; set; }
        public virtual double CostoLitro { get; set; }
        public virtual int Costo { get; set; }
        public virtual int VidaUtil { get; set; }
        public virtual bool Baja { get; set; }
        public virtual double RendimientoRalenti { get; set; }
    }
}