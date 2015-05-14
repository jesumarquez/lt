using System;
using Logictracker.DatabaseTracer.Core;

namespace Logictracker.Model.IAgent
{
    public interface IService
    {
        /// <summary>
        /// Metodo invocado al servicio, que debe iniciar su operacion ya que 
        /// todas las dependencias estan iniciadas.
        /// </summary>
        /// <returns>True si pudo iniciar</returns>
        bool ServiceStart();

        /// <summary>
        /// Se notifica al servicio que termine su operacion.
        /// </summary>
        /// <returns>True si pudo detener.</returns>
        bool ServiceStop();

	    String GetName();
	    String GetStaticKey();
    }

    public static class IServiceX
    {
		public static void SafeServiceStart(this IService me)
	    {
			try
			{
				if (!me.ServiceStart())
				{
					STrace.Error(me.GetType().FullName, String.Format("Error on ServiceStart() on XmlElement: <{0} x:Key={1} />", me.GetName(), me.GetStaticKey() ?? "null"));
				}
			}
			catch (Exception e)
			{
				STrace.Exception(me.GetType().FullName, e, String.Format("Exception on ServiceStart() on XmlElement: <{0} x:Key={1} />", me.GetName(), me.GetStaticKey() ?? "null"));
			}
	    }

		public static void SafeServiceStop(this IService me)
	    {
			try
			{
				if (!me.ServiceStop())
				{
					STrace.Error(me.GetType().FullName, String.Format("Error on ServiceStop() on XmlElement: <{0} x:Key={1} />", me.GetName(), me.GetStaticKey() ?? "null"));
				}
			}
			catch (Exception e)
			{
				STrace.Exception(me.GetType().FullName, e, String.Format("Exception on ServiceStop() on XmlElement: <{0} x:Key={1} />", me.GetName(), me.GetStaticKey() ?? "null"));
			}
	    }
    }
}
