using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Urbetrack.FleetManagment.Model;

namespace Urbetrack.FleetManagment.DataAccess
{
    public class MobilesRepository
    {
        private String query_string = "SELECT *, (SELECT p01.parenti01_fantasia FROM dbo.[par.par_enti_01_cab_empresas] as p01 WHERE p03.rela_parenti01 = p01.id_parenti01) as parenti01_fantasia, " +
                    "(SELECT p02.parenti02_descri FROM dbo.[par.par_enti_02_det_lineas] as p02 WHERE p03.rela_parenti02 = p02.id_parenti02) as parenti02_descri," +
                    "(SELECT p32.parenti32_cola_comandos FROM dbo.[par.par_enti_32_tipo_dispositivo] as p32 WHERE p08.rela_parenti32 = p32.id_parenti32) as parenti32_cola_comandos " +
                    "FROM dbo.[par.par_enti_03_cab_coches] as p03 JOIN " + 
                    "dbo.[par.par_enti_08_cab_dispositivos] as p08 ON p08.id_parenti08 = p03.rela_parenti08;"; 
        readonly List<Mobile> _mobiles = new List<Mobile>();

        public MobilesRepository()
        {
            LoadData();
        }
        /// <summary>
        /// Returns a shallow-copied list of all mobiles in the repository.
        /// </summary>
        public List<Mobile> GetMobiles()
        {
            return new List<Mobile>(_mobiles);
        }

        private void LoadData() 
        {
            _mobiles.Clear();
            using (var connection = new SqlConnection(Configuration.GetAsString("global.connection_string", "error")))
            {
                connection.Open();
                var command = new SqlCommand(query_string, connection);
                var reader = command.ExecuteReader();
            	while (reader.Read())
                {
                    var mobile = new Mobile();
                    mobile.VehicleId = Convert.ToInt32(reader.GetInt32(reader.GetOrdinal("id_parenti03")));
                    mobile.DeviceId = Convert.ToInt32(reader.GetInt32(reader.GetOrdinal("id_parenti08")));
                    mobile.Device = reader.GetString(reader.GetOrdinal("parenti08_codigo"));
                    mobile.Vehicle = reader.GetString(reader.GetOrdinal("parenti03_interno"));
                    mobile.District = reader.IsDBNull(reader.GetOrdinal("parenti01_fantasia"))
            		        		          	? "(sin distrito)"
            		        		          	: reader.GetString(reader.GetOrdinal("parenti01_fantasia"));
                    mobile.Base = reader.IsDBNull(reader.GetOrdinal("parenti02_descri"))
            		        		          	? "(sin base)"
            		        		          	: reader.GetString(reader.GetOrdinal("parenti02_descri"));
                    mobile.IMEI = reader.GetString(reader.GetOrdinal("parenti08_imei"));
                    mobile.Queue = reader.IsDBNull(reader.GetOrdinal("parenti32_cola_comandos"))
                                                ? ""
                                                : reader.GetString(reader.GetOrdinal("parenti32_cola_comandos"));
                    _mobiles.Add(mobile);
                }
            }
        }
    }
}
