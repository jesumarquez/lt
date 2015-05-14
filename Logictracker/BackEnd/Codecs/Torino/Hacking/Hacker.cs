#region Usings

using System;
using Urbetrack.Configuration;

#endregion

namespace Urbetrack.Hacking
{
    public class Hacker
    {
        public class XBEE
        {
            public static bool DisableAutomontion = true;
        }

        public class Device
        {
            static Device()
            {
                Torino05_WindowSize = Config.Torino.Torino05WindowsSize;
                Torino10_WindowSize = Config.Torino.Torino10WindowsSize;
                KeepAliveInterval = Config.Torino.KeepaliveInterval;
                FixMissmatchConfiguration = Config.Torino.FixMissmatchConfiguration;
            }
            public static bool CheckPasswords;
            public static int KeepAliveInterval;
            public static int Torino05_WindowSize;
            public static int Torino10_WindowSize;
            public static bool FixMissmatchConfiguration;
        }

        public class TCP
        {
            // retransmisiones en TCP para Sistelcom JAJA.!
            public static bool Retransmit = true;
        }

        public class UDP
        {
            public static bool DisableSocketRead;
            public static bool DisableSocketWrite;
        }

        public class ServerUT
        {
            public static bool DataPageExSignatureError;
        }

        public class Network
        {
            public static int DATA_PAGE_INITIAL_LAPSE = 1500;
            public static int DATA_INTER_PAGE_LAPSE = 10;
            public static int DATA_PAGE_LAPSE = 1000;
        }

        public class QTree
        {
            static QTree()
            {
                OperacionesPorPagina = Config.Torino.QtreeMaxPageOperations;
            }
            public static bool HuffmanEnabled;
            public static bool SendExplicitADDGR2;
            public static int OperacionesPorPagina;
        }

        public class UnetelV2
        {
            public static bool ReplyDisabled;
        }

        public class RND
        {
            public static bool Chance(int oftrue, int offalse, bool condition)
            {
                if (condition == false) return false;
                var a = new Random();
                return a.Next(0, oftrue + offalse) <= oftrue;
            }
        }
    }
}