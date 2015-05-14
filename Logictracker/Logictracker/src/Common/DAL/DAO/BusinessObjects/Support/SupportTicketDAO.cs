using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logictracker.Culture;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Support;
using NHibernate;
using NHibernate.Linq;
using System;

namespace Logictracker.DAL.DAO.BusinessObjects.Support
{
    public class SupportTicketDAO: GenericDAO<SupportTicket>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public SupportTicketDAO(ISession session) : base(session) { }

        #endregion

        public List<SupportTicket> GetByUsuario(params int[] idUsuario)
        {
            return Session.CreateQuery("from SupportTicket st where st.Baja = 0 and st.Usuario.Id in (:user) order by st.Fecha desc")
                .SetParameterList("user", idUsuario)
                .List<SupportTicket>() as List<SupportTicket>;
        }

        public List<SupportTicket> GetByUsuarioAndEmpresas(List<int> empresas, params int[] idUsuario)
        {
            return GetByUsuario(idUsuario).Where(st => (st.Empresa == null && empresas.Count > 1) || (st.Empresa != null && empresas.Contains(st.Empresa.Id))).ToList();
        }

        public List<SupportTicket> GetList(int empresa, Usuario user, int estado, DateTime? desde, DateTime? hasta, string filter)
        {
            var query = Session.Query<SupportTicket>().Where(st=>!st.Baja);
            if(empresa > 0)
            {
                query = query.Where(st => st.Empresa != null && st.Empresa.Id == empresa);
            }
            else if (user.Empresas.Count > 0)
            {
                var empresas = user.Empresas.OfType<Empresa>().Select(e => e.Id).ToList();
                query = query.Where(st => st.Empresa != null && empresas.Contains(st.Empresa.Id));
            }

            if (desde.HasValue) query = query.Where(t => t.Fecha >= desde.GetValueOrDefault());

            if (hasta.HasValue) query = query.Where(t => t.Fecha < hasta.GetValueOrDefault());

            var tickets = query.ToList();

            if (estado >= 0) tickets = tickets.Where(t => t.CurrentState == (short)estado).ToList();

            var content = filter.ToLower();

            if (String.IsNullOrEmpty(content)) return tickets;

            var result = new List<SupportTicket>(tickets.Count);
            foreach (var ticket in tickets)
            {
                if (ticket.Descripcion.ToLower().Contains(content) || ticket.Id.ToString() == content)
                    result.Add(ticket);
                else if (ticket.States.Cast<SupportTicketDetail>().Any(detail => detail.Descripcion != null && detail.Descripcion.ToLower().Contains(content)))
                    result.Add(ticket);
            }

            return result;
        }

        public List<SupportTicket> GetList(IEnumerable<int> empresas, IEnumerable<int> categorias, IEnumerable<int> subcategorias, IEnumerable<int> niveles, DateTime desde, DateTime hasta)
        {
            var q = Session.Query<SupportTicket>().Where(st => !st.Baja);

            if (!QueryExtensions.IncludesAll(empresas))
                q = q.FilterEmpresa(Session, empresas);
            
            if (!QueryExtensions.IncludesAll(empresas) || !QueryExtensions.IncludesAll(categorias))
                q = q.FilterCategoria(Session, empresas, categorias);

            if (!QueryExtensions.IncludesAll(empresas) || !QueryExtensions.IncludesAll(categorias) || !QueryExtensions.IncludesAll(subcategorias))
                q = q.FilterSubcategoria(Session, empresas, categorias, subcategorias);

            if (!QueryExtensions.IncludesAll(empresas) || !QueryExtensions.IncludesAll(niveles))
                q = q.FilterNivel(Session, empresas, niveles);

            q = q.Where(st => st.Fecha >= desde && st.Fecha <= hasta);

            return q.ToList();
        }

        public override void Delete(SupportTicket obj)
        {
            if (obj == null) return;

            obj.Baja = true;

            SaveOrUpdate(obj);
        }

        public List<string> GetTiposProblema()
        {
            return new List<string>
                       {
                           CultureManager.GetLabel("SUPPORT_TYPE_0_VEHICLE"),
                           CultureManager.GetLabel("SUPPORT_TYPE_1_APPLICATION")
                       };
        }

        public List<string> GetCategoriasProblemaByTipo(int tipo)
        {
            switch (tipo)
            {
                case 0:
                    return new List<string>
                               {
                                   CultureManager.GetLabel("SUPPORT_CAT_0_0_NO_REPORT"),
                                   CultureManager.GetLabel("SUPPORT_CAT_0_1_SENSOR"),
                                   CultureManager.GetLabel("SUPPORT_CAT_0_2_DISPLAY"),
                                   CultureManager.GetLabel("SUPPORT_CAT_0_3_OTHER"),
                                   CultureManager.GetLabel("SUPPORT_CAT_0_4_INSTALL"),
                                   CultureManager.GetLabel("SUPPORT_CAT_0_5_UNINSTALL"),
                                   CultureManager.GetLabel("SUPPORT_CAT_0_6_REVISION")
                               };
                case 1:
                    return new List<string>
                               {
                                   CultureManager.GetLabel("SUPPORT_CAT_1_0_NO_REPORT"),
                                   CultureManager.GetLabel("SUPPORT_CAT_1_1_EVENT_NO_POSITIONS"),
                                   CultureManager.GetLabel("SUPPORT_CAT_1_2_MODULE"),
                                   CultureManager.GetLabel("SUPPORT_CAT_1_3_OTHER")
                               };
                    default: return new List<string>(0);
            }
        }

        public List<string> GetEstados()
        {
            return new List<string>
                       {
                           CultureManager.GetLabel("SUPPORT_STATE_1_OPEN"),
                           CultureManager.GetLabel("SUPPORT_STATE_2_WORKING"),
                           CultureManager.GetLabel("SUPPORT_STATE_3_WAITING_USER"),
                           CultureManager.GetLabel("SUPPORT_STATE_4_SOLVED"),
                           CultureManager.GetLabel("SUPPORT_STATE_5_APPROVED"),
                           CultureManager.GetLabel("SUPPORT_STATE_6_REJECTED"),
                           CultureManager.GetLabel("SUPPORT_STATE_7_CLOSED"),
                           CultureManager.GetLabel("SUPPORT_STATE_8_INVALID")
                       };
        }
        
        public List<Color> GetColoresEstados()
        {
            return new List<Color>
                       {
                           Color.FromArgb(205, 92, 92),
                           Color.LightBlue,
                           Color.LightYellow,
                           Color.LightGreen,
                           Color.FromArgb(92, 205, 92),
                           Color.FromArgb(205, 92, 92),
                           Color.FromArgb(150, 205, 150),
                           Color.FromArgb(205, 150, 150)
                       };
        }

        public List<string> GetNiveles()
        {
            return new List<string>
                       {
                           CultureManager.GetLabel("SUPPORT_LEVEL_0_BLOCKING"),
                           CultureManager.GetLabel("SUPPORT_LEVEL_1_ERROR"),
                           CultureManager.GetLabel("SUPPORT_LEVEL_2_BUG"),
                           CultureManager.GetLabel("SUPPORT_LEVEL_3_SUGESTION"),
                           CultureManager.GetLabel("SUPPORT_LEVEL_4_QUESTION")
                       };
        }
    }
}
