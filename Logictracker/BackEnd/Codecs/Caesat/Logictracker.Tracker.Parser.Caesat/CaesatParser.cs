using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using log4net;
using Logictracker.Tracker.Parser.Spi;
using Logictracker.Types.BusinessObjects.Positions;

namespace Logictracker.Tracker.Parser.Caesat
{
    public class CaesatParser : IParser
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CaesatParser));
        
        public CaesatFrame ParseCaesatFrame(string frame)
        {
            Logger.DebugFormat("ParseCaesatFrame: {0} ", frame);

            int result;
            var caesatframe = new CaesatFrame();

            //0---------1---------2---------3---------4---------5-------   
            //0123456789012345678901234567890123456789012345678901234567
            //PC090415202111KPB696-34.77947-058.26496000045101+00+00+00|

            try
            {
                caesatframe.MessageType = ParseMessageType(frame);
                caesatframe.DateTime = ParseDateTime(frame);
                caesatframe.LicensePlate = ParseLicensePlate(frame);
                caesatframe.Coordinate = parseCoordinate(frame);
                int.TryParse(TrySubstring(frame, 39, 3, "speed"), out result);
                caesatframe.Speed = result;
                int.TryParse(TrySubstring(frame, 42, 3, "Meaning"), out result);
                caesatframe.Meaning = result;
                int.TryParse(TrySubstring(frame, 46, 2, "EventCode"), out result);
                caesatframe.EventCode = result;
                int.TryParse(TrySubstring(frame, 48, 3, "Temperature1"), out result);
                caesatframe.Temperature1 = result;
                int.TryParse(TrySubstring(frame, 51, 3, "Temperature2"), out result);
                caesatframe.Temperature2 = result;
                int.TryParse(TrySubstring(frame, 54, 3, "Temperature3"), out result);
                caesatframe.Temperature3 = result;
                caesatframe.Raw = frame;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Error obtaining caesatFrame {0}", ex.Message);
                return null;
            }

            return caesatframe;
        }

        private static string TrySubstring(string frame, int start, int length, string name)
        {
            try
            {
                return frame.Substring(start, length);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Error obtaining {0} : {1} Exception {2}", name, frame, ex.Message);
                return string.Empty;
            }
        }

        private static string ParseLicensePlate(string frame)
        {
            try
            {
                return frame.Substring(14, 6);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Error obtaining License Plate : {0} Exception {1}", frame, e.Message);
                throw;
            }
        }

        private static string ParseMessageType(string frame)
        {
            try
            {
                return frame.Substring(0, 2);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Error obtaining MessageType : {0} Exception {1}", frame, e.Message);
                return string.Empty;
            }
        }

        private Coordinate parseCoordinate(string frame)
        {
            try
            {
                var lat = float.Parse(string.Format(frame.Substring(20, 9)), CultureInfo.InvariantCulture);
                var lon = float.Parse(string.Format(frame.Substring(29, 10)), CultureInfo.InvariantCulture);

                return new Coordinate()
                {
                    Latitude = lat,
                    Longitude = lon,
                    IsValid = frame.Substring(45, 1).Equals("1")
                };
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Error obtaining Coordinate : {0} Exception {1}", frame, e.Message);
                throw;
            }
        }

        private static DateTime ParseDateTime(string frame)
        {
            //0123456789ABCD
            //PC090415202111...

            try
            {
                return new DateTime(int.Parse("20" + frame.Substring(6, 2)), int.Parse(frame.Substring(4, 2)), int.Parse(frame.Substring(2, 2)),
                    int.Parse(frame.Substring(8, 2)), int.Parse(frame.Substring(10, 2)), int.Parse(frame.Substring(12, 2)));
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Error in parseDateTime in frame {0} exception: {1}", e.Message);
                return DateTime.UtcNow;
            }
        }

        public Match SplitCaesatFrames(string frames)
        {
            Logger.DebugFormat("SplitCaesatFrames: {0} ", frames);

            var match = Regex.Match(frames, @"([A-Za-z0-9\-\+\.]+)", RegexOptions.IgnoreCase);
            return match;
            //return frames.Split('|').ToList();
        }

        public IList<IFrame> Parse(string receivedData)
        {
            Logger.DebugFormat("Parse frames: {0} ", receivedData);

            var caesatFrameList = new List<IFrame>();

            var match = SplitCaesatFrames(receivedData);

            while (match.Success)
            {
                Logger.DebugFormat("Match success: {0} ", match.Value);

                caesatFrameList.Add(ParseCaesatFrame(match.Value));
                match = match.NextMatch();
            }

            return caesatFrameList;
        }

        #region oldMethods
        //public CaesatFrame ParseCaesatFrame(string frame)
        //{
        //    Logger.DebugFormat("ParseCaesatFrame: {0} ",frame);

        //    var caesatframe = new CaesatFrame();

        //    //0---------1---------2---------3---------4---------5-------   
        //    //0123456789012345678901234567890123456789012345678901234567
        //    //PC090415202111KPB696-34.77947-058.26496000045101+00+00+00|

        //    caesatframe.MessageType = frame.Substring(0, 2);
        //    caesatframe.DateTime = parseDateTime(frame);
        //    caesatframe.LicensePlate = frame.Substring(14, 6);
        //    caesatframe.Coordinate = parseCoordinate(frame);
        //    caesatframe.Speed = int.Parse(frame.Substring(39, 3));
        //    caesatframe.Meaning = int.Parse(frame.Substring(42, 3));
        //    caesatframe.EventCode = int.Parse(frame.Substring(46, 2));
        //    caesatframe.Temperature1= int.Parse(frame.Substring(48, 3));
        //    caesatframe.Temperature2 = int.Parse(frame.Substring(51, 3));
        //    caesatframe.Temperature3 = int.Parse(frame.Substring(54, 3));
        //    caesatframe.Raw = frame;

        //    return caesatframe;
        //}

        //private Coordinate parseCoordinate(string frame)
        //{
        //    var lat = float.Parse(string.Format(frame.Substring(20, 9)),CultureInfo.InvariantCulture);
        //    var lon = float.Parse(string.Format(frame.Substring(29, 10)), CultureInfo.InvariantCulture); 

        //    return new Coordinate()
        //    {
        //        Latitude = lat,
        //        Longitude= lon,
        //        IsValid =  frame.Substring(45,1).Equals("1")
        //    };
        //}

        //private DateTime parseDateTime(string frame)
        //{
        //    //0123456789ABCD
        //    //PC090415202111...
        //    return new DateTime(int.Parse("20"+frame.Substring(6, 2)), int.Parse(frame.Substring(4, 2)),int.Parse(frame.Substring(2, 2)), 
        //        int.Parse(frame.Substring(8, 2)), int.Parse(frame.Substring(10, 2)),int.Parse(frame.Substring(12, 2)));
        //}

        //public Match SplitCaesatFrames(string frames)
        //{
        //    var match = Regex.Match(frames, @"([A-Za-z0-9\-\+\.]+)",RegexOptions.IgnoreCase);
        //    return match;
        //    //return frames.Split('|').ToList();
        //}

        //public IEnumerable<CaesatFrame> Parse(string receivedData)
        //{
        //    Logger.DebugFormat("Parse frames: {0} ",receivedData);

        //    var caestaFrameList = new List<CaesatFrame>();

        //    var match = SplitCaesatFrames(receivedData);

        //    while (match.Success)
        //    {
        //        caestaFrameList.Add(ParseCaesatFrame(match.Value));
        //        match = match.NextMatch();
        //    }

        //    return caestaFrameList;
        //}
        #endregion
    }
}
