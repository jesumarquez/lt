using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Permissions;
using System.Windows.Threading;
using System.Collections.Specialized;
using Urbetrack.QuadTree;
using System.Windows.Data;
using Urbetrack.Toolkit;

namespace RNR.Maps
{
    public class QuadTreeCellToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {            
            SolidColorBrush myBrush = new SolidColorBrush();
            if (value == null)
            {
                myBrush.Color = Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF);
                return myBrush;
            }

            var cell = value as MapCanvas.QuadTreeCell;
            /*if(cell != null)
            {
                IniFile.Get(Urbetrack.Toolkit.Process.GetApplicationFile("settings.ini"),"Colors","Grid" +) ()
            }*/
            switch (cell.Class)
            {
                case 0: 
                    myBrush.Color = Color.FromArgb(0x33,0,0,0);
                    break;
                case 1: 
                    myBrush.Color = Color.FromArgb(0x33,0,0,255);
                    break;
                case 2: 
                    myBrush.Color = Color.FromArgb(0x33,0,255,0);
                    break;
                case 3: 
                    myBrush.Color = Color.FromArgb(0x33,255,0,0);
                    break;
                case 4: 
                    myBrush.Color = Color.FromArgb(0x33,0xAA,0xEE,0);
                    break;
                case 5: 
                    myBrush.Color = Color.FromArgb(0x33,0x22,0xAA,0xEE);
                    break;
                case 6:
                    myBrush.Color = Color.FromArgb(0x33, 0x22, 0xAA, 0xEE);
                    break;
                case 7:
                    myBrush.Color = Color.FromArgb(0x33, 0xFF, 0x00, 0x00);
                    break;
                case 8:
                    myBrush.Color = Color.FromArgb(0x33, 0x22, 0xAA, 0xEE);
                    break;
                case 9:
                    myBrush.Color = Color.FromArgb(0x33, 0x22, 0xAA, 0xEE);
                    break;
                case 10:
                    myBrush.Color = Color.FromArgb(0x33, 0x22, 0xAA, 0xEE);
                    break;
                case 11:
                    myBrush.Color = Color.FromArgb(0x33, 0x22, 0xAA, 0xEE);
                    break;
                case 12:
                    myBrush.Color = Color.FromArgb(0x33, 0x22, 0xAA, 0xEE);
                    break;
                case 13:
                    myBrush.Color = Color.FromArgb(0x33, 0x22, 0xAA, 0xEE);
                    break;
                case 14:
                    myBrush.Color = Color.FromArgb(0x33, 0x22, 0xAA, 0xEE);
                    break;
                case 15:
                    myBrush.Color = Color.FromArgb(0x33, 0x22, 0xAA, 0xEE);
                    break;
                default:
                    myBrush.Color = Color.FromArgb(0x33,0xFF,0xFF,0xFF);
                    break;
            }
            return myBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

    }
}
