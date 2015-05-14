using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using LogicOut.Core;
using System.Globalization;
using LogicOut.Core.Export;

namespace LogicOut.Handlers
{
    public class Betonmac: IOutHandler
    {
        public static class Estados
        {
            public const short Eliminado = -1;
            public const short Pendiente = 0;
            public const short EnCurso = 1;
            public const short Anulado = 8;
            public const short Cerrado = 9;
        }

        public class Planta
        {
            public string Nombre { get; set; }
            public string Codigo { get; set; }
            public string ConnectionString { get; set; }
            public string ConnectionStringDespachos { get; set; }
            public string DespachosTableName { get; set; }
        }

        private const string PlantasPrefix = "logicout.betonmac.plantas.";

        Dictionary<string, bool> doneClientes = new Dictionary<string, bool>();
        Dictionary<string, bool> donePedidos = new Dictionary<string, bool>();
        Dictionary<string, bool> doneObras = new Dictionary<string, bool>();

        Dictionary<string, Planta> Plantas { get; set; }

        public Betonmac(string name):base(name)
        {
            // Handlers
            var plantas = ConfigurationManager.AppSettings.AllKeys.Where(k => k.ToLower().StartsWith(PlantasPrefix));

            Plantas = new Dictionary<string, Planta>(plantas.Count());

            foreach (var planta in plantas)
            {
                var nam = planta.Substring(PlantasPrefix.Length);
                var value = ConfigurationManager.AppSettings[planta];
                var conn = ConfigurationManager.AppSettings["logicout.betonmac." + nam + ".connectionstring"];
                var conn2 = ConfigurationManager.AppSettings["logicout.betonmac." + nam + ".connectionstringdespachos"];
                var tablename = ConfigurationManager.AppSettings["logicout.betonmac." + nam + ".tablename"];
                Plantas.Add(value, new Planta { Nombre = nam, Codigo = value, ConnectionString = conn, ConnectionStringDespachos = conn2, DespachosTableName = tablename });
            }
        }
        #region IOutHandler Members

        public override void Process()
        {
            doneClientes.Clear();
            donePedidos.Clear();
            doneObras.Clear();

            Logger.Debug("Pidiendo datos al servidor...");
            var tickets = Server.Export.ExportData(Config.SessionToken, Config.Company, Config.Branch, "betonmac", string.Empty);
            if(!tickets.RespuestaOk)
            {
                Logger.Error("No se pudo obtener los datos del server.\n "+tickets.Mensaje);
                return;
            }
            
            var plantas = tickets.Resultado.GroupBy(t => t["pedido.planta"]);
            Logger.Info(tickets.Resultado.Count() + " tickets recibidos. " + plantas.Count() + " plantas");

            foreach (var listPlanta in plantas)
            {
                try
                {
                    Logger.Debug("version 20121101.2");
                    Logger.Debug("Procesando planta " + listPlanta.Key);
                    if (!Plantas.ContainsKey(listPlanta.Key))
                    {
                        Logger.Error("No se encontró la configuración para la planta: " + listPlanta.Key);
                        foreach(var ticket in listPlanta)
                        {
                            MarcarComoSincronizado(ticket);
                        }

                        continue;
                    }
                    var planta = Plantas[listPlanta.Key];
                    var pedidos = listPlanta.GroupBy(p => p["pedido.codigo"]);
                    Logger.Debug(pedidos.Count() + " pedidos para la planta " + listPlanta.Key);

                    var connection = new OleDbConnection(planta.ConnectionString);
                    var connectionDespachos = new OleDbConnection(planta.ConnectionStringDespachos);
                    connection.Open();
                    connectionDespachos.Open();
                    foreach (var pedido in pedidos)
                    {
                        foreach (var data in pedido)
                        {
                            Logger.Debug(" ");
                            var estado = Convert.ToInt32(data["ticket.estado"]);

                            var txtEstado = estado == Estados.Pendiente ? "Pendiente" :estado == Estados.Anulado ? "Anulado" : estado ==Estados.Eliminado ? "Eliminado" : estado.ToString();
                            Logger.Debug("Sincronizando ticket " + data["ticket.codigo"] + ". Estado: " + txtEstado);

                            var transaction = connection.BeginTransaction();
                            var transactionDespachos = connectionDespachos.BeginTransaction();
                            try
                            {
                                switch (estado)
                                {
                                    case Estados.Pendiente:
                                        SyncCliente(connection,transaction, data);
                                        SyncObra(connection, transaction, data);
                                        var exists = SyncDespacho(connectionDespachos, transactionDespachos, data, planta);
                                        if(!exists) SyncPedido(connection, transaction, data);
                                        break;
                                    case Estados.Anulado:
                                    case Estados.Eliminado:
                                        CancelDespacho(connection, transaction, connectionDespachos, transactionDespachos, data, planta);
                                        break;
                                }
                                Logger.Debug("Aplicando cambios a base Administracion.");                                
                                transaction.Commit();
                                Logger.Debug("Aplicando cambios a base Despachos.");
                                transactionDespachos.Commit();
                                if (estado == Estados.Pendiente)
                                {
                                    if (!doneClientes.ContainsKey(data["cliente.codigo"])) doneClientes.Add(data["cliente.codigo"], true);
                                    var obraKey = string.Format("{0}:{1}", data["cliente.codigo"], data["obra.codigo"]);
                                    if (!doneObras.ContainsKey(obraKey)) doneObras.Add(obraKey, true);
                                    if (!donePedidos.ContainsKey(data["pedido.codigo"])) donePedidos.Add(data["pedido.codigo"], true);
                                }
                                MarcarComoSincronizado(data);
                            }
                            catch(Exception ex)
                            {
                                transaction.Rollback();
                                transactionDespachos.Rollback();
                                Logger.Error("Error procesando pedido.\n" + ex.ToString());
                                if(ex.InnerException != null)
                                {
                                    Logger.Error("InnerException: \n" + ex.InnerException.ToString());
                                }
                            }
                        }

                    }

                    connection.Close();
                    connectionDespachos.Close();
                }
                catch(Exception ex)
                {
                    Logger.Error("Error procesando la planta: " + listPlanta.Key + "\n" + ex);
                }
            }
        }

        protected void MarcarComoSincronizado(OutData data)
        {
            try
            {
                Logger.Debug("Marcando ticket " + data["ticket.id"] + " como sincronizado.");
                Server.Export.Done(Config.SessionToken, Config.Company, Config.Branch, "betonmac",
                                   data["ticket.id"]);
            }
            catch
            {
                Logger.Info("No se pudo marcar el registro como sincronizado.");
            }
        }

        protected void SyncCliente(OleDbConnection connection, OleDbTransaction transaction, OutData data)
        {
            if (doneClientes.ContainsKey(data["cliente.codigo"])) return;

            var codigo = Trunc(data["cliente.codigo"], 10);
            var nombre = Trunc(data["cliente.nombre"], 40);

            Logger.Debug("Sincronizando cliente " + nombre + ". Codigo: " + codigo);
            
            var direccion = Trunc(data["cliente.direccion"],40);
            var localidad = Trunc(data["cliente.localidad"], 20);
            var codpos = string.Empty;
            var provincia = Trunc(data["cliente.provincia"], 20);
            var pais = Trunc(data["cliente.pais"], 20);
            var telefono = Trunc(data["cliente.telefono"], 20);
            var fax = string.Empty;
            var condiva = string.Empty;
            var cuit = string.Empty;
            var ingbrut = string.Empty;
            var obs = Trunc(data["cliente.observaciones"], 40) ?? string.Empty;
            var habil = "0";

            var command = GetCommand(connection, transaction, "SELECT * FROM Clientes WHERE Codigo = @codigo");
            command.Parameters.AddWithValue("@codigo", codigo);
            var reader = command.ExecuteReader();
            var exists = reader.Read();
            reader.Close();

            string script;
            if(exists)
            {
                Logger.Debug("El cliente con código " + codigo + " ya existe. Actualizando.");
                script =@"UPDATE Clientes SET 
    Nombre = @nombre, 
    Direccion = @direccion, 
    Localidad = @localidad,
    CodPos = @codpos,
    Provincia = @provincia,
    Pais = @pais,
    Telefono = @telefono,
    Fax = @fax,
    CondIVA = @condiva,
    Cuit = @cuit,
    IngBrut = @ingbrut,
    Obs = @obs,
    habil = @habil,
    FechaActualiz = Now()
    WHERE Codigo = @codigo";
            }
            else
            {
                Logger.Debug("Insertando nuevo cliente con código " + codigo);
                script =
                    @"INSERT INTO Clientes (Nombre, Direccion, Localidad, CodPos, Provincia, Pais, Telefono, Fax, CondIVA, Cuit, IngBrut, Obs, Habil, FechaActualiz, Codigo)
    VALUES (@nombre, @direccion, @localidad,@codpos, @provincia, @pais, @telefono, @fax, @condiva, @cuit, @ingbrut, @obs, @habil, Now(), @codigo)";

            }

            command.CommandText = script;
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@nombre", nombre);
            command.Parameters.AddWithValue("@direccion", direccion);
            command.Parameters.AddWithValue("@localidad", localidad);
            command.Parameters.AddWithValue("@codpos", codpos);
            command.Parameters.AddWithValue("@provincia", provincia);
            command.Parameters.AddWithValue("@pais", pais);
            command.Parameters.AddWithValue("@telefono", telefono);
            command.Parameters.AddWithValue("@fax", fax);
            command.Parameters.AddWithValue("@condiva", condiva);
            command.Parameters.AddWithValue("@cuit", cuit);
            command.Parameters.AddWithValue("@ingbrut", ingbrut);
            command.Parameters.AddWithValue("@obs", obs);
            command.Parameters.AddWithValue("@habil", habil);
            command.Parameters.AddWithValue("@codigo", codigo);

            command.ExecuteNonQuery();
        }

        protected void SyncObra(OleDbConnection connection, OleDbTransaction transaction, OutData data)
        {
            var obraKey = string.Format("{0}:{1}", data["cliente.codigo"], data["obra.codigo"]);
            if (doneObras.ContainsKey(obraKey)) return;

            var cliente = data["cliente.codigo"];
            var codigo = Convert.ToDouble(data["obra.codigo"]);
            var nombrecli = Trunc(data["cliente.nombre"], 40);
            var nombre = Trunc(data["obra.nombre"], 40);


            Logger.Debug("Sincronizando obra " + nombre + ". Codigo: " + codigo + ". Cliente " + nombrecli + ". (" + cliente+")");

            var direccion = Trunc(data["obra.direccion"], 40);
            var localidad = Trunc(data["obra.localidad"], 40);
            var observaciones = Trunc(data["obra.observaciones"], 40) ?? string.Empty;

            var command = GetCommand(connection, transaction, "SELECT * FROM EncabOC WHERE Codigo = @cliente and NroOc = @codigo");
            command.Parameters.AddWithValue("@cliente", cliente);
            command.Parameters.AddWithValue("@codigo", codigo);
            var reader = command.ExecuteReader();
            var exists = reader.Read();
            reader.Close();

            string script;
            if(exists)
            {
                Logger.Debug("La obra con código " + codigo + " del cliente " + cliente + " ya existe. Actualizando.");
                script = @"UPDATE EncabOC SET 
    Nombre = @nombrecli,
    Obra = @nombre,
    DireccionObra = @direccion,
    LocObra = @localidad,
    ObservOC = @observaciones
    WHERE  Codigo = @cliente and NroOc = @codigo";
            }
            else
            {
                Logger.Debug("Insertando nueva obra con código " + codigo + " para el cliente " + cliente);
                script = @"INSERT INTO EncabOC (FechaEmision, Nombre, Obra, DireccionObra, LocObra, ObservOC, TipoRem, FechaVtoCtrato, PorcenAjuste, Codigo, NroOc)
    VALUES (Now(), @nombrecli,@nombre, @direccion, @localidad, @observaciones, 1, null, 0, @cliente, @codigo)";
            }

            command.CommandText = script;
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@nombrecli", nombrecli);
            command.Parameters.AddWithValue("@nombre", nombre);
            command.Parameters.AddWithValue("@direccion", direccion);
            command.Parameters.AddWithValue("@localidad", localidad);
            command.Parameters.AddWithValue("@observaciones", observaciones);
            command.Parameters.AddWithValue("@cliente", cliente);
            command.Parameters.AddWithValue("@codigo", codigo);
            command.ExecuteNonQuery();

            
            // Actualización de OCTabla
            if (exists)
            {
                command = GetCommand(connection, transaction, "SELECT * FROM OCTabla WHERE Codigo = @cliente and NroOC = @codigo");
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@cliente", cliente);
                command.Parameters.AddWithValue("@codigo", codigo);
                reader = command.ExecuteReader();
                exists = reader.Read();
                reader.Close();
            }
            if (!exists)
            {
                Logger.Debug("Insertando en OCTabla. obra: " + codigo + ". cliente: " + cliente);
                script = "INSERT INTO OCTabla (NroOC, Fecha, Codigo) VALUES (@codigo, null, @cliente)";

                command.CommandText = script;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@codigo", codigo);
                command.Parameters.AddWithValue("@cliente", cliente);
                
                command.ExecuteNonQuery();
            }
            else
            {
                script = "UPDATE OCTabla SET Fecha = null WHERE NroOC = @codigo AND Codigo = @cliente";

                command.CommandText = script;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@codigo", codigo);
                command.Parameters.AddWithValue("@cliente", cliente);

                command.ExecuteNonQuery();
            }
        }

        protected void SyncPedido(OleDbConnection connection, OleDbTransaction transaction, OutData data)
        {
            if (donePedidos.ContainsKey(data["pedido.codigo"])) return;

            var cliente = data["cliente.codigo"];
            var obra = Convert.ToDouble(data["obra.codigo"]);
            var producto = GetCodigoProducto(data);
            var cantidad = ToDouble(data["pedido.cantidad"]);
            var observaciones = string.Empty;

            Logger.Debug("Sincronizando pedido. Cliente: " + cliente + ". Obra: " + obra + ". Formula: " + producto + ". Cantidad: " + cantidad);

            var command = GetCommand(connection, transaction, "SELECT * FROM CompoOC WHERE Codigo = @cliente and NroOc = @obra and CodigoForm = @producto");
            command.Parameters.AddWithValue("@cliente", cliente);
            command.Parameters.AddWithValue("@obra", obra);
            command.Parameters.AddWithValue("@producto", producto);
            var reader = command.ExecuteReader();
            var exists = reader.Read();
            reader.Close();

            string script;
            if (exists)
            {
                Logger.Debug("Agregando saldo...");
                script = @"UPDATE CompoOC SET 
    M3Ajus = M3Ajus + @m3ajus,
    M3Saldo = M3Saldo + @m3saldo
    WHERE  Codigo = @cliente and NroOc = @obra and CodigoForm = @producto";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@m3ajus", cantidad);
                command.Parameters.AddWithValue("@m3saldo", cantidad);
                command.Parameters.AddWithValue("@cliente", cliente);
                command.Parameters.AddWithValue("@obra", obra);
                command.Parameters.AddWithValue("@producto", producto);
            }
            else
            {
                Logger.Debug("Agregando fórmula y saldo...");
                script = @"INSERT INTO CompoOC (NroOC, CodigoForm, M3Sol, M3Ajus, M3Entreg, M3Saldo, ObsFormu, Codigo)
    VALUES (@obra, @producto, @m3sol, 0, 0,@m3saldo, @obsformu, @cliente)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@obra", obra);
                command.Parameters.AddWithValue("@producto", producto);
                command.Parameters.AddWithValue("@m3sol", cantidad);
                command.Parameters.AddWithValue("@m3saldo", cantidad);
                command.Parameters.AddWithValue("@obsformu", observaciones);
                command.Parameters.AddWithValue("@cliente", cliente);
            }

            command.CommandText = script;
            
            command.ExecuteNonQuery();
        }
        protected bool SyncDespacho(OleDbConnection connection, OleDbTransaction transaction, OutData data, Planta planta)
        {
            var obra = Convert.ToDouble(data["obra.codigo"]);
            var fechaHora = Convert.ToDateTime(data["ticket.fecha"], CultureInfo.InvariantCulture).AddHours(-3);
            var fecha = fechaHora.Date;
            var hora = fechaHora.ToString("HH:mm");
            var codigoproducto = GetCodigoProducto(data);
            var descriproducto = data["ticket.producto"];
            var codigocliente = data["cliente.codigo"];
            var descricliente = data["cliente.nombre"];
            var m3sol = ToSingle(data["ticket.cantidad"]);
            var orden = Convert.ToInt32(data["ticket.orden"]);
            var obradesc = Trunc(data["obra.nombre"], 40);
            var codigo = Trunc(data["ticket.codigo"], 20);

            Logger.Debug("Sincronizando Despacho " + fechaHora.ToString("dd/MM/yyyy HH:mm") + ". Orden: " + orden);

            var command = GetCommand(connection, transaction, "SELECT * FROM " + planta.DespachosTableName + " WHERE Fecha = @fecha and TicketNro = @orden");
            command.Parameters.AddWithValue("@fecha", fecha);
            command.Parameters.AddWithValue("@orden", orden);
            var reader = command.ExecuteReader();
            var exists = reader.Read();
            reader.Close();

            string script;
            if (!exists)
            {
                Logger.Debug("Insertando nuevo despacho...");
                script =
                    @"INSERT INTO "+planta.DespachosTableName+ @" (OrdenCompra,HabilAutom,Hora,Formula,CodAsociado,M3Sol,
        Codigo, Cliente, Ejecuto, ReOrden, 
        Fecha, Mezclador, TipoRem, TicketNro, TolvaDest, M3Dosif, M3SolTeo, DatoAuxVsM, FormuEnLD, Obra,
        CostoHo, UnidHo, ModoTransp, ContratoAdm, Pedido_Vta)
    VALUES (@obra,0,@hora,@codigoproducto,@descriproducto,@m3sol,
        @codigocliente, @descricliente, 'no', 0,
        @fecha,'0000M',1,@orden, 'A',0,0, 0,0, @obradesc,
        '','','',@orden2,@codigo)";


                command.CommandText = script;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@obra", obra);
                command.Parameters.AddWithValue("@hora", hora);
                command.Parameters.AddWithValue("@codigoproducto", codigoproducto);
                command.Parameters.AddWithValue("@descriproducto", descriproducto);
                command.Parameters.AddWithValue("@m3sol", m3sol);
                command.Parameters.AddWithValue("@codigocliente", codigocliente);
                command.Parameters.AddWithValue("@descricliente", descricliente);
                command.Parameters.AddWithValue("@fecha", fecha);
                command.Parameters.AddWithValue("@orden", orden);
                command.Parameters.AddWithValue("@obradesc", obradesc);
                command.Parameters.AddWithValue("@orden2", orden);
                command.Parameters.AddWithValue("@codigo", codigo);

                command.ExecuteNonQuery();
            }
            else
            {
                Logger.Debug("El despacho ya existe.");
            }
            return exists;
        }

        protected void CancelDespacho(OleDbConnection connection, OleDbTransaction transaction, OleDbConnection connectionDespacho, OleDbTransaction transactionDespacho, OutData data, Planta planta)
        {
            var fechaHora = Convert.ToDateTime(data["ticket.fecha"], CultureInfo.InvariantCulture).AddHours(-3);
            var fecha = fechaHora.Date;
            var orden = Convert.ToInt32(data["ticket.orden"]);

            var cliente = data["cliente.codigo"];
            var obra = data["obra.codigo"];
            var producto = GetCodigoProducto(data);
            var cantidad = ToSingle(data["ticket.cantidad"]);

            Logger.Debug("Cancelando Despacho " + fechaHora.ToString("dd/MM/yyyy HH:mm") + ". Orden: " + orden);

            var command = GetCommand(connectionDespacho, transactionDespacho, "SELECT * FROM " + planta.DespachosTableName + " WHERE Fecha = @fecha and TicketNro = @orden");
            command.Parameters.AddWithValue("@fecha", fecha);
            command.Parameters.AddWithValue("@orden", orden);
            var reader = command.ExecuteReader();
            var exists = reader.Read();
            reader.Close();

            if (exists)
            {
                Logger.Debug("Eliminando despacho.");
                command.CommandText = @"DELETE FROM "+planta.DespachosTableName+" WHERE Fecha = @fecha and TicketNro = @orden";

                command.Parameters.Clear();
                command.Parameters.AddWithValue("@fecha", fecha);
                command.Parameters.AddWithValue("@orden", orden);

                command.ExecuteNonQuery();

                Logger.Debug("Restando saldo." + cantidad);
                command = GetCommand(connection, transaction, @"UPDATE CompoOC SET 
        M3Sol = M3Sol - @cantidad,
        M3Saldo = M3Saldo - @cantidad2
        WHERE  Codigo = @cliente and NroOc = @obra and CodigoForm = @producto");
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@cantidad", cantidad);
                command.Parameters.AddWithValue("@cantidad2", cantidad);
                command.Parameters.AddWithValue("@cliente", cliente);
                command.Parameters.AddWithValue("@obra", obra);
                command.Parameters.AddWithValue("@producto", producto);

                command.ExecuteNonQuery();
            }
            else
            {
                Logger.Debug("No existe el despacho. Ignorando.");
            }
        }


        private OleDbCommand GetCommand(OleDbConnection connection, OleDbTransaction transaction, string script)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = script;
            return command;
        }
        private string Trunc(string text, int length)
        {
            if (text == null) return null;
            return text.Length > length ? text.Substring(0, length) : text;
        }
        private double ToDouble(string text)
        {
            double d;
            text = text.Replace(',', '.');
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;

            throw new FormatException("Formato incorrecto. Se esperaba un número. Se encontró: "+text);
        }
        private double ToSingle(string text)
        {
            Single d;
            text = text.Replace(',', '.');
            if (Single.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;

            throw new FormatException("Formato incorrecto. Se esperaba un número. Se encontró: " + text);
        }
        private string GetCodigoProducto(OutData data)
        {
            var usaprefijo = Convert.ToBoolean(data["ticket.usaprefijo"]);
            var producto = data["ticket.codigoproducto"];
            if (!usaprefijo) return producto;
            var planta = data["pedido.planta"];
            return planta + producto;
        }
        #endregion
    }
}
