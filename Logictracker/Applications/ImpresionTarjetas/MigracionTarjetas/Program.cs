using System.Data.SqlClient;
using System.Data.SqlServerCe;

namespace MigracionTarjetas
{
    class Program
    {
        static void Main(string[] args)
        {
            var sqlConn = new SqlConnection("Server=192.168.10.179;Initial Catalog=tarjetas;User ID=tarjetas;Password=tarjetas;");
            //var ceConn = new SqlCeConnection("Data Source=|DataDirectory|\\App_Data\\tarjetas.sdf;Persist Security Info=True;Encrypt Database=True;Password=tar!123*;File Mode=shared read;");
            var ceConn = new SqlCeConnection("Data Source=D:\\Proyectos\\Logictracker\\Source\\git\\Logictracker\\Applications\\ImpresionTarjetas\\ImpresionTarjetas\\bin\\Debug\\tarjetas.sdf;Persist Security Info=True;Encrypt Database=True;Password=tar!123*;File Mode=shared read;");

            sqlConn.Open();
            ceConn.Open();

            var cmd = sqlConn.CreateCommand();
            cmd.CommandText = @"select legajo, apellido, nombre, documento, puesto, upcode, foto, code, alta, editado, impreso
                                from empleados 
                                where legajo not like 'C%'
                                or legajo not like 'G%'
                                or legajo not like 'B%'
                                or legajo not like '3er%'
                                or legajo not like 'Taller%'
                                or legajo not like 'Visita%'";

            var upcmd = ceConn.CreateCommand();
            //upcmd.CommandText = "INSERT INTO upcode (code, image, used) VALUES (@c,@i,@u)";
            upcmd.CommandText = "UPDATE upcode set used = 1 where code = @c";

            var empcmd = ceConn.CreateCommand();
            empcmd.CommandText = @"INSERT INTO [empleados]
           ([legajo]
           ,[apellido]
           ,[nombre]
           ,[documento]
           ,[puesto]
           ,[upcode]
           ,[foto]
           ,[code]
           ,[alta]
           ,[editado]
           ,[impreso]
           ,[empresa])
     VALUES
           (@legajo
           ,@apellido
           ,@nombre
           ,@documento
           ,@puesto
           ,@upcode
           ,@foto
           ,@code
           ,@alta
           ,@editado
           ,@impreso
            ,2)";



            var reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                upcmd.Parameters.Clear();
                upcmd.Parameters.AddWithValue("@c", reader.GetValue(5));
                //upcmd.Parameters.AddWithValue("@i", reader.GetValue(7));
                //upcmd.Parameters.AddWithValue("@u", true);

                upcmd.ExecuteNonQuery();

                empcmd.Parameters.Clear();
                empcmd.Parameters.AddWithValue("@legajo", reader.GetValue(0));
                empcmd.Parameters.AddWithValue("@apellido", reader.GetValue(1));
               empcmd.Parameters.AddWithValue("@nombre", reader.GetValue(2));
               empcmd.Parameters.AddWithValue("@documento", reader.GetValue(3));
               empcmd.Parameters.AddWithValue("@puesto", reader.GetValue(4));
               empcmd.Parameters.AddWithValue("@upcode", reader.GetValue(5));
               empcmd.Parameters.AddWithValue("@foto", reader.GetValue(6));
               empcmd.Parameters.AddWithValue("@code", reader.GetValue(7));
               empcmd.Parameters.AddWithValue("@alta", reader.GetValue(8));
               empcmd.Parameters.AddWithValue("@editado", reader.GetValue(9));
               empcmd.Parameters.AddWithValue("@impreso", reader.GetValue(10));

                empcmd.ExecuteNonQuery();

            }
            reader.Close();

            sqlConn.Close();
            ceConn.Close();

        }
    }
}
