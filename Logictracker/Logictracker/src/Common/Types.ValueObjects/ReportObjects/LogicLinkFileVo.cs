using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class LogicLinkFileVo
    {
        public const int IndexId = 0;
        public const int IndexEmpresa = 1;
        public const int IndexBase = 2;
        public const int IndexServerName = 3;
        public const int IndexFilePath = 4;
        public const int IndexFileSource = 5;
        public const int IndexStrategy = 6;
        public const int IndexDateAdded = 7;
        public const int IndexDateProcessed = 8;
        public const int IndexStatus = 9;
        public const int IndexResult = 10;

        [GridMapping(Index = IndexId, ResourceName = "Labels", VariableName = "ID", AllowGroup = false)]
        public int Id { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01", AllowGroup = true)]
        public string Empresa { get; set; }

        [GridMapping(Index = IndexBase, ResourceName = "Entities", VariableName = "PARENTI02", AllowGroup = true)]
        public string Base { get; set; }

        [GridMapping(Index = IndexServerName, ResourceName = "Labels", VariableName = "SERVER_NAME", IncludeInSearch = true)]
        public string ServerName { get; set; }

        [GridMapping(Index = IndexFilePath, ResourceName = "Labels", VariableName = "FILE_PATH", IncludeInSearch = true)]
        public string FilePath { get; set; }

        [GridMapping(Index = IndexFileSource, ResourceName = "Labels", VariableName = "FILE_SOURCE", IncludeInSearch = true)]
        public string FileSource { get; set; }

        [GridMapping(Index = IndexStrategy, ResourceName = "Labels", VariableName = "STRATEGY", IncludeInSearch = true)]
        public string Strategy { get; set; }

        [GridMapping(Index = IndexDateAdded, ResourceName = "Labels", VariableName = "DATE_ADDED", IncludeInSearch = true)]
        public DateTime DateAdded { get; set; }

        [GridMapping(Index = IndexDateProcessed, ResourceName = "Labels", VariableName = "DATE_PROCESSED", IncludeInSearch = true)]
        public DateTime? DateProcessed { get; set; }

        [GridMapping(Index = IndexStatus, ResourceName = "Labels", VariableName = "STATUS", IncludeInSearch = true)]
        public string Status { get; set; }

        [GridMapping(Index = IndexResult, ResourceName = "Labels", VariableName = "RESULT", IncludeInSearch = true)]
        public string Result { get; set; }

        public LogicLinkFileVo(LogicLinkFile logicLinkFile)
        {
            Id = logicLinkFile.Id;
            Empresa = logicLinkFile.Empresa.RazonSocial;
            Base = logicLinkFile.Linea != null ? logicLinkFile.Linea.Descripcion : string.Empty;
            ServerName = logicLinkFile.ServerName;
            FilePath = logicLinkFile.FilePath;
            FileSource = logicLinkFile.FileSource;
            Strategy = logicLinkFile.Strategy;
            DateAdded = logicLinkFile.DateAdded.ToDisplayDateTime();

            if (logicLinkFile.DateProcessed == null)
            {
                DateProcessed = null;
            }
            else
            {
                DateProcessed = logicLinkFile.DateProcessed.Value.ToDisplayDateTime();
            }

            
            var status = string.Empty;
            switch (logicLinkFile.Status)
            {
                case LogicLinkFile.Estados.Pendiente: status = "PENDIENTE"; break;
                case LogicLinkFile.Estados.Procesado: status = "PROCESADO"; break;
                case LogicLinkFile.Estados.Procesando: status = "PROCESANDO"; break;
                case LogicLinkFile.Estados.Cancelado: status = "CANCELADO"; break;
            }
            Status = status;
            Result = logicLinkFile.Result;
        }
    }
}
