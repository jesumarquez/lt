using System.Globalization;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Tickets;
using System;

namespace LogicOut.Server
{
    public static class Convert
    {
        public static class Queries
        {
            public const string Betonmac = "betonmac";
            public const string Molinete = "molinete";
        }
        public static class Types
        {
            public const string Ticket = "ticket";
            public const string Empleado = "empleado";
            public const string Tarjeta = "tarjeta";
        }
        public static OutData FromTicket(int id, Ticket ticket)
        {
            var data = new OutData(Types.Ticket);
            
            data.AddProperty("id", id.ToString());
            data.AddProperty("ticket.id", ticket.Id.ToString());
            data.AddProperty("ticket.codigo", ticket.Codigo);
            data.AddProperty("ticket.cantidad", ticket.CantidadCarga);
            data.AddProperty("ticket.cantidadpedido", ticket.CantidadPedido);
            data.AddProperty("ticket.cantidadacumulada", ticket.CumulativeQty);
            data.AddProperty("ticket.codigoproducto", ticket.Pedido.Producto.Codigo);
            data.AddProperty("ticket.producto", ticket.Pedido.Producto.Descripcion);
            data.AddProperty("ticket.usaprefijo", ticket.Pedido.Producto.UsaPrefijo ? "true" : "false");
            data.AddProperty("ticket.fecha", ticket.FechaTicket.Value.ToString(CultureInfo.InvariantCulture));
            data.AddProperty("ticket.estado", ticket.Estado.ToString(CultureInfo.InvariantCulture));
            data.AddProperty("ticket.orden", ticket.OrdenDiario.ToString());

            var cliref = ticket.Cliente.ReferenciaGeografica != null && ticket.Cliente.ReferenciaGeografica.Direccion != null;
            data.AddProperty("cliente.codigo", ticket.Cliente.Codigo);
            data.AddProperty("cliente.nombre", ticket.Cliente.Descripcion);
            data.AddProperty("cliente.telefono", ticket.Cliente.Telefono);
            data.AddProperty("cliente.observaciones", ticket.Cliente.Comentario1 ?? string.Empty);
            data.AddProperty("cliente.direccion", cliref ? ticket.Cliente.ReferenciaGeografica.Descripcion : string.Empty);
            data.AddProperty("cliente.localidad", cliref ? ticket.Cliente.ReferenciaGeografica.Direccion.Partido : string.Empty);
            data.AddProperty("cliente.provincia", cliref ? ticket.Cliente.ReferenciaGeografica.Direccion.Provincia : string.Empty);
            data.AddProperty("cliente.pais", cliref ? ticket.Cliente.ReferenciaGeografica.Direccion.Pais : string.Empty);

            data.AddProperty("obra.codigo", ticket.PuntoEntrega.Codigo);
            data.AddProperty("obra.nombre", ticket.PuntoEntrega.Descripcion);
            data.AddProperty("obra.direccion", ticket.PuntoEntrega.ReferenciaGeografica.Descripcion);
            data.AddProperty("obra.localidad", ticket.PuntoEntrega.ReferenciaGeografica.Direccion != null ? ticket.PuntoEntrega.ReferenciaGeografica.Direccion.Partido : string.Empty);
            data.AddProperty("obra.observaciones", ticket.PuntoEntrega.Comentario1 ?? string.Empty);
            data.AddProperty("obra.telefono", ticket.PuntoEntrega.Telefono);

            data.AddProperty("pedido.codigo", ticket.Pedido.Codigo);
            data.AddProperty("pedido.planta", ticket.Pedido.BocaDeCarga.Codigo);
            data.AddProperty("pedido.cantidad", (ticket.Pedido.Cantidad + ticket.Pedido.CantidadAjuste).ToString());
            data.AddProperty("pedido.contacto", ticket.Pedido.Contacto);
            data.AddProperty("pedido.minimixer", ticket.Pedido.EsMinimixer ? "true" : "false");
            
            return data;
        }
        public static OutData ToMolinete(int id, Empleado empleado)
        {
            var data = new OutData(Types.Empleado);

            var pin = GetPinCerbero(empleado.Tarjeta);

            data.AddProperty("id", id.ToString());
            data.AddProperty("empleado.id", empleado.Id.ToString());
            data.AddProperty("empleado.dni", empleado.Entidad.NroDocumento);
            data.AddProperty("empleado.nombre", empleado.Entidad.Descripcion);
            data.AddProperty("empleado.apellido", string.Empty);
            data.AddProperty("empleado.email", string.Empty);
            data.AddProperty("empleado.telefono", string.Empty);
            data.AddProperty("empleado.fechanac", string.Empty);
            data.AddProperty("empleado.nacionalidad", string.Empty);
            data.AddProperty("empleado.domicilio", string.Empty);
            data.AddProperty("empleado.codigopostal", string.Empty);
            data.AddProperty("empleado.localidad", string.Empty);
            data.AddProperty("empleado.pais", string.Empty);
            data.AddProperty("empleado.categoria", empleado.Categoria != null ? empleado.Categoria.Nombre : string.Empty);
            data.AddProperty("empleado.sexo", "0");
            data.AddProperty("empleado.provincia", string.Empty);
            data.AddProperty("empleado.legajo", empleado.Legajo);
            data.AddProperty("empleado.cuil", empleado.Entidad.Cuil);
            data.AddProperty("empleado.tarjeta", pin.ToString(CultureInfo.InvariantCulture));

            return data;
        }
        public static OutData ToMolinete(int id, Tarjeta tarjeta)
        {
            var data = new OutData(Types.Tarjeta);

            var today = DateTime.Today;
            var pin = GetPinCerbero(tarjeta);

            data.AddProperty("id", id.ToString());
            data.AddProperty("tarjeta.id", tarjeta.Id.ToString(CultureInfo.InvariantCulture));
            data.AddProperty("tarjeta.pin", pin.ToString(CultureInfo.InvariantCulture));
            data.AddProperty("tarjeta.desde", today.ToString(CultureInfo.InvariantCulture));
            data.AddProperty("tarjeta.hasta", today.AddYears(10).ToString(CultureInfo.InvariantCulture));
            data.AddProperty("tarjeta.estado", "1");
            data.AddProperty("tarjeta.tipo", "1");
            return data;
        }

        public static int GetPinCerbero(Tarjeta tarjeta)
        {
            const int hexlen = 6;
            var pin = tarjeta != null ? tarjeta.Pin.Length < hexlen ? tarjeta.Pin.PadLeft(hexlen, '0') : tarjeta.Pin.Substring(tarjeta.Pin.Length - hexlen) : "0";
            try
            {
                return int.Parse(pin, NumberStyles.HexNumber);
                
            }catch(Exception e)
            {
                STrace.Exception(typeof(Convert).FullName, e, string.Format("Error de conversion de pin id tarjeta {0}",tarjeta == null ? -1:tarjeta.Id));
                return -1;
            }
        }
    }
}
