using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using CompumapCom;
using MapasySistemas.VO;

namespace etao.sap2avl
{
    public class ProjImport
    {
        Nomenclador tool = new Nomenclador();
        EventLog el = null;

        public ProjImport(EventLog _el)
        {
            el = _el;
        }

        public class Record : Dictionary<string, string>
        {
            public string error;
            public double lat;
            public double lon;
            public bool nomenclada;
            public bool valid;
            public int idMapa;
            public int idPoligonal;
        }

        public bool nomenclar(Record r)
        {
            if (!r.valid)
            {
                return false;
            }
            // si esta vacio no tenemos la provincia
            if (r["state"].Trim() == "")
            {
                r.error = string.Format("NF,PROVINCIA,{0}", r["custcode"]);
                return false;
            }
            // seguimos
            if (r["ciudad"].Trim() == "")
            {
                r.error = string.Format("NF,PARTIDO,{0}", r["custcode"]);
                return false;
            }
            PoligonalVO[] calles;
            int ciudad;
            try
            {
                ciudad = Convert.ToInt32(r["ciudad"].Trim());
            }
            catch (FormatException e)
            {
                r.error = string.Format("NE,CIUDAD-FTO,{0},{1}", r["ciudad"], r["custcode"]);
                return false;
            }
            int provincia;
            try
            {
                provincia = Convert.ToInt32(r["state"].Trim());
            }
            catch (FormatException e)
            {
                r.error = string.Format("NE,PROVINCIA-FTO,{0},{1}", r["state"], r["custcode"]);
                return false;
            }
            try
            {
                // la famosa calle
                calles = tool.getPoligonalPartidoProvinciaNomenclada(r["calle"], ciudad, provincia);
            }
            catch (Exception e)
            {
                r.error = string.Format("NE,EXCEPT,{0},{1}", r["custcode"], e.Message);
                return false;
            }
            // si devuelve 0 no conidio nada pasamos a localidad
            if (calles.Length == 0)
            {
                r.error = string.Format("NE,CALLE,{0},{1}", r["calle"], r["custcode"]);
                return false;
            }
            // si devuelve + de 1 ambiguedad
            if (calles.Length > 1)
            {
                r.error = string.Format("NA,CALLE,{0},{1}", r["calle"], r["custcode"]);
                foreach (PoligonalVO calle in calles)
                {
                    r.error += "," + calle.NombreLargo;
                }
                return false;
            }
            // la loca altura
            CDireccion dir = new CDireccion();
            try
            {
                dir.Altura = Convert.ToInt32(r["altura"].Trim());
            } 
            catch (FormatException e)
            {
                r.error = string.Format("NE,ALTURA-FTO,{0},{1}", r["altura"], r["custcode"]);
                return false;
            }
            r.idMapa = dir.Nmapa = (short)calles[0].IdMapaUrbano;
            r.idPoligonal = dir.Calle1 = calles[0].IdPoligonal;
            if (dir.IsValid == 0)
            {
                r.error = "OK," + r["custcode"];
                r.lat = dir.Latitud;
                r.lon = dir.Longitud;
                // icon = 0
                // staus = 0 = activo
                // pais = "Argentina"
                // baja = false
                // fecha_alta = NOW
                // fecha_baja = NULL
                return true;
            }
            else
            {
                r.error = string.Format("NE,ALTURA,{0},{1}", r["altura"], r["custcode"]);
                return false;
            }
        }


        public bool Insert(Record r, string spname)
        {
            try
            {
                if (!r.valid)
                {
                    return false;
                }
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["connstr"]))
                {
                    conn.Open();
                    SqlCommand inserter = conn.CreateCommand();
                    inserter.CommandText = spname;
                    bool pais = false;
                    foreach (string key in r.Keys)
                    {
                        //inserter.CommandText += "@" + key + ", ";
                        if (key == "pais") pais = true;
                        inserter.Parameters.AddWithValue("@" + key, r[key].Trim());
                    }
                    if (!pais)
                    {
                        inserter.Parameters.AddWithValue("@pais", "AR");
                    }
                    //inserter.CommandText += "@idmapa, @idpoligonal, @lat, @lon, @nomenclada";
                    inserter.CommandType = CommandType.StoredProcedure;
                    inserter.Parameters.AddWithValue("@idmapa", r.idMapa);
                    inserter.Parameters.AddWithValue("@idpoligonal", r.idPoligonal);
                    inserter.Parameters.AddWithValue("@lat", r.lat);
                    inserter.Parameters.AddWithValue("@lon", r.lon);
                    inserter.Parameters.AddWithValue("@nomenclada", (r.nomenclada ? true : false));
                    //Console.Write(inserter.CommandText);
                    inserter.ExecuteReader();
                }
                return true;
            }
            catch (SqlException sqle)
            {
                r.error = String.Format("SQLE,{0},LN:{2},TEXT:{1}", r["custcode"], sqle.Message, sqle.LineNumber);
                return false;
            }
        }

/*
        void xlog(string s)
        {
            Console.Write(s);
            if (el != null) el.WriteEntry(s);
        }
*/

        public bool ImportFixed(StreamReader file, string format, StreamWriter log, string spname)
        {
            try
            {
                string[] fields = format.Split(';');
                while (!file.EndOfStream)
                {
                    string line = file.ReadLine();
                    int baseindex = 0;
                    Record r = new Record();
                    r.valid = false;
                    foreach (string fs in fields)
                    {
                        string[] f = fs.Split(',');
                        string fname = f[1];
                        int fsize = Convert.ToInt32(f[0]);
                        string value = line.Substring(baseindex, fsize);
                        baseindex += fsize;
                        if (fname != "ignore")
                        {
                            int outint;
                            int.TryParse(value.Trim(), out outint);
                            if (fname == "valid" && outint == 1)
                            {
                                r.valid = true;
                                r.Add(fname, value);
                            }
                            else
                            {
                                r.Add(fname, value);
                            }
                        }
                    }
                    r.error = "";
                    r.nomenclada = nomenclar(r);

                    if (!r.nomenclada)
                    {
                        if (r.error != "") {
                            log.WriteLine(string.Format("{0},{1}", DateTime.Now, r.error));
                        }
                        if (!Insert(r, spname))
                        {
                            if (r.error != "")
                            {
                                log.WriteLine(string.Format("{0},{1}", DateTime.Now, r.error));
                            }
                        }
                        else
                        {
                            log.WriteLine(string.Format("{0},I-OK,{1}", DateTime.Now, r["custcode"]));
                        }
                    }
                    else
                    {
                        if (!Insert(r, spname))
                        {
                            if (r.error != "")
                            {
                                log.WriteLine(string.Format("{0},{1}", DateTime.Now, r.error));
                            }
                        }
                        else
                        {
                            log.WriteLine(string.Format("{0},NI-OK,{1}", DateTime.Now, r["custcode"]));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.WriteLine(String.Format("Excepcion procesando archivo:{0}", e.ToString()));
                return false;
            }
            return true;
        }

        public EventLog El
        {
            get { return el; }
        }
    }
}
