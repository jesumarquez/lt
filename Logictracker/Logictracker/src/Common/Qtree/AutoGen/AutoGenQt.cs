using System;
using System.Linq;
using Compumap.Core;
using Compumap.Core.Base;
using System.Collections.Generic;

namespace Urbetrack.Common.Qtree.AutoGen
{
    public class AutoGenQt
    {
        public List<NivelAutoGen> Niveles;
        //public short[] NivelPoligonal = { 1000, 2200, 2300, 1100, 1200 };
        //public short[] ValorNivel = { 1, 2, 3, 4, 5 };
        //public short[] BrushSizes = {2, 2, 2, 2, 2};
        //public int BrushSize = 1;

        public AutoGenQt()
        {
            Niveles = new List<NivelAutoGen>{
                new NivelAutoGen(1, 1200, 4, 2),
                new NivelAutoGen(2, 1100, 6, 2),
                new NivelAutoGen(3, 2300, 11, 3),
                new NivelAutoGen(3, 2200, 11, 3),
                new NivelAutoGen(4, 1000, 11, 3)
            };
        }


        public void Generate(Qtree qtree)
        {
            //var mapa = new Mapa("d:\\Mapas\\amba\\bsas.map");

            //var mapa = new Mapa("d:\\Mapas\\Chubut\\cmdo.map");
            var cartografia = new Cartografia("d:\\Mapas\\amba");
            foreach (var mapa in cartografia.Mapas)
            {
                Generate(qtree, mapa);
            }
        }

        private void Generate(Qtree qtree, Mapa mapa)
        {
            if (!IsIncludedInQtree(mapa, qtree)) return;

            for (int j = mapa.Poligonales.Count - 1; j >= 0; j--)
            {
                var poligonal = mapa.Poligonales[j];
                if (!Drawable(poligonal.Nivel)) continue;

                int[] vertices = mapa.Poligonales.GetVertices(j);

                LonLat last = null;
                bool drawNext = true;
                foreach (var vertice in vertices)
                {
                    var lonlat = new LonLat(mapa.Coords[Math.Abs(vertice)]);
                    if (last != null && drawNext)
                    {
                        var qs = qtree.MakeLeafLine(last.Longitud, last.Latitud, lonlat.Longitud, lonlat.Latitud, GetNivel(poligonal.Nivel).BrushSize);
                        foreach (var leaf in qs)
                            if (!leaf.Locked && CanOverwrite(poligonal.Nivel, leaf.Valor))
                            {
                                var latlon = qtree.GetCenterLatLon(leaf.Posicion);
                                qtree.SetValue(latlon.Latitud, latlon.Longitud, GetNivel(poligonal.Nivel).NivelQtree);
                            }
                    }
                    last = lonlat;
                    drawNext = vertice > 0;
                }
            }
        }

        private bool IsIncludedInQtree(Mapa mapa, Qtree qtree)
        {
            if(qtree.Format == QtreeFormat.Torino) return true;
            var header = mapa.Header;
            var minlat = Util.LatitudFromWorldCoord(header.YMin);
            var maxlat = Util.LatitudFromWorldCoord(header.YMax);
            var minlon = Util.LongitudFromWorldCoord(header.XMin);
            var maxlon = Util.LongitudFromWorldCoord(header.XMax);
            var bottom = Math.Min(minlat, maxlat);
            var top = Math.Max(minlat, maxlat);
            var left = Math.Min(minlon, maxlon);
            var right = Math.Max(minlon, maxlon);
            return !(top < qtree.Bottom || bottom > qtree.Top || left > qtree.Right || right < qtree.Left);
        }

        public bool CanOverwrite(int nivelPoligonal, int nivelViejo)
        {
            if(nivelViejo == 0) return true;
            foreach (var nivelAutoGen in Niveles.OrderBy(n=>n.Prioridad))
            {
                if(nivelAutoGen.NivelQtree == nivelViejo) return false;
                if(nivelAutoGen.NivelPoligonal == nivelPoligonal) return true;
            }
            return true;
        }
        public bool Drawable(int nivelPoligonal)
        {
            return Niveles.Any(n => n.NivelPoligonal == nivelPoligonal);
        }
        public NivelAutoGen GetNivel(int nivelPoligonal)
        {
            return Niveles.Where(n => n.NivelPoligonal == nivelPoligonal).FirstOrDefault();
        }
        //public int GetPrioridad(int nivel)
        //{
        //    for (int i = 0; i < NivelPoligonal.Length; i++)
        //    {
        //        if (NivelPoligonal[i] == nivel) return i;
        //    }
        //    return 9999;
        //}
        //public int GetPrioridadNivel(int nivel)
        //{
        //    for (int i = 0; i < ValorNivel.Length; i++)
        //    {
        //        if (ValorNivel[i] == nivel) return i;
        //    }
        //    return 9999;
        //}
        //public int GetNivelVia(int nivel)
        //{
        //    return ValorNivel[GetPrioridad(nivel)];
        //}
        //public int GetBrushSize(int nivel)
        //{
        //    return BrushSizes[GetPrioridad(nivel)];
        //}

        
    }
}
