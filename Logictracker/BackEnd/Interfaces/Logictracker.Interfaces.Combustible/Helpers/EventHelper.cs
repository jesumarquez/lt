#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Logictracker.Messages.Saver;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Utils;

#endregion

namespace Logictracker.Interfaces.Combustible.Helpers
{
    public class EventHelper
    {
        #region Constants

            #region Bit Index

            private const int MOilBit = 0;
            private const int MBatteryOff = 0;
            private const int MPowerOn = 1;
            private const int MBatteryOn = 2;
            private const int MDisconnected = 3;

            #endregion

            #region Message Codes

			private const String CodeOilPressure = "350";
			private const String CodeNoOilPressure = "351";

			private const String CodeMBatteryOn = "352";
			private const String CodeMWithouthBattery = "353";
			private const String CodeMBatteryOff = "354";
			private const String CodeMPowerOn = "355";
			private const String CodeMDisconnected = "356";
			private const String CodeMConnected = "357";
			private const String CodeMCaudalSuperado = "364";
			private const String CodeMTiempoSinReportar = "372";

			private const String CodeTNoConfig = "358";
			private const String CodeTCommError = "359";
			private const String CodeTInternalError = "360";
			private const String CodeTTransmissorError = "361";
			private const String CodeTWait = "362";
			private const String CodeTOk = "363";
			private const String CodeTStockCritico = "365";
			private const String CodeTStockReposicion = "366";
			private const String CodeTAguaSuperada = "367";
			private const String CodeTAguaNoAlcanzada = "368";
			private const String CodeTVolumenTeoricoNegativo = "369";
			private const String CodeTVolumenTeoricoSuperoCapacidad = "370";
			private const String CodeTDiferenciaRealVsTeoricoExcedida = "373";
			private const String CodeTTiempoSinReportar = "371";

            #endregion

            #region Messages For Codes

			private const String MsgOilPressure = "";
			private const String MsgNoOilPressure = "";

			private const String MsgMBatteryOn = "";
			private const String MsgMWithouthBattery = "";
			private const String MsgBatteryOff = "";
			private const String MsgMPowerOn = "";
			private const String MsgMDisconnected = "";
			private const String MsgMConnected = "";
			private const String MsgMCaudalSuperado = ". Caudal:{0} lt/h";

			private const String MsgTNoConfig = "";
			private const String MsgTCommError = "";
			private const String MsgTInternalError = "";
			private const String MsgTTransmissorError = "";
			private const String MsgTWait = "";
			private const String MsgTOk = "";
			private const String MsgTStockReposicion = ". Stock: {0} lt. Stock de Reposicion: {1} lt";
			private const String MsgTStockCritico = ". Stock: {0} lt. Stock Critico: {1} lt";
			private const String MsgTAguaSuperada = ". Nivel de Agua: {0} lt";
			private const String MsgTAguaNoAlcanzada = ". Nivel de Agua: {0} lt";

            #endregion

        #endregion

        #region Private Properties

        /// <summary>
        /// Key is CENTER+ID of the Motor and value the last bit value.
        /// </summary>
		private static readonly Dictionary<String, bool> DictionaryLastOilBit = new Dictionary<String, bool>();

        /// <summary>
        /// Key is CENTER+ID of the Motor and value the last bit value.
        /// </summary>
		private static readonly Dictionary<String, bool> DictionaryLastDisconnectedBit = new Dictionary<String, bool>();

        /// <summary>
        /// Key is CENTER+ID of the Motor and value the last bit value.
        /// </summary>
		private static readonly Dictionary<String, bool> DictionaryLastBatteryOnBit = new Dictionary<String, bool>();

        /// <summary>
        /// Key is CENTER+ID of the Tank and value the last event value.
        /// </summary>
		private static readonly Dictionary<String, int> DictionaryLastTankEvent = new Dictionary<String, int>();

        /// <summary>
        /// Key is Code of the Tank and value the last event value.
        /// </summary>
		private static readonly Dictionary<String, Double> DictionaryLastStockValue = new Dictionary<String, Double>();

        /// <summary>
        /// Key is Code of the Tank and value the last event value.
        /// </summary>
		private static readonly Dictionary<String, Double> DictionaryLastWaterLevel = new Dictionary<String, Double>();

        /// <summary>
        /// Fuel messages saver.
        /// </summary>
        private static EventoCombustibleSaver _eventoCombustibleSaver;

        /// <summary>
        /// Fuel messages data access saver.
        /// </summary>
        private static EventoCombustibleSaver EventoCombustibleSaver { get { return _eventoCombustibleSaver ?? (_eventoCombustibleSaver = new EventoCombustibleSaver()); } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Process a Motor Entries string.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="id"></param>
        /// <param name="entriesString"></param>
        /// <param name="date"></param>
		public static void ProcessMotorEntries(String center, String id, String entriesString, DateTime date)
        {
            var entry = Encoding.ASCII.GetBytes(entriesString)[0];

            var oilBit = BitHelper.IsBitSet(entry, MOilBit+1);

            ProcessOilPressure(center, id, oilBit, date);
        }

        /// <summary>
        /// Process a Motor and Medidor Events string.
        /// IMPORTANT: idMotor and idMedidor are exclusive. Both of them can't have value at the same time.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="idMedidor"></param>
        /// <param name="eventsString"></param>
        /// <param name="date"></param>
        /// <param name="idMotor"></param>
		public static void ProcessMotorAndMedidorEvents(String center, String idMotor, String idMedidor, String eventsString, DateTime date)
        {
            var events = Encoding.ASCII.GetBytes(eventsString)[eventsString.Length - 1];

            ProcessBatteryOff(idMotor, idMedidor, BitHelper.IsBitSet(events, MBatteryOff + 1), date);
            ProcessPowerOn(idMotor, idMedidor, BitHelper.IsBitSet(events, MPowerOn + 1), date);
            ProcessDisconnected(center, idMotor, idMedidor, BitHelper.IsBitSet(events, MDisconnected + 1), date);
            ProcessBatteryOn(center, idMotor, idMedidor, BitHelper.IsBitSet(events, MBatteryOn + 1), date);
        }

        /// <summary>
        /// Process a Tank Events string.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="id"></param>
        /// <param name="eventsString"></param>
        /// <param name="date"></param>
		public static void ProcessTankEvents(String center, String id, String eventsString, DateTime date)
        {
            var events = (int)Encoding.ASCII.GetBytes(eventsString)[0];

            ProcessTankEvent(events, center, id, date);
        }

        /// <summary>
        /// Evaluates if the Motor has exceeded the Maximum Value of the Caudal configured for it
        /// </summary>
        /// <param name="caudal"></param>
        /// <param name="caudalimetro"></param>
        /// <param name="fecha"></param>
        public static void EvaluteMaximumCaudal(double caudal, Caudalimetro caudalimetro, DateTime fecha)
        {
            if (caudal > caudalimetro.CaudalMaximo && caudalimetro.CaudalMaximo != 0)
                EventoCombustibleSaver.CreateNewEvent(caudalimetro.Codigo, null, CodeMCaudalSuperado, String.Format(MsgMCaudalSuperado, caudal), fecha);
        }

        /// <summary>
        /// Evaluates if the Tanque has reached their Reposicion and Critico Stock Values.
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="tanque"></param>
        /// <param name="fecha"></param>
        public static void EvaluateStockCriticoAndReposicion(double volume, Tanque tanque, DateTime fecha)
        {
            Double lastStock = -1;

            if (DictionaryLastStockValue.ContainsKey(tanque.Codigo)) lastStock = DictionaryLastStockValue[tanque.Codigo];

            if (StockIsCritical(tanque, lastStock, volume))
            {
                EventoCombustibleSaver.CreateNewEvent(null, tanque.Codigo, CodeTStockCritico, String.Format(MsgTStockCritico, volume, tanque.StockCritico), fecha);

                SetLastValue(DictionaryLastStockValue, tanque.Codigo, volume);

                return;
            }

            if(StockNeedsReposition(tanque, lastStock, volume))
                EventoCombustibleSaver.CreateNewEvent(null, tanque.Codigo, CodeTStockReposicion, String.Format(MsgTStockReposicion,volume,tanque.StockReposicion), fecha);

            SetLastValue(DictionaryLastStockValue, tanque.Codigo, volume);
        }

        /// <summary>
        /// Evaluates if the Tanque has reached their Nivel Agua Maximum and Minimum Values.
        /// </summary>
        /// <param name="agua"></param>
        /// <param name="tanque"></param>
        /// <param name="fecha"></param>
        public static void EvaluteWaterLevel(double agua, Tanque tanque, DateTime fecha)
        {
            Double lastWaterLevel = -1;

            if (DictionaryLastWaterLevel.ContainsKey(tanque.Codigo)) lastWaterLevel = DictionaryLastWaterLevel[tanque.Codigo];

            if (WaterOverMaxLevel(tanque, lastWaterLevel, agua)) EventoCombustibleSaver.CreateNewEvent(null, tanque.Codigo, CodeTAguaSuperada, String.Format(MsgTAguaSuperada, agua), fecha);

            if (WaterBelowMinLevel(tanque, lastWaterLevel, agua)) EventoCombustibleSaver.CreateNewEvent(null, tanque.Codigo, CodeTAguaNoAlcanzada, String.Format(MsgTAguaNoAlcanzada, agua), fecha);

            SetLastValue(DictionaryLastWaterLevel, tanque.Codigo, agua);
        }

        #endregion

        #region Private Methods

        private static bool StockNeedsReposition(Tanque tanque, double lastStock, double volume)
        {
            return tanque.StockReposicion.HasValue && ((lastStock.Equals(-1) || lastStock > (double)tanque.StockReposicion) && (volume <= (double)tanque.StockReposicion));
        }

        private static bool StockIsCritical(Tanque tanque, double lastStock, double volume)
        {
            return tanque.StockCritico.HasValue && ((lastStock.Equals(-1) || lastStock > (double)tanque.StockCritico) && (volume <= (double)tanque.StockCritico));
        }

        private static bool WaterBelowMinLevel(Tanque tanque, double lastWaterLevel, double agua)
        {
            return tanque.AguaMin.HasValue && ((lastWaterLevel.Equals(-1) || lastWaterLevel >= (double)tanque.AguaMin) && agua < (double)tanque.AguaMin);
        }

        private static bool WaterOverMaxLevel(Tanque tanque, double lastWaterLevel, double agua)
        {
            return tanque.AguaMax.HasValue && ((lastWaterLevel.Equals(-1) || lastWaterLevel <= (double)tanque.AguaMax) && agua > (double)tanque.AguaMax);
        }

        /// <summary>
        /// Sets a value in a dictionary.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private static void SetLastValue(IDictionary dictionary,object key,object value)
        {
            if(dictionary.Contains(key)) dictionary.Remove(key);

            dictionary.Add(key, value);
        }

        /// <summary>
        /// Process an OIL PRESSURE bit.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="id"></param>
        /// <param name="oilBit"></param>
        /// <param name="date"></param>
		private static void ProcessOilPressure(String center, String id, bool oilBit, DateTime date)
        {
            var key = center + id;
            bool lastOilValue;

            DictionaryLastOilBit.TryGetValue(key, out lastOilValue);

            if (lastOilValue == oilBit) return;

            if (oilBit) EventoCombustibleSaver.CreateNewEvent(id, null, CodeOilPressure, MsgOilPressure, date);
            else EventoCombustibleSaver.CreateNewEvent(id, null, CodeNoOilPressure, MsgNoOilPressure, date);

            SetLastValue(DictionaryLastOilBit, key, oilBit);
        }

        /// <summary>
        /// Process a BATTERY ON bit.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="idMedidor"></param>
        /// <param name="newValue"></param>
        /// <param name="date"></param>
        /// <param name="idMotor"></param>
		private static void ProcessBatteryOn(String center, String idMotor, String idMedidor, bool newValue, DateTime date)
        {
            var key = center + idMotor ?? idMedidor;
            bool lastValue;

            DictionaryLastBatteryOnBit.TryGetValue(key, out lastValue);

            if (lastValue == newValue) return;

            if (newValue) /*si es 1 se desconecto*/ EventoCombustibleSaver.CreateNewEvent(idMotor, idMedidor, CodeMBatteryOn, MsgMBatteryOn, date);
            else EventoCombustibleSaver.CreateNewEvent(idMotor, idMedidor, CodeMWithouthBattery, MsgMWithouthBattery, date);

            SetLastValue(DictionaryLastBatteryOnBit, key, newValue);
        }

        /// <summary>
        /// Process a DISCONNECTED ENGINE bit.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="idMedidor"></param>
        /// <param name="newValue"></param>
        /// <param name="date"></param>
        /// <param name="idMotor"></param>
		private static void ProcessDisconnected(String center, String idMotor, String idMedidor, bool newValue, DateTime date)
        {
            var key = center + idMotor ?? idMedidor;
            bool lastValue;

            DictionaryLastDisconnectedBit.TryGetValue(key, out lastValue);

            if (lastValue == newValue) return;

            if (newValue) /*si es 1 se desconecto*/ EventoCombustibleSaver.CreateNewEvent(idMotor, idMedidor, CodeMDisconnected, MsgMDisconnected, date);
            else EventoCombustibleSaver.CreateNewEvent(idMotor, idMedidor, CodeMConnected, MsgMConnected, date);

            SetLastValue(DictionaryLastDisconnectedBit, key, newValue);
        }

        /// <summary>
        /// Process a POWER ON bit.
        /// </summary>
        /// <param name="idMotor"></param>
        /// <param name="idMedidor"></param>
        /// <param name="powerOn"></param>
        /// <param name="date"></param>
		private static void ProcessPowerOn(String idMotor, String idMedidor, bool powerOn, DateTime date)
        {
            if (powerOn) EventoCombustibleSaver.CreateNewEvent(idMotor, idMedidor, CodeMPowerOn, MsgMPowerOn, date);
        }

        /// <summary>
        /// Process a BATTERY OFF bit.
        /// </summary>
        /// <param name="idMedidor"></param>
        /// <param name="batteryOff"></param>
        /// <param name="date"></param>
        /// <param name="idMotor"></param>
		private static void ProcessBatteryOff(String idMotor, String idMedidor, bool batteryOff, DateTime date)
        {
            if (batteryOff) EventoCombustibleSaver.CreateNewEvent(idMotor, idMedidor, CodeMBatteryOff, MsgBatteryOff, date);
        }

        /// <summary>
        /// Process a Tank event.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="center"></param>
        /// <param name="id"></param>
        /// <param name="fecha"></param>
		private static void ProcessTankEvent(int events, String center, String id, DateTime fecha)
        {
            String code;
            String message;

            switch (events)
            {
                case 0: { code = CodeTOk; message = MsgTOk; break; }
                case 1: { code = CodeTNoConfig; message = MsgTNoConfig; break; }
                case 2: { code = CodeTCommError; message = MsgTCommError; break; }
                case 3: { code = CodeTInternalError; message = MsgTInternalError; break; }
                case 4: { code = CodeTTransmissorError; message = MsgTTransmissorError; break; }
                case 5: { code = CodeTWait; message = MsgTWait; break; }
                default: return;
            }

            var key = center + id;
            int lastValue;

            DictionaryLastTankEvent.TryGetValue(key, out lastValue);

            if (events.Equals(lastValue)) return;

            EventoCombustibleSaver.CreateNewEvent(null, id, code, message, fecha);

            SetLastValue(DictionaryLastTankEvent, key, events);
        }

        #endregion
    }
}
