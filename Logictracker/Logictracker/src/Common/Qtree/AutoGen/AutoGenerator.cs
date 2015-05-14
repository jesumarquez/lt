#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Compumap.Core;
using Compumap.Core.Base;
using Logictracker.Configuration;

#endregion

namespace Logictracker.Qtree.AutoGen
{
    public class AutoGenerator
    {
        public List<NivelAutoGen> Niveles;

        public AutoGenerator()
        {
            Niveles = new List<NivelAutoGen>{
                new NivelAutoGen(1, 1200, 4, 2),
                new NivelAutoGen(2, 1100, 6, 2),
                new NivelAutoGen(3, 2300, 11, 3),
                new NivelAutoGen(3, 2200, 11, 3),
                new NivelAutoGen(4, 1000, 11, 3)
            };
        }


        public void Generate(BaseQtree qtree)
        {
            var cartografia = new Cartografia(Config.Map.MapFilesDirectory);
            foreach (var mapa in cartografia.Mapas)
            {
                Generate(qtree, mapa);
            }
        }

        private void Generate(BaseQtree qtree, Mapa mapa)
        {
            if (!IsIncludedInQtree(mapa, qtree)) return;

            for (var j = mapa.Poligonales.Count - 1; j >= 0; j--)
            {
                var poligonal = mapa.Poligonales[j];
                if (!Drawable(poligonal.Nivel)) continue;

                var vertices = mapa.Poligonales.GetVertices(j);

                LonLat last = null;
                var drawNext = true;
                foreach (var vertice in vertices)
                {
                    var lonlat = new LonLat(mapa.Coords[Math.Abs(vertice)]);
                    if (last != null && drawNext)
                    {
                        var qs = qtree.MakeLeafLine(last.Longitud, last.Latitud, lonlat.Longitud, lonlat.Latitud, GetNivel(poligonal.Nivel).BrushSize);
                        for (var i = 0; i < qs.Count; i++)
                        {
                            var leaf = qs[i];
                            if (!leaf.Locked && CanOverwrite(poligonal.Nivel, leaf.Valor) &&
                                qtree.IsInsideQtree(leaf.Posicion.Latitud, leaf.Posicion.Longitud))
                            {
                                var latlon = qtree.GetCenterLatLon(leaf.Posicion);
                                qs[i] = qtree.SetValue(latlon.Latitud, latlon.Longitud, GetNivel(poligonal.Nivel).NivelQtree);
                                qtree.Commit();
                            }
                        }
                    }
                    last = lonlat;
                    drawNext = vertice > 0;
                }
            }
        }

        private static bool IsIncludedInQtree(Mapa mapa, BaseQtree qtree)
        {
            var header = mapa.Header;
            var minlat = Util.LatitudFromWorldCoord(header.YMin);
            var maxlat = Util.LatitudFromWorldCoord(header.YMax);
            var minlon = Util.LongitudFromWorldCoord(header.XMin);
            var maxlon = Util.LongitudFromWorldCoord(header.XMax);
            var bottom = Math.Min(minlat, maxlat);
            var top = Math.Max(minlat, maxlat);
            var left = Math.Min(minlon, maxlon);
            var right = Math.Max(minlon, maxlon);
            return qtree.IsInsideQtree(top, bottom, left, right);
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
            return Niveles.FirstOrDefault(n => n.NivelPoligonal == nivelPoligonal);
        }
    }
}
