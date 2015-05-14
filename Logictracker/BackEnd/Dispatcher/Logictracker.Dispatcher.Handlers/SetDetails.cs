using System.Linq;
using Logictracker.AVL.Messages;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Dispatcher.Core;
using Logictracker.Messages.Sender;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Types.BusinessObjects.Dispositivos;
using System;

namespace Logictracker.Dispatcher.Handlers
{
    [FrameworkElement(XName = "SetDetailHandler", IsContainer = false)]
    public class SetDetails : DeviceBaseHandler<SetDetail>
    {
        #region Protected Methods

        protected override HandleResults OnDeviceHandleMessage(SetDetail message)
        {
			STrace.Debug(GetType().FullName, message.DeviceId, String.Format("SetDetail Processing: Name={0} Value={1}", message.GetUserSetting(SetDetail.Fields.Name), message.GetUserSetting(SetDetail.Fields.Value)));
			if (String.IsNullOrEmpty(message.GetUserSetting(SetDetail.Fields.Name))) return HandleResults.SilentlyDiscarded;

            DaoFactory.Session.Refresh(Dispositivo);

            var userSettingName = message.GetUserSetting(SetDetail.Fields.Name);
            var userSettingValue = message.GetUserSetting(SetDetail.Fields.Value);
			if (userSettingName == "Telephone")
			{
                Dispositivo.Telefono = userSettingValue ?? String.Empty;
                DaoFactory.DispositivoDAO.SaveOrUpdate(Dispositivo);
				return HandleResults.Success;
			}

            if (userSettingName == "Port")
            {
                try
                {
                    Dispositivo.Port = Convert.ToInt32(userSettingValue ?? "0");
                    DaoFactory.DispositivoDAO.SaveOrUpdate(Dispositivo);
                    return HandleResults.Success;
                } 
                catch (Exception)
                {
                    return HandleResults.SilentlyDiscarded;
                }
            }

            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    var name = (userSettingName ?? String.Empty).ToLower();
                    var consumidor = message.GetUserSetting(SetDetail.Fields.Consumidor) ?? String.Empty;
                    var editable = message.GetUserSetting(SetDetail.Fields.Editable) == "true";
                    var metadata = message.GetUserSetting(SetDetail.Fields.Metadata) ?? String.Empty;
                    var reset = message.GetUserSetting(SetDetail.Fields.RequiereReset) == "true";
                    var tipo = message.GetUserSetting(SetDetail.Fields.TipoDato) ?? String.Empty;
                    var valorinicial = message.GetUserSetting(SetDetail.Fields.ValorInicial) ?? String.Empty;
                    var valor = userSettingValue ?? String.Empty;

                    var tipoDispositivo = Dispositivo.TipoDispositivo;
                    var parametros = tipoDispositivo.TiposParametro.Cast<TipoParametroDispositivo>();
                    var parametro = parametros.FirstOrDefault(p => p.Nombre.ToLower() == name);
                    if (parametro == null)
                    {
                        parametro = new TipoParametroDispositivo
                                    {
                                        Consumidor = consumidor,
                                        Nombre = name,
                                        Descripcion = name,
                                        DispositivoTipo = tipoDispositivo,
                                        Editable = editable,
                                        Metadata = metadata,
                                        RequiereReset = reset,
                                        TipoDato = tipo,
                                        ValorInicial = valorinicial,
                                    };

                        tipoDispositivo.TiposParametro.Add(parametro);

                        DaoFactory.TipoDispositivoDAO.SaveOrUpdate(tipoDispositivo);
                    }

                    var detalles = Dispositivo.DetallesDispositivo.Cast<DetalleDispositivo>();
                    var detalle = detalles.FirstOrDefault(d => d.TipoParametro.Nombre.ToLower() == name);
                    if (detalle == null)
                    {
                        detalle = new DetalleDispositivo {Dispositivo = Dispositivo, Revision = 0, TipoParametro = parametro};

                        Dispositivo.AddDetalleDispositivo(detalle);
                    }

                    var changed = detalle.Valor != valor;
                    detalle.Valor = valor;
                    DaoFactory.DispositivoDAO.SaveOrUpdate(Dispositivo);
                    transaction.Commit();
                    if (changed && parametro.RequiereReset)
                    {
                        try
                        {
                            MessageSender.CreateReboot(Dispositivo, null).Send();
                        }
                        catch (Exception ex)
                        {
                            STrace.Exception(GetType().FullName, ex, Dispositivo.Id,
                                String.Format("No se pudo enviar el comando Reboot al Dispositivo {0}", Dispositivo.Id));
                        }
                    }

                }
                catch (Exception ex)
                {                    
                    STrace.Exception(GetType().FullName, ex, message.DeviceId,
                        String.Format("SetDetail Exception: Name={0} Value={1}", message.GetUserSetting(SetDetail.Fields.Name),
                            message.GetUserSetting(SetDetail.Fields.Value)));
                    transaction.Rollback();
                    throw ex;
                }
            }
            return HandleResults.Success;
        }

        #endregion
    }
}
