#region Usings

using System;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Estado : IAuditable, ISecurable
    {
        public static class Evento
        {
            public const short Normal = 0;
            public const short GiroTrompoDerecha = 1;
            public const short GiroTrompoIzquierda = 2;
            public const short TolvaActiva = 3;
            public const short TolvaInactiva = 4;
            public const short SaleDePlanta = 5;
            public const short LlegaAObra = 6;
            public const short SaleDeObra = 7;
            public const short LlegaAPlanta = 8;
            public const short GiroTrompoDetenido = 9;
            public const short Iniciado = 10;
            public const short GiroTrompoHorarioLento = 11;
            public const short GiroTrompoHorarioRapido = 12;
            public const short GiroTrompoAntihorarioLento = 13;
            public const short GiroTrompoAntihorarioRapido = 14;
        }

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        #region ISecurable

        public virtual Empresa Empresa { get { return Linea == null ? null : Linea.Empresa; } }
        public virtual Linea Linea { get; set; }

        #endregion

        public virtual Mensaje Mensaje { get; set; }

        public virtual Icono Icono { get; set; }

        public virtual string Descripcion { get; set; }

        public virtual short Deltatime { get; set; }

        public virtual int Codigo { get; set; }

        public virtual short Orden { get; set; }

        public virtual bool Modo { get; set; }

        public virtual short EsPuntoDeControl { get; set; }

        public virtual bool Informar { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var estado = obj as Estado;
            if (estado == null) return false;
            return Id == estado.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}