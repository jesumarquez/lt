using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Model;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Utils;
using System.Runtime.InteropServices;

namespace Logictracker.Layers.DeviceCommandCodecs
{
     public class NovatelObjectSerialized : GenericParserSerialized
    {
        public string paramOne { get; set; } //1234 Param1 (4 bytes) 
        public string ModemID { get; set; } // MTGPSTEST Modem ID (22 bytes)
        public string MessageID { get; set; } //RMC protocol header (6 bytes)
        public string UTCPosition { get; set; } //UTC Position 165717.00 hh mm ss.sss (9 bytes)
        public string Status { get; set; } //A A=datavalid or V=datanot valid or 9 =last known GPS location (1 bytes)
        public string Latitude { get; set; } // Latitude 3259.816776 dd mm.mmmm 
        public string Indicator { get; set; } // N/S Indicator N N =north or S =south
        public string Longitude { get; set; } // Longitude 09642.858868 ddd mm.mmmm
        public string IndicatorTwo { get; set; } // E/W Indicator W E=east or W =west
        public string SpeedOverGround { get; set; } // Speed Over Ground 0.0 knots 
        public string CourseOverGround { get; set; } // CourseOver Ground 0 degrees True 
        public string Date { get; set; } // Date 070108 dd mm yy 
        public string MagneticVariation { get; set; } // Magnetic Variation* 3.5 degrees 
        public string EastOrWest { get; set; } // W E=east or W =west
        public string PositionModeIndicator { get; set; } // Position ModeIndicator* A A=Autonomous
        public string Checksum { get; set; } // Checksum *30 

        /*1234 MTGPSTEST              $GPRMC,165717.00,A,3259.816776,N,09642.858868,W,0.0,0.0,070108,3.5, W,A*30 */
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct NovatelObjectStruct
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string paramOne;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string spaceWhite0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 22)]
            public string ModemID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string spaceWhite2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string MessageID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string comma0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
            public string UTCPosition;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string comma1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            public string Status;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string comma2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
            public string Latitude;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string comma3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            public string Indicator;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string comma4;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public string Longitude;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string comma5;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            public string IndicatorTwo;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string comma6;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public string SpeedOverGround;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string comma7;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public string CourseOverGround;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string comma8;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string Date;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string comma9;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public string MagneticVariation;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string comma10;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            public string EastOrWest;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            private string comma11;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            public string PositionModeIndicator;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public string Checksum;
        }

        public NovatelObjectSerialized()
        {
            paramOne = new string(' ', 4); //1234 Param1 (4 bytes) 
            ModemID = new string(' ', 22); // MTGPSTEST Modem ID (22 bytes)
            MessageID = new string(' ', 6); //RMC protocol header (6 bytes)
            UTCPosition = new string(' ', 9); //UTC Position 165717.00 hh mm ss.sss (9 bytes)
            Status = new string(' ', 1); //A A=datavalid or V=datanot valid or 9 =last known GPS location (1 bytes)
            Latitude = new string(' ', 11); // Latitude 3259.816776 dd mm.mmmm 
            Indicator = new string(' ', 1); // N/S Indicator N N =north or S =south
            Longitude = new string(' ', 12); // Longitude 09642.858868 ddd mm.mmmm
            IndicatorTwo = new string(' ', 1); // E/W Indicator W E=east or W =west 
            SpeedOverGround = new string(' ', 3); // Speed Over Ground 0.0 knots 
            CourseOverGround = new string(' ', 1); // CourseOver Ground 0 degrees True 
            Date = new string(' ', 6); // Date 070108 dd mm yy 
            MagneticVariation = new string(' ', 4); // Magnetic Variation* 3.5 degrees 
            EastOrWest = new string(' ', 1); // W E=east or W =west
            PositionModeIndicator = new string(' ', 1); // Position ModeIndicator* A A=Autonomous
            Checksum = new string(' ', 3); // Checksum *30 
        }

        internal void SetStructData(byte[] b1)
        {
            NovatelObjectStruct dataStruct = (NovatelObjectStruct)getStructData(b1, new NovatelObjectStruct());
            paramOne = dataStruct.paramOne; //1234 Param1 (4 bytes) 
            ModemID = dataStruct.ModemID; // MTGPSTEST Modem ID (22 bytes)
            MessageID = dataStruct.MessageID; //RMC protocol header (6 bytes)
            UTCPosition = dataStruct.UTCPosition; //UTC Position 165717.00 hh mm ss.sss (9 bytes)
            Status = dataStruct.Status; //A A=datavalid or V=datanot valid or 9 =last known GPS location (1 bytes)
            Latitude = dataStruct.Latitude; // Latitude 3259.816776 dd mm.mmmm 
            Indicator = dataStruct.Indicator; // N/S Indicator N N =north or S =south
            Longitude = dataStruct.Longitude; // Longitude 09642.858868 ddd mm.mmmm
            IndicatorTwo = dataStruct.IndicatorTwo; // E/W Indicator W E=east or W =west 
            SpeedOverGround = dataStruct.SpeedOverGround; // Speed Over Ground 0.0 knots 
            CourseOverGround = dataStruct.CourseOverGround; // CourseOver Ground 0 degrees True 
            Date = dataStruct.Date; // Date 070108 dd mm yy 
            MagneticVariation = dataStruct.MagneticVariation; // Magnetic Variation* 3.5 degrees 
            EastOrWest = dataStruct.EastOrWest; // W E=east or W =west
            PositionModeIndicator = dataStruct.PositionModeIndicator; // Position ModeIndicator* A A=Autonomous
            Checksum = dataStruct.Checksum; // Checksum *30 
        }
    }
}
