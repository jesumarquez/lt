using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.ValueObjects.Documentos.Partes;
using Logictracker.Web.Documentos.Interfaces;
using Logictracker.Web.Documentos.ParteComparativo.Entities;

namespace Logictracker.Web.Documentos.Partes
{
    public class ParteComparativoMovilReportStrategy : IReportStrategy
    {
        private readonly DAOFactory _daoFactory;

        public ParteComparativoMovilReportStrategy(DAOFactory daoFactory) { _daoFactory = daoFactory; }

        #region IReportStrategy Members

        public IList GetData(params object[] parameters)
        {
            var pcMoviles = new List<ParteComparativoMovil>();

            var aseguradora = Convert.ToInt32(parameters[0]);
            var inicio = Convert.ToDateTime(parameters[1]);
            var fin = Convert.ToDateTime(parameters[2]);

            var user = _daoFactory.UsuarioDAO.FindById(WebSecurity.AuthenticatedUser.Id);

            var documentos = _daoFactory.DocumentoDAO.FindList(aseguradora, -1, -1, inicio, fin, 0, -1, user);

            var dicDocumentos = new Dictionary<string, List<Documento>>(documentos.Count);

            foreach (Documento documento in documentos)
            {
                var idTipoServicio = documento.Valores.ContainsKey(ParteCampos.TipoServicio)
                ? Convert.ToInt32(documento.Valores[ParteCampos.TipoServicio])
                : 0;

                var tipoServicio = ParteCampos.ListaTipoServicios[idTipoServicio];

                var equipo = _daoFactory.EquipoDAO.FindById(Convert.ToInt32(documento.Valores[ParteCampos.Equipo].ToString()));

                var key = tipoServicio == ParteCampos.ListaTipoServicios[0]
                    ? equipo.Descripcion
                    : tipoServicio;

                if(!dicDocumentos.ContainsKey(key)) dicDocumentos.Add(key, new List<Documento>());

                dicDocumentos[key].Add(documento);
            }
           
            foreach (var key in dicDocumentos.Keys)
            {
                var firstDoc = dicDocumentos[key].First();

                var idTipoServicio = firstDoc.Valores.ContainsKey(ParteCampos.TipoServicio)
                ? Convert.ToInt32(firstDoc.Valores[ParteCampos.TipoServicio])
                : 0;

                var tipoServicio = ParteCampos.ListaTipoServicios[idTipoServicio];

                var equipo = _daoFactory.EquipoDAO.FindById(Convert.ToInt32(firstDoc.Valores[ParteCampos.Equipo].ToString()));
                               
                var pcMovil = new ParteComparativoMovil
                                  {
                                      Equipo = equipo,
                                      Horas = TimeSpan.Zero,
                                      TipoServicio = tipoServicio,
                                      TipoServicioId = idTipoServicio
                                  };

                var min = DateTime.MaxValue;
                var max = DateTime.MinValue;

                if(dicDocumentos.ContainsKey(key))
                {
                    pcMovil.Partes = dicDocumentos[key].Count();

                    foreach(var documento in dicDocumentos[key])
                    {
                        pcMovil.Kilometraje += Convert.ToInt32(documento.Valores[ParteCampos.KilometrajeTotal]);

                        var col1 = documento.Valores[ParteCampos.SalidaAlPozo] as IList;

                        if (col1 == null) continue;

                        var col2 = documento.Valores[ParteCampos.LlegadaAlPozo] as IList;
                        var col3 = documento.Valores[ParteCampos.SalidaDelPozo] as IList;
                        var col4 = documento.Valores[ParteCampos.LlegadaDelPozo] as IList;
                      
                        for (var i = 0; i < col1.Count; i++)
                        {
                            var dt1 = Convert.ToDateTime(col1[i], CultureInfo.InvariantCulture);
                            var dt2 = Convert.ToDateTime(col2[i], CultureInfo.InvariantCulture);
                            var dt3 = Convert.ToDateTime(col3[i], CultureInfo.InvariantCulture);
                            var dt4 = Convert.ToDateTime(col4[i], CultureInfo.InvariantCulture);
                            var time = dt2.Subtract(dt1).Add(dt4.Subtract(dt3));

                            if(dt1 < min) min = dt1;
                            if(dt4 > max) max = dt4;

                            pcMovil.Horas = pcMovil.Horas.Add(time);
                        }
                    }
                }
                else pcMovil.Partes = 0;

                pcMoviles.Add(pcMovil);
            }

            return pcMoviles;
        }

        #endregion
    }
}