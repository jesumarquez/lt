using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Logictracker.Tracker.Parser.Caesat;
using NUnit.Framework;

namespace Logictracker.Tracker.Test.Protocol.Parser.Caesat
{
    [TestFixture]
    public class CaesatParserTest
    {
        public CaesatParser CaesatParser;

        [SetUp]
        public void Setup()
        {
            CaesatParser = new CaesatParser();
        }

        [Test]
        public void ParseOneFrameTest()
        {
            var caesatFrame = CaesatParser.ParseCaesatFrame(GetOneFrame());
            
            Assert.IsNotNull(caesatFrame);
            Assert.IsTrue("PC".Equals(caesatFrame.MessageType), "Message type no match");
            Assert.IsTrue(new DateTime(2015,04,09,20,21,11).Equals(caesatFrame.DateTime),"Datetime no match");
            Assert.IsTrue("KPB696".Equals(caesatFrame.LicensePlate), "License plate no match");
            Assert.IsTrue(Math.Abs(-34.77947 - caesatFrame.Coordinate.Latitude) < 0.05, "Latitude no match");
            Assert.IsTrue(Math.Abs(-058.26496 - caesatFrame.Coordinate.Longitude) < 0.05, "Longitude no match");
            Assert.IsTrue(0 == caesatFrame.Speed, "Speed no match");
            Assert.IsTrue(45 == caesatFrame.Meaning, "Meaning no match");
            Assert.IsTrue(true.Equals(caesatFrame.Coordinate.IsValid), "Validity no match");
            Assert.IsTrue(1 == caesatFrame.EventCode, "Event code no match");
            Assert.IsTrue(0==caesatFrame.Temperature1, "Temperature 1 no match");
            Assert.IsTrue(1==caesatFrame.Temperature2, "Temperature 2 no match");
            Assert.IsTrue(-1==caesatFrame.Temperature3, "Temperature 3 no match");

        }

        [Test]
        public void SplitFramesTest()
        {
            Match caesatFrames = CaesatParser.SplitCaesatFrames(GetManyFrames());
            
            Assert.IsTrue(2==caesatFrames.Groups.Count);
        }

        [Test]
        public void ParseManyFrames()
        {
            var frames = CaesatParser.Parse(GetManyFrames());
            
            Assert.IsTrue(frames.Count() == 5);
            Assert.IsTrue(frames.GetType() == typeof(List<CaesatFrame>));
        }

        [Test]
        public void ParseBadFrame()
        {
            var raw = "PC150515155220";
            var frames = CaesatParser.Parse(raw).ToList();

            Assert.IsTrue(frames.Count() ==1);
            Assert.IsTrue(frames[0] == null);
        }

        private string GetManyFrames()
        {
            return @"PC150415184316KQD370-34.70594-058.49942000045101+00+00+00|
                        PC150415184348KPB696-34.81253-058.19249000045101+00+00+00|
                        PC150415184352FNQ168-34.81228-058.19286000000101+00+00+00|
                        PC150415184402LZP774-34.70600-058.49948000315101+00+00+00|
                        PC150415184412NBN015-34.41278-058.97967000315108+00+00+00|";
        }

        private static string GetOneFrame()
        {
            return "PC090415202111KPB696-34.77947-058.26496000045101+00+01-01|";
        }

        private string GetExampleData()
        {
            return
                @"PC090415202111KPB696-34.77947-058.26496000045101+00+00+00|PC090415202127C38433-34.81323-058.19607000000101+00+00+00|PC090415202137GTZ780-34.81262-058.19275000000101+00+00+00|PC090415202151KQI440-34.81241-058.19266000045101+00+00+00|PC090415202200FNQ168-34.81228-058.19286000090101+00+00+00|PC090415202239LZP774-34.81215-058.19315000045101+00+00+00|PC090415202253KQD370-34.81203-058.19321000045101+00+00+00|PC090415202302KPB696-34.78163-058.26150032180101+00+00+00|PC090415202305NBN015-34.81226-058.19289000000101+00+00+00|PC090415202311KPB696-34.78216-058.26098024090101+00+00+00|PC090415202313KPB696-34.78218-058.26081024090101+00+00+00|PC090415202334KPB696-34.78252-058.25875032135101+00+00+00|PC090415202357LED017-34.81253-058.19272000000101+00+00+00|PC090415202406C38433-34.81323-058.19607000000108+00+00+00|PC090415202412GTZ780-34.81241-058.19263000045109+00+00+00|PC090415202428C38433-34.81323-058.19607000000101+00+00+00|PC090415202437GTZ780-34.81241-058.19261000045101+00+00+00|PC090415202453FYZ069-34.81218-058.19306000045101+00+00+00|PC090415202453KQD370-34.81205-058.19321000045101+00+00+00|PC090415202459FNQ168-34.81228-058.19286000090101+00+00+00|PC090415202513CFH414-34.81255-058.19235000225101+00+00+00|PC090415202513KPB696-34.78533-058.25429000045101+00+00+00|PC090415202539LZP774-34.81215-058.19315000045101+00+00+00|PC090415202604NBN015-34.81226-058.19289000000101+00+00+00|PC090415202650KQI440-34.81239-058.19266000045101+00+00+00|PC09041520";
        }
    }
}
