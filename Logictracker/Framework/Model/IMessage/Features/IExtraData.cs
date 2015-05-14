#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Logictracker.Model
{
    /// <summary>
    /// Interface para eventos con datos extra
    /// </summary>
    public interface IExtraData : IMessage
    {
        /// <summary>
        /// Extra data list
		/// data[0] && Assert(Code == Generic): codigo del evento
		/// data[1] && Assert(Code == StopStatus...): Identificador del Stop
		/// data[1 | 2 | ...] && Assert(HasExtraData(Code)): Datos extra
		/// </summary>
        List<Int64> Data { get;}
    }

    public static class IExtraDataX
    {
		public static Int64 GetData(this IExtraData me)
		{
			return me == null ? 0 : (me.Data.Count <= 0 ? 0 : me.Data[0]);
		}

		public static Int64 GetData2(this IExtraData me)
		{
			return me == null ? 0 : (me.Data.Count <= 1 ? 0 : me.Data[1]);
		}
    }
}
