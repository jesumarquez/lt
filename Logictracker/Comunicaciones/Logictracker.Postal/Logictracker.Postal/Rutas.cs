using System;
using System.Collections.Generic;
using System.Data;
using Urbetrack.Postal.DataSources;
using Urbetrack.Postal.Enums;

namespace Urbetrack.Postal
{
    public static class Rutas
    {
        private static readonly List<Ruta> _rutas = new List<Ruta>();


        public static List<Ruta> GetRutas() { return _rutas; }

        public static void AddRuta(Ruta s)
        {
            if (_rutas.Contains(s)) _rutas.Remove(s);
            _rutas.Add(s);
        }

        public static void Init()
        {
            _rutas.Clear();

            SQLiteDataSet sq = new SQLiteDataSet();
            //sq.read("select rutas.*, tipos_de_servicio.*, tipos_de_servicio.descripcion as tiposerviciodescripcion, tipos_de_servicio.descripcion_corta as tiposerviciodescripcioncorta from rutas,tipos_de_servicio where tipos_de_servicio.id = tipo_de_servicio");
            sq.read("select rutas.*, tipos_de_servicio.*, tipos_de_servicio.descripcion as tiposerviciodescripcion, tipos_de_servicio.descripcion_corta as tiposerviciodescripcioncorta, motivos.descripcion as motivodescripcion, motivos.es_entrega as motivoesentrega from rutas,tipos_de_servicio left join motivos on motivos.id = motivo where tipos_de_servicio.id = tipo_de_servicio order by numero_de_item");
            if (sq.dataSet.Tables != null)
            {
                DataTable table = sq.dataSet.Tables[0];
                foreach (DataRow row in table.Rows)
                {
                    Ruta ruta = new Ruta();
                    ruta.Id = (int) row["id"];
                    ruta.Direccion = (string) row["direccion"];
                    ruta.Estado = EstadoServicioExtenssions.GetState(Convert.ToInt32(row["estado"]));
                    ruta.Cliente = (int) row["cliente"];
                    ruta.ClienteDesc = "";
                    ruta.Destinatario = (string) row["destinatario"];
                    ruta.Pieza = (string) row["pieza"];
                    ruta.TipoServicio = (int) row["tipo_de_servicio"];
                    ruta.TipoServicioDesc = (string) row["tiposerviciodescripcion"];
                    ruta.TipoServicioDescCorta = (string) row["tiposerviciodescripcioncorta"];
                    ruta.Selected = false;
                    if (row["motivo"] != DBNull.Value)
                    {
                        var m = new Motivo();
                        m.Value = (int)row["motivo"];
                        m.EsEntrega = (bool)row["motivoesentrega"];
                        m.Text = (string)row["motivodescripcion"];
                        ruta.Motivo = m;
                    }

                    
                    if (row["lateral1"] != DBNull.Value)
                    {
                        ruta.Lateral1 = (string) row["lateral1"];
                    }                   
                    else
                    {
                        ruta.Lateral1 = "";
                    }

                    if (row["lateral2"] != DBNull.Value)
                    {
                        ruta.Lateral2 = (string)row["lateral2"];
                    }
                    else
                    {
                        ruta.Lateral2 = "";
                    }


                    if (row["referencia"] != DBNull.Value)
                    {
                        ruta.Referencia = (string)row["referencia"];
                    }
                    else
                    {
                        ruta.Referencia = "";
                    }
                    ruta.Longitud = row["longitud"] != DBNull.Value ? (double?)Convert.ToDouble(row["longitud"]) : null;
                    ruta.Latitud = row["latitud"] != DBNull.Value ? (double?)Convert.ToDouble(row["latitud"]) : null;

                    ruta.Confirma = TipoValidacionExtenssions.GetType(Convert.ToInt32(row["confirma"]));
                    ruta.ConFoto = TipoValidacionExtenssions.GetType(Convert.ToInt32(row["con_foto"]));
                    ruta.ConLaterales = TipoValidacionExtenssions.GetType(Convert.ToInt32(row["con_laterales"]));
                    ruta.ConReferencia = TipoValidacionExtenssions.GetType(Convert.ToInt32(row["con_Referencia"]));
                    ruta.ConGPS = TipoValidacionExtenssions.GetType(Convert.ToInt32(row["con_GPS"]));

                    if (row["foto"] != DBNull.Value)
                    {
                        ruta.Foto = (Byte[]) row["foto"];
                    }
                    else
                    {
                        ruta.Foto = new Byte[0];
                    }


                    ruta.CalcularEstados();
                    
                    _rutas.Add(ruta);
                    
                }

            }


            /*
            _rutas.AddRange(new List<Ruta>{ 
                new Ruta{ Id = 0,
                    Direccion="Chacabuco 1580, Capital",
                    Estado=EstadoServicio.Pendiente,
                    Cliente =1,
                    Destinatario="Juan Castro",
                    Pieza="LPA",
                    TipoServicio = 3,
                    TipoServicioDesc="Acuse con GeoFoto",
                    Lateral1="",
                    Lateral2=""
                },
                new Ruta{ Id = 1,
                    Direccion="Rivadavia 4260, Capital",
                    Estado=EstadoServicio.Pendiente,
                    Motivo = 1,
                    Cliente =1,
                    TipoServicio = 3,
                    Destinatario="Pedro Bonini",
                    Pieza="LPB",
                    TipoServicioDesc="Acuse con GeoFoto",
                    Lateral1="",
                    Lateral2="",
                    Referencia = ""
                },
                new Ruta{ Id = 2,
                    Direccion="Brasil 969, Capital",
                    Estado=EstadoServicio.Terminado,
                    Motivo = 1,
                    Cliente =1,
                    TipoServicio = 3,
                    Destinatario="Jose Alvarez Unzué", 
                    Pieza="LPC",
                    TipoServicioDesc="Acuse con GeoFoto",
                    Lateral1="967",
                    Lateral2="971",
                    Referencia = "Puerta verde"
                },
                new Ruta{ Id = 3,
                    Direccion="Mario Bravo 14, Banfield",
                    Estado=EstadoServicio.Pendiente,
                    Motivo = 1,
                    Cliente =1,
                    TipoServicio = 3,
                    Destinatario="Pablo Marmol",
                    Pieza="LPD",
                    TipoServicioDesc="Simple",
                    Lateral1="",
                    Lateral2="",
                    Referencia = ""
                }}
                );
             */
        }
    }
}
