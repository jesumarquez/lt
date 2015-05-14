using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Model;
using Logictracker.Utils;

namespace Logictracker.Layers.DeviceCommandCodecs
{
    public enum DeviceCommandResponseStatus
    {
        Valid,
        Invalid,
        Exception
    }

    public abstract class BaseDeviceCommand
    {
        public class Attributes
        {
            public const string IdNum = "ID";
            public const string Tries = "TRIES";
            public const string Status = "STATUS";
            public const string Status_None = "NONE";
            public const string Status_Sent = "SENT";
            public const string Status_WaitingAck = "WACK";
            public const string Status_Expired = "EXPIRED";
            public const string Status_GarminNotConnected = "NOGARMIN";
            public const string Status_Rollbacked = "ROLLBACKED";
            public const string LastTry = "LASTTRY";
            public const string ExpiresOn = "EXPIRESON";
        }

        protected INode _node;
        protected string _command;
        protected Dictionary<string, string> _attributes;

        public int? IdNum { get; set; }
        public uint? MessageId { get; set; }
        public uint? Tries { get; set; }
        public DateTime? LastTry { get; set; }
        public DateTime? ExpiresOn { get; set; }

        protected static String byte2string(byte[] b)
        {
            return Encoding.ASCII.GetString(b, 0, b.Length);
        }

        protected BaseDeviceCommand()
        {
            throw new NotSupportedException();
        }

        protected BaseDeviceCommand(string command) : this(command, null, null) { }

        protected BaseDeviceCommand(string command, INode node, DateTime? newExpiresOn)
        {
            _node = node;
            _command = getCommandFrom(command);
            _attributes = getCommandAttributesFrom(command);

            var idNum = (node != null ? node.IdNum ?? 0 : 0);

            #region idNum
            if (idNum == 0)
            {
                if (_attributes.ContainsKey(Attributes.IdNum))
                {
                    try
                    {
                        IdNum = Convert.ToInt32(_attributes[Attributes.IdNum]);
                        _attributes.Remove(Attributes.IdNum);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
                IdNum = idNum;
                if (_attributes.ContainsKey(Attributes.IdNum))
                    _attributes.Remove(Attributes.IdNum);
            }
            #endregion idNum


            #region Tries
            if (Tries == null)
            {
                if (_attributes.ContainsKey(Attributes.Tries))
                {
                    try
                    {
                        Tries = (uint?)Convert.ToInt32(_attributes[Attributes.Tries]);
                        _attributes.Remove(Attributes.Tries);
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                if (_attributes.ContainsKey(Attributes.Tries))
                    _attributes.Remove(Attributes.Tries);
            }
            #endregion Tries

            #region LastTry
            if (_attributes.ContainsKey(Attributes.LastTry))
            {
                try
                {
                    var dt = _attributes[Attributes.LastTry];
                    if (!String.IsNullOrEmpty(dt))
                    {
                        LastTry = DateTime.ParseExact(dt, "O", CultureInfo.InvariantCulture,
                                                      DateTimeStyles.None);
                        _attributes.Remove(Attributes.LastTry);
                    }
                }
                catch (Exception)
                {
                }
            }
            else
                LastTry = null;
            #endregion LastTry

            #region Expiration
            if (_attributes.ContainsKey(Attributes.ExpiresOn))
            {
                try
                {
                    var dt = _attributes[Attributes.ExpiresOn];
                    if (!String.IsNullOrEmpty(dt))
                    {
                        ExpiresOn = DateTime.ParseExact(dt, "O", CultureInfo.InvariantCulture,
                                                      DateTimeStyles.None);
                        _attributes.Remove(Attributes.ExpiresOn);
                    }
                }
                catch (Exception)
                {
                }
            }
            else
                ExpiresOn = null;

            if (newExpiresOn != null)
            {
                ExpiresOn = newExpiresOn;
            }

            #endregion Expiration

        }
        
        public static BaseDeviceCommand createFrom(string command, INode node, DateTime? ExpiresOn)
        {
            BaseDeviceCommand cmd = null;
            switch (node.NodeType)
            {
                case NodeTypes.Trax:
                    cmd = new GTEDeviceCommand(command, node, ExpiresOn);
                    break;
                case NodeTypes.Virloc:
                    cmd = new VirlocDeviceCommand(command, node, ExpiresOn);
                    break;
            }

            if (cmd == null) return null;

            return cmd;
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool clean)
        {
            var result = new StringBuilder();
            result.AppendFormat(">{0}", _command);
            foreach (var attribute in _attributes)
            {
                if (!(attribute.Key == Attributes.Status && clean))
                    result.AppendFormat(";{0}={1}", attribute.Key, attribute.Value);
            }

            if (IdNum != null && !isSettingIDMessage())
                result.AppendFormat(";{0}={1:D4}", Attributes.IdNum, IdNum);
            if (Tries != null && clean == false)
                result.AppendFormat(";{0}={1}", Attributes.Tries, Tries.Value);
            if (LastTry != null && clean == false)
                result.AppendFormat(";{0}={1}", Attributes.LastTry, LastTry.Value.ToString("O"));

            addCustomToString(clean, ref result);
            addCustomCheckSum(clean, ref result);

            result.Append("<");
            return result.ToString();
        }


        protected static byte CalculateCheckSum(String str)
        {
            var bytes = Encoding.ASCII.GetBytes(str);
            byte cs = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                cs ^= bytes[i];
            }
            return cs;
        }

        public string getCommand()
        {
            return _command;
        }

        public Dictionary<string, string> getAttributes()
        {
            return _attributes;
        }

        protected abstract string getCommandPattern();
        protected abstract string getCommandAttributesPattern();

        private string getCommandFrom(String command)
        {
            var mCommand = Regex.Match(command, getCommandPattern());
            if (mCommand.Success &&
                mCommand.Groups["command"].Success)
            {
                return mCommand.Groups["command"].Value;
            }
            else
            {
                STrace.Error(typeof (BaseDeviceCommand).FullName, IdNum ?? 0, "RECEIVED COMMAND: " + command);
                return null;
            }
        }

        private Dictionary<string, string> getCommandAttributesFrom(String command)
        {
            var mAttributes = Regex.Matches(command, getCommandAttributesPattern());

            return mAttributes.Cast<Match>().ToDictionary(ma => ma.Groups["param"].Value, ma => ma.Groups["value"].Value);
        }

        public bool isAlreadySent()
        {
            return _attributes.ContainsKey(Attributes.Status) &&
                   _attributes[Attributes.Status] == Attributes.Status_Sent;
        }

        public String getStatus()
        {
            return (_attributes.ContainsKey(Attributes.Status) ? _attributes[Attributes.Status] : null);

        }

        public void setStatus(String newStatus)
        {
            if (newStatus == null &&
                _attributes.ContainsKey(Attributes.Status))
            {
                _attributes.Remove(Attributes.Status);
            }
            else
                _attributes[Attributes.Status] = newStatus;
        }

        public abstract DeviceCommandResponseStatus isExpectedResponse(BaseDeviceCommand response);

        public abstract bool isGarminMessage();

        protected abstract bool isSettingIDMessage();

        protected abstract void addCustomToString(bool clean, ref StringBuilder result);

        protected abstract void addCustomCheckSum(bool clean, ref StringBuilder result);

        public abstract BaseDeviceCommand BuildAck();
    }
}
