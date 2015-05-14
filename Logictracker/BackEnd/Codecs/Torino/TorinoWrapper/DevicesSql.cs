#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Urbetrack.Comm.Core.Transaction;
using Urbetrack.Configuration;
using Urbetrack.Utils;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Torino;
using FleetDevices = Urbetrack.Comm.Core.Fleet.Devices;

#endregion


namespace Urbetrack.Gateway.Joint.MessageQueue
{
    public static class DevicesSql
    {
        public delegate void InitializeProgressHandler(int total_steps, int done_steps);

        public static event InitializeProgressHandler InitializeProgress;

        public static string ConnectionString { get; set; }
        public static string FirmwarePath { get; set; }
        public static TransactionUser TransactionUser { get; set; }

        static DevicesSql()
        {
            ConnectionString = Config.Torino.ConnectionString;
            FirmwarePath = Process.GetApplicationFolder("temp");
            STrace.Debug(typeof(DevicesSql).FullName,"Preparando Administrador de Dispositivos...");
            FleetDevices.I().RetrieveDevices += RetrieveDevices;
            FleetDevices.I().DeviceUpdate += DeviceUpdate;
            FleetDevices.I().FirmwareRequest += FirmwareRequest;
            FleetDevices.I().RetrieveMessages += RetrieveMessages;
            FleetDevices.I().RetrieveRiders += RetrieveRiders;
        }

        public static void Initialize() {}

        public static string FirmwareRequest(object sender, int firm_id)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                	var cmd = string.Format("SELECT * FROM fleet03_firmware WHERE id_parenti24 = {0};", firm_id);

                	var command = new SqlCommand(cmd, connection);
                	connection.Open();
                	var reader = command.ExecuteReader();
                	if (reader.Read())
                	{
                		var signature = reader.GetString(reader.GetOrdinal("parenti24_firma"));
                		var filename = string.Format(@"{0}\{1}.bin", FirmwarePath, signature);
                		if (File.Exists(filename)) return filename;
						var b = new byte[512];
						var size = reader.GetBytes(reader.GetOrdinal("parenti24_binario"), 0, b, 0, 0);
                		using (var fw = File.Create(filename))
                		{
                			var offset = 0;
                			var count = reader.GetBytes(reader.GetOrdinal("parenti24_binario"), offset, b, 0, 512);
                			var broken = false;
                			while (count > 0 && size > 0)
                			{
                				size -= count;
                				offset += (int) count;
                				if (broken) return "db_error";
                				if (count < 512)
                				{
                					for (var i = count - 1; i < 512; i++)
                					{
                						b[i] = 0xFF;
                					}
                					broken = true;
                				}
                				fw.Write(b, 0, 512);
                				var read = 512;
                				if (size < 512) read = (int) size;
                				count = reader.GetBytes(reader.GetOrdinal("parenti24_binario"), offset, b, 0, read);
                			}
                		}
                		return filename;
                	}
                }
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(DevicesSql).FullName,e);
            }
            return "error";
        }

        private static void StoreParameter(Device d, Device.Parameter param, SqlConnection connection)
        {
            try
            {
                var command = new SqlCommand("sp_set_device_parameter", connection)
                                  {
                                      CommandType = CommandType.StoredProcedure
                                  };

                var retur_value = command.Parameters.Add("RETURN_VALUE", SqlDbType.Int);
                retur_value.Direction = ParameterDirection.ReturnValue;

                command.Parameters.AddWithValue("@p1_device_id", d.Id_short);
                command.Parameters.AddWithValue("@p2_parameter", param.Nombre);
                command.Parameters.AddWithValue("@p3_value", param.Valor);
                command.Parameters.AddWithValue("@p4_consumer", param.Consumidor);
                command.Parameters.AddWithValue("@p5_data_type", param.TipoDato);
                command.Parameters.AddWithValue("@p6_valor_inicial", param.ValorInicial);
                command.Parameters.AddWithValue("@p7_editable", 0);

                var rows = command.ExecuteNonQuery();

                STrace.Debug(typeof(DevicesSql).FullName, String.Format("URBETRACK: set_parameter({0},{1}) rows:{2}", param.Nombre, param.Valor, rows));
            }
            catch (Exception e)
            {
                e.Data.Add("Parameter:", param.Nombre);
                throw;
            }
        }

        public static Device DeviceUpdate(object sender, Device d)
        {
            try
            {
                if (d.UpdateRequired)
                {
                    using (var connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();
                        foreach (var param in d.Parameters.Values)
                        {
                            if (!param.UpdateRequired) continue;
                            StoreParameter(d, param, connection);
                            param.UpdateRequired = false;
                        }
                        d.UpdateRequired = false;
                    }
                }

                using (var connection = new SqlConnection(ConnectionString))
                {
                	var cmd = string.Format("SELECT * FROM fleet01_devices WHERE id_parenti08 = {0}", d.Id_short);

                	var command = new SqlCommand(cmd, connection);
                	connection.Open();
                	var reader = command.ExecuteReader();
                	if (reader.Read())
                	{
                		var rd = new Device(TransactionUser)
                		         	{
                		         		Id_short = Convert.ToInt16(reader.GetInt32(reader.GetOrdinal("id_parenti08"))),
                		         		Base = reader.IsDBNull(reader.GetOrdinal("parenti02_descri"))
                		         		       	? "(sin vehiculo)"
                		         		       	: reader.GetString(reader.GetOrdinal("parenti02_descri")),
                		         		Vehicle = reader.IsDBNull(reader.GetOrdinal("parenti03_interno"))
                		         		          	? "(sin vehiculo)"
                		         		          	: reader.GetString(reader.GetOrdinal("parenti03_interno")),
                		         		LegacyCode = reader.GetString(reader.GetOrdinal("parenti08_codigo")),
                		         		Imei = reader.GetString(reader.GetOrdinal("parenti08_imei")),
                		         		FirmaRequerida = reader.IsDBNull(reader.GetOrdinal("parenti24_firma"))
                		         		                 	? ""
                		         		                 	: reader.GetString(reader.GetOrdinal("parenti24_firma")),
                		         		FirmwareId =
                		         			reader.IsDBNull(reader.GetOrdinal("rela_parenti24"))
                		         				? 0
                		         				: Convert.ToInt32(
                		         					reader.GetInt32(reader.GetOrdinal("rela_parenti24")))
                		         	};
                		reader.Close();
                		var pcmd = new SqlCommand(String.Format("SELECT * FROM fleet02_parameters where device_id = {0};", d.Id_short), connection);
                		reader = pcmd.ExecuteReader();

                		while (reader.Read())
                		{
                			var p = new Device.Parameter
                			        	{
                			        		Consumidor = reader.GetString(reader.GetOrdinal("parenti31_consumidor"))[0],
                			        		TipoDato = reader.GetString(reader.GetOrdinal("parenti31_tipo_dato")),
                			        		Nombre = reader.GetString(reader.GetOrdinal("parenti31_nombre")),
                			        		Revision = reader.GetInt32(reader.GetOrdinal("parenti30_revision")),
                			        		Valor = reader.GetString(reader.GetOrdinal("parenti30_valor")),
                			        		ValorInicial = reader.GetString(reader.GetOrdinal("parenti31_valor_inicial"))
                			        	};
                			rd.Parameters.Add(p.Nombre, p);
                		}
                		reader.Close();

                		return rd;
                	}
                }
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(DevicesSql).FullName, e);
            }
            return null;
        }

        private static int CountQuery(SqlConnection connection, SqlCommand command)
        {
            var query = command.CommandText;
            var counter = new SqlCommand("select count(*) as total FROM (" + query + ") as source", connection);
            var reader = counter.ExecuteReader();
            reader.Read();
            var result = reader.GetInt32(reader.GetOrdinal("total"));
            reader.Close();
            return result;
        }

        public static Dictionary<int, Device> RetrieveDevices()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
            	var devices = new Dictionary<int, Device>();
            	connection.Open();
            	var command = new SqlCommand("SELECT * FROM fleet01_devices", connection);
            	var reader = command.ExecuteReader();
            	var c = 0;
            	while (reader.Read())
            	{
            		var d = new Device(TransactionUser)
            		        	{
            		        		Id_short = Convert.ToInt16(reader.GetInt32(reader.GetOrdinal("id_parenti08"))),
            		        		Base = reader.IsDBNull(reader.GetOrdinal("parenti02_descri"))
            		        		       	? "(sin vehiculo)"
            		        		       	: reader.GetString(reader.GetOrdinal("parenti02_descri")),
            		        		Vehicle = reader.IsDBNull(reader.GetOrdinal("parenti03_interno"))
            		        		          	? "(sin vehiculo)"
            		        		          	: reader.GetString(reader.GetOrdinal("parenti03_interno")),
            		        		LegacyCode = reader.GetString(reader.GetOrdinal("parenti08_codigo")),
            		        		Imei = reader.GetString(reader.GetOrdinal("parenti08_imei")),
            		        		FirmaRequerida = reader.IsDBNull(reader.GetOrdinal("parenti24_firma"))
            		        		                 	? ""
            		        		                 	: reader.GetString(reader.GetOrdinal("parenti24_firma")),
            		        		FirmwareId =
            		        			reader.IsDBNull(reader.GetOrdinal("rela_parenti24"))
            		        				? 0
            		        				: Convert.ToInt32(
            		        					reader.GetInt32(reader.GetOrdinal("rela_parenti24")))
            		        	};

            		STrace.Debug(typeof (DevicesSql).FullName, String.Format("DB.DEVICES: agregando Device[{0}] IMEI={1}", d.LogId, d.Imei));
            		devices.Add(d.Id_short, d);
            		c++;
            		if (c%100 == 0)
            		{
            			STrace.Debug(typeof (DevicesSql).FullName, String.Format("DB.DEVICES: {0} dispositivos recuperados.", c));
            		}
            	}
            	reader.Close();
            	c = 0;

            	if (Config.Torino.DisableParameters)
            	{
            		STrace.Debug(typeof (DevicesSql).FullName, "DB.DEVICES: parametros deshabilitados.");
            		return devices;
            	}

            	var cmd = new SqlCommand("SELECT * FROM fleet02_parameters", connection);

            	var total = CountQuery(connection, cmd);
            	STrace.Debug(typeof (DevicesSql).FullName, String.Format("DB.DEVICES: parametrizando {0} dispositivos, total de parametros {1}", devices.Count, total));
            	if (InitializeProgress != null)
            	{
            		InitializeProgress(total, c);
            	}
            	reader = cmd.ExecuteReader();
            	while (reader.Read())
            	{
            		c++;
            		var dev_id = reader.GetInt32(reader.GetOrdinal("device_id"));
            		if (!devices.ContainsKey(dev_id)) continue;
            		var dev = devices[dev_id];
            		var p = new Device.Parameter
            		        	{
            		        		Consumidor = reader.GetString(reader.GetOrdinal("parenti31_consumidor"))[0],
            		        		TipoDato = reader.GetString(reader.GetOrdinal("parenti31_tipo_dato")),
            		        		Nombre = reader.GetString(reader.GetOrdinal("parenti31_nombre")),
            		        		Revision = reader.GetInt32(reader.GetOrdinal("parenti30_revision")),
            		        		Valor = reader.GetString(reader.GetOrdinal("parenti30_valor")),
            		        		ValorInicial = reader.GetString(reader.GetOrdinal("parenti31_valor_inicial"))
            		        	};
					if (!dev.Parameters.ContainsKey(p.Nombre))
            			dev.Parameters.Add(p.Nombre, p);
					else if (p.Revision > dev.Parameters[p.Nombre].Revision)
						dev.Parameters[p.Nombre] = p;

					if (total > 24 && c%(total/25) == 0)
            		{
            			if (InitializeProgress != null)
            			{
            				InitializeProgress(total, c);
            			}
            		}
            		if (c%10000 == 0)
            		{
            			STrace.Debug(typeof (DevicesSql).FullName, String.Format("DB.DEVICES: {0} parametros cargados.", c));
            		}
            	}
            	if (InitializeProgress != null)
            	{
            		InitializeProgress(0, 0);
            	}
            	return devices;
            }
        }

        public static List<Device.Message> RetrieveMessages(Device d)
        {
            if (!d.SupportsMessages) return null;
            using (var connection = new SqlConnection(ConnectionString))
            {
            	var messages = new List<Device.Message>();
            	connection.Open();
            	var command = new SqlCommand(String.Format("select * from fleet05_messages where organization_base_descri = '{0}';", d.Base), connection);
            	var reader = command.ExecuteReader();
            	while (reader.Read())
            	{
            		var msg = new Device.Message
            		          	{
            		          		Code = reader.GetInt16(reader.GetOrdinal("code")),
            		          		Text = reader.GetString(reader.GetOrdinal("message")),
            		          		Revision = reader.GetInt32(reader.GetOrdinal("revision")),
            		          		Source = reader.GetString(reader.GetOrdinal("source"))[0],
            		          		Destination = reader.GetString(reader.GetOrdinal("destination"))[0],
            		          		Deleted = reader.GetBoolean(reader.GetOrdinal("state"))
            		          	};
            		messages.Add(msg);
            	}
            	return messages;
            }
        }

        public static Dictionary<string, Device.Rider> RetrieveRiders()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
            	var riders = new Dictionary<string, Device.Rider>();
            	connection.Open();
            	var command = new SqlCommand("select * from fleet06_staff;", connection);
            	var reader = command.ExecuteReader();
            	while (reader.Read())
            	{
            		var msg = new Device.Rider
            		          	{
            		          		Revision = reader.GetInt32(reader.GetOrdinal("revision")),
            		          		Identifier = reader.GetString(reader.GetOrdinal("identifier")),
            		          		Description = reader.GetString(reader.GetOrdinal("description")),
            		          		File = reader.IsDBNull(reader.GetOrdinal("legajo"))
            		          		       	? "(desconocido)"
            		          		       	: reader.GetString(reader.GetOrdinal("legajo"))
            		          	};
            		riders.Add(msg.Identifier, msg);
            	}
            	return riders;
            }
        }
        
    }
}