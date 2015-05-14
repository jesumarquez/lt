using System;
using System.Collections.Generic;
using System.Text;
using Urbetrack.QuadTree;
using Urbetrack.Toolkit;
using System.Linq;
using System.Xml.Linq;
using Compumap.Core;
using System.Collections;
using System.Runtime.InteropServices;
using Compumap.Core.Base;

namespace QuadTreeTest
{
    class Program
    {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Vertice
        {
            /// <summary>
            /// 1 byte, prioridad de tránsito
            /// </summary>
            [MarshalAs(UnmanagedType.I8, SizeConst = 8)]
            public long Prioridad;

            /// <summary>
            /// 1 byte, tipo 1, 2, 3 o 4. Si menor a 0, es límite y coincide con uno de otro mapa.
            /// </summary>
            [MarshalAs(UnmanagedType.I8, SizeConst = 8)]
            public long Tipo;

            /// <summary>
            /// 2 bytes, índice en la tabla de su tipo
            /// </summary>
            [MarshalAs(UnmanagedType.I2, SizeConst = 2)]
            public short NReg;
        }
        static void Main(string[] args)
        {
            Console.Write("QuadTree Tester Inicializando v 1.0\r\n");

            string basedir = @"C:\QTREE_REPO";

            var repo = new Repository();

            if (!System.IO.Directory.Exists(basedir))
            {
                Console.Write("El repositorio [{0}] no existe, lo creamos.\r\n", basedir);
                StorageGeometry sgeom = new StorageGeometry() { 
                    Signature = "0123456789ABCDEF",
                    CellBits = (char)4,
                    ActiveKey = 0x55AA55AA,
                    DataStart = 2,
                    FileSectorCount = 64,
                    Lat_OffSet = 340000000,
                    Lon_OffSet = 580000000,
                    Lat_Grid = 5000,
                    Lon_Grid = 5000,
                    Lat_GridCount = 2000,
                    Lon_GridCount = 2000
                };
                
                if (!repo.Init(basedir, sgeom))
                {
                    Console.Write("Imposible inicializar el repositorio GR2\r\n");
                    Console.ReadLine();
                    return;
                }
            }

            StorageGeometry sgeom2 = new StorageGeometry();

            if (!repo.Open(basedir, ref sgeom2))
            {
                Console.Write("El repositorio GR2 parece corrupto.\r\n");
                Console.ReadLine();
                return;
            }


            Console.WriteLine("Header Signature: {0}", sgeom2.Signature);
            Console.ReadLine();

            
            var Cartografia = new Cartografia("C:\\MapasCopia");
            foreach (var map in Cartografia)
            {
                for (int j = map.Poligonos.Count - 1; j >= 0; j--)
                {
                    var poligono = map.Poligonos[j];
                    if (poligono.Nivel != 5000) continue;
                    int[] vertices = map.Poligonos.GetVertices(j);
                    

                    for (int i = 0; i < vertices.Length; i++)
                    {
                        var vertex = vertices[i];
                        var lonlat = new LonLat(map.Coords[vertex]);
                        //6T.TRACE("Creando vertice #{0} {1} {2}", vertex, lonlat.Latitud, lonlat.Longitud);
                        repo.SetPositionClass((float)lonlat.Latitud, (float)lonlat.Longitud, 1);
                        var Gr2Cache = repo.IndexCatalog.OfType<GR2>().FirstOrDefault();
                        Gr2Cache.SetReference((float)lonlat.Latitud, (float)lonlat.Longitud, "v", map.Vertices[vertex]);
                    }
                    
                }
            }

            repo.SetPositionClass(34.5665F, 55.2235F, 12);
            int clase = repo.GetPositionClass(34.5665F, 55.2235F);
            var Gr2Cache2 = repo.IndexCatalog.OfType<GR2>().FirstOrDefault();
            var rd = Gr2Cache2.GetReference <Vertice>(34.5665F, 55.2235F, "vertices");

            Vertice ppp = new Vertice()
            {
                Tipo=22123123,Prioridad=1234,NReg=2357
            };
            Gr2Cache2.SetReference(34.5665F, 55.2235F, "vertices", ppp);
            Console.Write("sample: clase leida {0}\r\n", clase);
            Console.Write("\r\n");
            /*
            qtree.SetPositionClass(34.2665, 55.4235, 3);
            qtree.SetPositionClass(34.267, 55.4235, 4);
            qtree.SetPositionClass(34.268, 55.4235, 5);
            qtree.SetPositionClass(34.269, 55.4235, 6);
            qtree.SetPositionClass(34.270, 55.4235, 7);
            qtree.SetPositionClass(34.271, 55.4235, 8);

            clase = qtree.GetPositionClass(34.2665, 55.4235);
            Console.Write("sample: clase leida {0}\r\n", clase);
            Console.Write("\r\n");

            clase = qtree.GetPositionClass(34.5665, 55.2235);
            Console.Write("sample: clase leida {0}\r\n", clase);
            Console.Write("\r\n");

            clase = qtree.GetPositionClass(34.2665, 55.4235);
            Console.Write("sample: clase leida {0}\r\n", clase);
            Console.Write("\r\n");

            qtree.SetPositionClass(37.5665, 55.2235, 12);
            clase = qtree.GetPositionClass(37.5665, 55.2235);
            Console.Write("sample: clase leida {0}\r\n", clase);
            Console.Write("\r\n");

            qtree.SetPositionClass(37.2665, 55.4235, 3);
            clase = qtree.GetPositionClass(37.2665, 55.4235);

            Console.Write("sample: clase leida {0}\r\n", clase);
            Console.Write("\r\n");

            clase = qtree.GetPositionClass(37.5665, 55.2235);
            Console.Write("sample: clase leida {0}\r\n", clase);
            Console.Write("\r\n");

            clase = qtree.GetPositionClass(37.2665, 55.4235);
            Console.Write("sample: clase leida {0}\r\n", clase);
            Console.Write("\r\n");

            qtree.SetPositionClass(37.2665, 55.4235, 8);
            clase = qtree.GetPositionClass(37.2665, 55.4235);
            Console.Write("sample: clase leida {0}\r\n", clase);
            Console.Write("\r\n");

            qtree.SetPositionClass(34.0, 55.0, 4);
            qtree.SetPositionClass(34.99999, 55.99999, 4);

            qtree.SetPositionClass(46.5, 65.5, 5);
            qtree.SetPositionClass(47.2, 56.4, 15); */


            repo.Close();
            Console.ReadLine();
        }
    }
}
