using System;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.Process.Import.Client.DataStrategy
{
    public class FixedDataStrategy: FileDataStrategy
    {
        protected int[] Anchos { get; set; }

        public FixedDataStrategy(params IDataSourceParameter[] parameters)
            :base(parameters)
        {
            if (!Parameters.ContainsKey("anchos")) throw new ApplicationException("No se encontro el parametro anchos");

            try
            {
                Anchos = Parameters["anchos"].Trim().Split('-').Select(a => Convert.ToInt32(a)).ToArray();
            }
            catch (Exception)
            {
                throw new ApplicationException("El formato del parametro ancho no es correcto");
            }

            Logger.Info("CsvDataStrategy iniciado (Folder: " + Folder + " | FileName: " + FileName + " | HeaderRow: " + HeaderRow + " | Anchos: " + Anchos);
        }

        public override string[] Split(string line)
        {
            var list = new List<string>();
            var indice = 0;
            
            foreach (var ancho in Anchos)
            {
                var text = line.Substring(indice, ancho).Trim();
                list.Add(text);
                indice += ancho;
            }
            
            return list.ToArray();
        }
    }
}
