using System;
using System.Collections.Generic;
using LogicOut.Core;
using LogicOut.Core.Export;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Globalization;
using System.Text;

namespace LogicOut.Handlers
{
    public class Molinete : IOutHandler
    {
        private const string Query = "molinete";
        private const string ConnStringKey = "connstring";
        private const string LastDateKey = "lastDate";
        public string ConnectionString { get; set; }

        public Molinete(string name)
            : base(name)
        {
            Logger.Info("Cargando configuración...");
            ConnectionString = GetParameterValue(ConnStringKey);

            if(string.IsNullOrEmpty(ConnectionString))
            {
                Logger.Fatal(string.Format("No se encontró el parámetro de configuración '{0}'", GetParameterKey(ConnStringKey)));
                return;
            }

            Logger.Debug("ConnectionString: " + ConnectionString);
        }

        public override void Process()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Sicronizar Fichadas
                    SendFichadas(connection);

                    //Sincronizar Empelados-Tarjetas
                    var outData = GetData(Query, string.Format("server={0}", Config.ServerName));
                    if (outData == null) return;

                    foreach (var data in outData)
                    {
                        var transaction = connection.BeginTransaction();
                        try
                        {
                            switch (data.Entity)
                            {
                                case "empleado":
                                    CreateEmpleado(connection, transaction, data);
                                    break;
                                case "tarjeta":
                                    CreateTarjeta(connection, transaction, data);
                                    break;
                            }

                            transaction.Commit();

                            var id = data.AsInt32("id");
                            if (id.HasValue)
                            {
                                MarkAsDone(Query, id.Value, true);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Logger.Error(string.Format("Error procesando {0}.\n{1}", data.Entity, ex));
                            if (ex.InnerException != null)
                            {
                                Logger.Error(string.Format("InnerException: \n{0}", ex.InnerException));
                            }
                        }
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("Error procesando Fichadas.\n{0}", ex));
                    if (ex.InnerException != null)
                    {
                        Logger.Error(string.Format("InnerException: \n{0}", ex.InnerException));
                    }
                }
            }
        }

        protected void LogQuery (MySqlCommand command)
        {
            try
            {
                var builder = new StringBuilder("Query : " + command.CommandText);

                foreach (MySqlParameter parameter in command.Parameters)
                {
                    builder.AppendFormat(" '{0}' : '{1}'", parameter.ParameterName , parameter.Value);
                }
                
                Logger.Debug(builder.ToString());

            }catch(Exception e)
            {
                Logger.Error(e.ToString());
            }
        }

        protected void SendFichadas(MySqlConnection connection)
        {
            const string select = "select IDTR, DTIME, IDEquipment, Point, Type, Media, IDZonaIN, IDZonaOUT, DNI, Category from transaction where DTIME > @lastDate";
            

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            DateTime lastDate;
            var lastDateKey = GetParameterKey(LastDateKey);
            var param = config.AppSettings.Settings[lastDateKey];
            if(param == null || !DateTime.TryParse(param.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out lastDate))
            {
                lastDate = new DateTime(2000,01,01);
            }

            var command = GetCommand(connection, null, select);


            command.Parameters.AddWithValue("@lastDate", lastDate);

            var result = new List<string>();

            LogQuery(command);

            var reader = command.ExecuteReader();
            while(reader.Read())
            {
                try
                {
                    var fecha = reader.GetDateTime("DTIME");
                    var equipo = reader.GetInt32("IDEquipment");
                    var point = reader.GetInt32("Point");
                    var tipo = reader.GetInt32("Type");
                    var dni = reader.GetString("DNI");
                    var categoria = reader.GetString("Category");
                    var tarjeta = reader.GetString("Media");
                    var direccion = reader.GetString("IDZonaIN") == "Principal" ? "0" : "1";
                    var message = string.Format("F:{0}:{1}:{2}:{3}:{4}:{5}", equipo, point, tipo,
                                                fecha.ToUniversalTime().ToString("yyyyMMddHHmmss"), tarjeta, direccion);
                    result.Add(message);

                    var parameters = string.Format("server={0};data={1}", Config.ServerName, message);

                    Logger.Debug("Enviando datos al servidor: " + parameters);
                    var response = GetData("fichada", parameters);

                    if(response == null || response.Length == 0)
                    {
                        throw new ApplicationException("La respuesta de la fichada es null o vacía");
                    }
                    if(response[0]["done"] != "true")
                    {
                        throw new ApplicationException(response[0]["message"]);
                    }

                    if (fecha > lastDate)
                    {
                        lastDate = fecha;

                        if (param == null) config.AppSettings.Settings.Add(lastDateKey, lastDate.ToString(CultureInfo.InvariantCulture));
                        else config.AppSettings.Settings[lastDateKey].Value = lastDate.ToString(CultureInfo.InvariantCulture);
                        config.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");
                    }
                }
                catch(Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            reader.Close();
        }

        protected void CreateTarjeta(MySqlConnection connection, MySqlTransaction transaction, OutData data)
        {
            var id = data.AsInt32("tarjeta.id") ?? 0;
            var pin = data.AsInt32("tarjeta.pin") ?? 0;
            var desde = data.AsDateTime("tarjeta.desde") ?? DateTime.Now;
            var hasta = data.AsDateTime("tarjeta.hasta") ?? desde.AddYears(10);
            var estado = data.AsInt32("tarjeta.estado") ?? 1;
            var tipo = data.AsInt32("tarjeta.tipo") ?? 1;

            const string select = "SELECT * FROM cardenable WHERE IDCard=@pin AND Type=@tipo";
            const string insert = "INSERT INTO cardenable (IDCard,DDesde,DHasta,State,Type) VALUES(@pin,@desde,@hasta,@estado,@tipo)";
            const string update = "UPDATE cardenable set DDesde=@desde,DHasta=@hasta,State=@estado WHERE IDCard=@pin AND Type=@tipo";

            var command = GetCommand(connection, transaction, select);
            command.Parameters.AddWithValue("@pin", pin);
            command.Parameters.AddWithValue("@tipo", tipo);
            var exists = Exists(command);

            if(exists)
            {
                Logger.Debug(string.Format("UPDATE Tarjeta:id={0};pin={1};desde={2};hasta={3};estado={4};tipo={5}", id, pin, desde, hasta, estado, tipo));
                command = ResetCommand(command, update);
                command.Parameters.AddWithValue("@desde", desde);
                command.Parameters.AddWithValue("@hasta", hasta);
                command.Parameters.AddWithValue("@estado", estado);
                command.Parameters.AddWithValue("@pin", pin.ToString());
                command.Parameters.AddWithValue("@tipo", tipo);
                command.ExecuteNonQuery();
            }
            else
            {
                Logger.Debug(string.Format("INSERT Tarjeta:id={0};pin={1};desde={2};hasta={3};estado={4};tipo={5}", id, pin, desde, hasta, estado, tipo));
                command = ResetCommand(command, insert);
                command.Parameters.AddWithValue("@pin", pin.ToString());
                command.Parameters.AddWithValue("@desde", desde);
                command.Parameters.AddWithValue("@hasta", hasta);
                command.Parameters.AddWithValue("@estado", estado);
                command.Parameters.AddWithValue("@tipo", tipo);
                command.ExecuteNonQuery();
            }
        }
        protected void CreateEmpleado(MySqlConnection connection, MySqlTransaction transaction, OutData data)
        {
            var id = data.AsInt32("empleado.id") ?? 0;
            var dni = data.AsString("empleado.dni", 20) ?? string.Empty;
            var nombre = data.AsString("empleado.nombre",32) ?? string.Empty;
            var apellido = data.AsString("empleado.apellido",32) ?? string.Empty;
            var email = data.AsString("empleado.email",50) ?? string.Empty;
            var telefono = data.AsString("empleado.telefono", 20) ?? string.Empty;
            var fechanac = data.AsDateTime("empleado.fechanac") ??  new DateTime(2000,1,1);
            var nacionalidad = data.AsString("empleado.nacionalidad", 20) ?? string.Empty;
            var domicilio = data.AsString("empleado.domicilio", 20) ?? string.Empty;
            var codigopostal = data.AsString("empleado.codigopostal", 20) ?? string.Empty;
            var localidad = data.AsString("empleado.localidad", 20) ?? string.Empty;
            var pais = data.AsString("empleado.pais", 20) ?? string.Empty;
            var sexo = data.AsByte("empleado.sexo") ?? 0;
            var categoria = data.AsString("empleado.categoria", 20) ?? "empleados";
            var fechamod = DateTime.Now;
            var provincia = data.AsString("empleado.provincia", 50) ?? string.Empty;
            var legajo = data.AsString("empleado.legajo", 32) ?? string.Empty;
            var cuil = data.AsString("empleado.cuil", 32) ?? string.Empty;

            var tarjeta = data.AsInt32("empleado.tarjeta") ?? 0;

            const string select = "SELECT * FROM usuarios WHERE DNI=@dni";
            const string insert = "INSERT INTO usuarios (DNI,Name,LastName,Email,Tel,FNacimiento,Nacionalidad,Domicilio,CodePost,Localidad,Pais,Category,Sexo,FModify,Provincia,Legajo,CUIL) VALUES (@dni,@name,@lastname,@email,@tel,@fechanac,@nacionalidad,@domicilio,@codigopostal,@localidad,@pais,@categoria,@sexo,@fechamod,@provincia,@legajo,@cuil)";
            const string update = "UPDATE usuarios SET Name=@name,LastName=@lastname,Email=@email,Tel=@tel,FNacimiento=@fechanac,Nacionalidad=@nacionalidad,Domicilio=@domicilio,CodePost=@codigopostal,Localidad=@localidad,Pais=@pais,Category=@categoria,Sexo=@sexo,FModify=@fechamod,Provincia=@provincia,Legajo=@legajo,CUIL=@cuil WHERE DNI=@dni";

            var command = GetCommand(connection, transaction, select);
            command.Parameters.AddWithValue("@dni", dni);
            var exists = Exists(command);

            if (exists)
            {
                Logger.Debug(string.Format("UPDATE Empleado:dni={0};nombre={1};apellido={2};email={3};telefono={4};fechanac={5}nacionalidad={6};domicilio={7};codigopostal={8};localidad={9};pais={10};categoria={11};sexo={12};fechamod={13};provincia={14};legajo={15};cuil={16};tarjeta={17};",dni, nombre, apellido, email, telefono, fechanac, nacionalidad,domicilio, codigopostal, localidad, pais, categoria,sexo, fechamod, provincia, legajo, cuil, tarjeta));
                command = ResetCommand(command, update);
                command.Parameters.AddWithValue("@name", nombre);
                command.Parameters.AddWithValue("@lastname", apellido);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@tel", telefono);
                command.Parameters.AddWithValue("@fechanac", fechanac);
                command.Parameters.AddWithValue("@nacionalidad", nacionalidad);
                command.Parameters.AddWithValue("@domicilio", domicilio);
                command.Parameters.AddWithValue("@codigopostal", codigopostal);
                command.Parameters.AddWithValue("@localidad", localidad);
                command.Parameters.AddWithValue("@pais", pais);
                command.Parameters.AddWithValue("@categoria", categoria);
                command.Parameters.AddWithValue("@sexo", sexo);
                command.Parameters.AddWithValue("@fechamod", fechamod);
                command.Parameters.AddWithValue("@provincia", provincia);
                command.Parameters.AddWithValue("@legajo", legajo);
                command.Parameters.AddWithValue("@cuil", cuil);
                command.Parameters.AddWithValue("@dni", dni);
                command.ExecuteNonQuery();
            }
            else
            {
                Logger.Debug(string.Format("INSERT Empleado:dni={0};nombre={1};apellido={2};email={3};telefono={4};fechanac={5}nacionalidad={6};domicilio={7};codigopostal={8};localidad={9};pais={10};categoria={11};sexo={12};fechamod={13};provincia={14};legajo={15};cuil={16};tarjeta={17};", dni, nombre, apellido, email, telefono, fechanac, nacionalidad, domicilio, codigopostal, localidad, pais, categoria, sexo, fechamod, provincia, legajo, cuil, tarjeta));
                command = ResetCommand(command, insert);
                command.Parameters.AddWithValue("@dni", dni);
                command.Parameters.AddWithValue("@name", nombre);
                command.Parameters.AddWithValue("@lastname", apellido);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@tel", telefono);
                command.Parameters.AddWithValue("@fechanac", fechanac);
                command.Parameters.AddWithValue("@nacionalidad", nacionalidad);
                command.Parameters.AddWithValue("@domicilio", domicilio);
                command.Parameters.AddWithValue("@codigopostal", codigopostal);
                command.Parameters.AddWithValue("@localidad", localidad);
                command.Parameters.AddWithValue("@pais", pais);
                command.Parameters.AddWithValue("@categoria", categoria);
                command.Parameters.AddWithValue("@sexo", sexo);
                command.Parameters.AddWithValue("@fechamod", fechamod);
                command.Parameters.AddWithValue("@provincia", provincia);
                command.Parameters.AddWithValue("@legajo", legajo);
                command.Parameters.AddWithValue("@cuil", cuil);
                command.ExecuteNonQuery();
            }

            if (tarjeta > 0)
            {
                const string deleteusercard = "DELETE FROM cardassigned WHERE DNI=@dni";
                const string selectcard = "SELECT * FROM cardassigned WHERE IDCARD=@idcard AND Type=1";
                const string insertcard = "INSERT INTO cardassigned (IDCARD,DNI,DTCreate,State,Type) VALUES (@idcard,@dni,@fechamod,1,1)";
                const string updatecard = "UPDATE cardassigned SET DNI=@dni, State=1 WHERE IDCARD=@idcard AND Type=1";

                Logger.Debug(string.Format("DELETE Empleado-Tarjeta:dni={0};", dni));
                command = ResetCommand(command, deleteusercard);
                command.Parameters.AddWithValue("@dni", dni);
                command.ExecuteNonQuery();

                command = ResetCommand(command, selectcard);
                command.Parameters.AddWithValue("@idcard", tarjeta.ToString());
                command.Parameters.AddWithValue("@dni", dni);
                exists = Exists(command);

                if (exists)
                {
                    Logger.Debug(string.Format("UPDATE Empleado-Tarjeta:dni={0};tarjeta={1};fecha={2};estado={3};tipo={4};", dni, tarjeta, fechanac, 1, 1));
                    command = ResetCommand(command, updatecard);
                    command.Parameters.AddWithValue("@dni", dni);
                    command.Parameters.AddWithValue("@idcard", tarjeta.ToString());
                    command.ExecuteNonQuery();
                }
                else
                {
                    Logger.Debug(string.Format("INSERT Empleado-Tarjeta:dni={0};tarjeta={1};fecha={2};estado={3};tipo={4};", dni, tarjeta, fechanac, 1, 1));
                    command = ResetCommand(command, insertcard);
                    command.Parameters.AddWithValue("@idcard", tarjeta.ToString());
                    command.Parameters.AddWithValue("@dni", dni);
                    command.Parameters.AddWithValue("@fechamod", fechamod);
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                const string updatecard = "UPDATE cardassigned SET State=0 WHERE DNI=@dni";

                Logger.Debug(string.Format("UPDATE Empleado-Tarjeta:dni={0};tarjeta=todas;tipo=todos;estado=0;", dni));   
                command = ResetCommand(command, updatecard);
                command.Parameters.AddWithValue("@dni", dni);
                command.ExecuteNonQuery();
            }
        }

        private MySqlCommand GetCommand(MySqlConnection connection, MySqlTransaction transaction, string script)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = script;
            return command;
        }

        private bool Exists(MySqlCommand command)
        {
            var reader = command.ExecuteReader();
            var exists = reader.Read();
            reader.Close();
            return exists;
        }

        private MySqlCommand ResetCommand(MySqlCommand command, string query)
        {
            command.CommandText = query;
            command.Parameters.Clear();
            return command;
        }
    }
}
