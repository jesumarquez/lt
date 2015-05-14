
#region Usings

using System;
using Logictracker.DAL.Factories;

#endregion

namespace Logictracker.Interfaces.Combustible.Parsers
{
    interface IParser<T>
    {
        /// <summary>
        /// Parses the string returning a <T> object.
        /// </summary>
        /// <returns></returns>
		void Parse(String str, DAOFactory daof);
    }
}
