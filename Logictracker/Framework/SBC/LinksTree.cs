#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Model.IAgent;
using Logictracker.Utils;

#endregion

namespace Logictracker.Layers
{
	[FrameworkElement(XName = "LinksTree", IsContainer = false)]
    public class LinksTree : FrameworkElement, IService
    {
		#region Attributes

		/// <summary>
		/// Obtiene o Establece el tiempo (en segundos) de inactividad en 
		/// una sesion para cambiar su estado a Expired;
		/// </summary>
		[ElementAttribute(XName = "LinkAgeTimeout", DefaultValue = 300)]
		public int LinkAgeTimeout { get; set; }

		/// <summary>
		/// Obtiene o Establece el tiempo (en segundos) de inactividad en 
		/// una sesion para eliminarla del mapa.
		/// </summary>
		[ElementAttribute(XName = "LinkPurgeTimeout", DefaultValue = 360)]
		public int LinkPurgeTimeout { get; set; }
		
		#endregion

		#region Properties

		private readonly Dictionary<int, ILink> links_table = new Dictionary<int, ILink>();
		private readonly ReaderWriterLockSlim rwlock = new ReaderWriterLockSlim();
		private LinksTask maintainance;
		
        ///<summary>
        /// Delegado del evento disparado cuando una <c>Link</c> expira.
        ///</summary>
        ///<param name="link">Session que paso a inactiva</param>
        public delegate void LinkExpiredDelegate(ILink link);

#pragma warning disable 67
        /// <summary>
        /// Se dispara cuando una ruta en estado Active, supera la edad maxima 
        /// establecidad en <c>LinkAgeTimeout</c>
        /// </summary>
        public event LinkExpiredDelegate LinkExpired;
#pragma warning restore 67

		#endregion

		#region IService

		public bool ServiceStart()
		{
			maintainance = new LinksTask(this);
			maintainance.Start();
			return true;
		}

		public bool ServiceStop()
		{
			maintainance.Stop();
			maintainance = null;
			return true;
		}
		
		#endregion

		#region Public Methods

		public ILink Find(EndPoint ep, IUnderlayingNetworkLayer unl)
		{
			rwlock.EnterReadLock();
			try
			{
				var link = links_table
					.Where(route => ep.Equals(route.Value.EndPoint)) // el operador == siempre da falso por que solo compara la referencia y son 2 instancias distintas con los mismos datos!
					.Select(route => route.Value)
					.FirstOrDefault();

				if (link != null)
				{
					link.UnderlayingNetworkLayer = unl;
					link.Age = DateTime.Now;
					//link.State = LinkStates.Active;
				}

				return link;
			}
			finally
			{
				rwlock.ExitReadLock();
			}
		}

		public ILink GetOrCreate(INode Device, IUnderlayingNetworkLayer unl, EndPoint ep, int formerEndPointId)
		{
			rwlock.EnterReadLock();
			try
			{
				if (links_table.ContainsKey(Device.Id))
				{
					var link = links_table[Device.Id];
					if (Device.Id == formerEndPointId)
					{
						link.EndPoint = ep;
					}
					return link;
				}
			}
			finally
			{
				rwlock.ExitReadLock();
			}

            rwlock.EnterWriteLock();
			try
			{
				if (links_table.ContainsKey(Device.Id))
				{
					var link = links_table[Device.Id];
					if (Device.Id == formerEndPointId)
					{
						link.EndPoint = ep;
					}
					return link;
				}
				else
				{
					//limpiar otras referencias al endpoint
					var linkedendpoints = links_table
						.Where(route => ep.Equals(route.Value.EndPoint)) // el operador == siempre da falso por que solo compara la referencia y son 2 instancias distintas con los mismos datos!
						.Select(route => route.Value)
						.ToList();
					foreach (var lep in linkedendpoints)
					{
						lep.EndPoint = null;
					}

					//crear el nuevo link para devolver
					var link = new Link
						{
							Age = DateTime.Now,
							State = LinkStates.New,
							Device = Device,
							UnderlayingNetworkLayer = unl,
							EndPoint = ep,
						};
					links_table.Add(Device.Id, link);
					return link;
				}
			}
			finally
			{
				rwlock.ExitWriteLock();
			}
		}

		public ILink Get(int DeviceId)
		{
			rwlock.EnterReadLock();

		    ILink link;
            
            try
            {
                link = links_table
                    .Where(route => route.Value.Device.GetDeviceId() == DeviceId)
                    .Select(route => route.Value)
                    .FirstOrDefault();
            }finally
            {
                rwlock.ExitReadLock();
            }
		    
			if (link != null)
			{
				link.Age = DateTime.Now;
				//link.State = LinkStates.Active;
			}
			return link;
		}
		
		#endregion

		#region Private Members

		private class LinksTask : Task
		{
			private readonly LinksTree Parent;
			public LinksTask(LinksTree parent) : base("Links Table")
			{
				Parent = parent;
			}

			protected override int DoWork(ulong tick)
			{
				var toDelete = new List<int>();
				
                Parent.rwlock.EnterReadLock();
                try
                {
                    // se ejecuta cada 60 ciclos osea cada 5 minutos.
                    if ((tick % 60) == 0)
                    {
                        toDelete.AddRange(Parent.links_table
                            .Where(link => link.Value.State == LinkStates.Expired)
                            .Where(link => Math.Abs(DateTime.Now.Subtract(link.Value.Age).TotalSeconds) > Parent.LinkPurgeTimeout)
                            .Select(link => link.Value.Device.GetDeviceId()));
                    }

                    foreach (var link in Parent.links_table.Values
                        .Where(link => link.State == LinkStates.Active)
                        .Where(link => Math.Abs(DateTime.Now.Subtract(link.Age).TotalSeconds) > Parent.LinkAgeTimeout))
                    {
                        link.State = LinkStates.Expired;
                        //to_expire.Add(link);
                    }
                }
                finally
                {
                    Parent.rwlock.ExitReadLock();
                }

			    Parent.rwlock.EnterWriteLock();
                try
                {
                    foreach (var pc in toDelete)
                    {
                        Parent.links_table.Remove(pc);
                    }
                }
                finally
                {
                    Parent.rwlock.ExitWriteLock();
                }

			    return 5000; // ejecuta cada 5 segundos
			}
		}

		#endregion
    }
}