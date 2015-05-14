#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.ControlDeCombustible
{
    [Serializable]
    public class VolumenHistoricoInvalido : IAuditable
    {
        public VolumenHistoricoInvalido() {}

        public VolumenHistoricoInvalido(VolumenHistorico volume, int motivoDescarte)
        {
            Fecha = volume.Fecha;
            Volumen = volume.Volumen;
            EsTeorico = volume.EsTeorico;
            Tanque = volume.Tanque;
            VolumenAgua = volume.VolumenAgua;
            FechaDescarte = DateTime.Now;
            MotivoDescarte = motivoDescarte;
        }

        public virtual int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual double Volumen { get; set; }
        public virtual bool EsTeorico { get; set; }
        public virtual Tanque Tanque { get; set; }
        public virtual double VolumenAgua { get; set; }
        public virtual DateTime FechaDescarte { get; set; }
        public virtual int MotivoDescarte { get; set; }

        public virtual Type TypeOf() { return GetType(); }
    }
}
