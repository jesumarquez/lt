using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects;
using Logictracker.Utils;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class EstadoDAO : GenericDAO<Estado>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public EstadoDAO(ISession session) : base(session) { }

        #endregion

        public Estado FindByCodigo(int linea, int codigo)
        {
            return Session.Query<Estado>()
				.Where(e => e.Linea.Id == linea && e.Codigo == codigo)
                .SafeFirstOrDefault();
        }

        public List<Estado> GetByPlanta(int linea)
        {
            return Session.Query<Estado>().Where(e => e.Linea.Id == linea)
                        .OrderBy(e=>e.Orden)
                        .ToList();
        }

        public Estado GetUltimoByPlanta(int linea)
        {
            return Session.Query<Estado>().Where(e => e.Linea.Id == linea)
                        .OrderByDescending(e => e.Orden)
                        .FirstOrDefault();
        }

        public void DuplicateEstados(List<Int32> ids, IList idsLineas)
        {
            var estados = new List<Estado>(ids.Count);
            var lineas = new List<Linea>(idsLineas.Count);

            estados.AddRange(ids.Select(id => FindById(id)));

            var lineaDao = new LineaDAO();

            lineas.AddRange(from int id in idsLineas select lineaDao.FindById(id));

            var mensajeDao = new MensajeDAO();

            foreach (var est in lineas.SelectMany(linea =>
                (from estado in estados
                 where !Exists(estado.Codigo, estado.Descripcion, linea.Id) && mensajeDao.Exists(estado.Mensaje.Codigo, linea.Empresa, linea)
                 select new Estado
                    {
                        Codigo = estado.Codigo,
                        Deltatime = estado.Deltatime,
                        Descripcion = estado.Descripcion,
                        EsPuntoDeControl = estado.EsPuntoDeControl,
                        Icono = estado.Icono,
                        Informar = estado.Informar,
                        Mensaje = mensajeDao.FindById(mensajeDao.GetByCodigo(estado.Mensaje.Codigo, linea.Empresa, linea).Id), Modo = estado.Modo, Orden = estado.Orden, Linea = linea
                    })))
                SaveOrUpdate(est);
        }

        private bool Exists(int codigo, string nombre, int linea)
        {
            return Session.Query<Estado>().Any(e => e.Linea.Id == linea && e.Codigo == codigo && e.Descripcion == nombre);
        }
    }
}