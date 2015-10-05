using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;
using log4net;
using LogicTracker.App.Web.Api.Models;
using Logictracker.Tracker.Services;
using Logitracker.Codecs.Sitrack;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class ReceptionController : ApiController
    {
        public IReceptionService ReceptionService { get; set; }
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReceptionController));

        [Route("api/reception/positions")]
        [HttpPut]
        public IHttpActionResult ReceivePosition([FromBody]SitrackReceptionModel[] positions)
        {
            Logger.Warn(positions.ToString());
            try
            {
                var frameList = SitrackModelToFrame(positions);
                // Comentado 4/10 por DAD para poder hacer andar el Build del VSO
                //ReceptionService.ParseSitrackPositions(frameList);
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
                //throw;
            }
            return Ok();
        }

        public List<SitrackFrame> SitrackModelToFrame(SitrackReceptionModel[] positions)
        {
            var frameReport = new StringBuilder(positions.Length + " -> ");
            var frameList = new List<SitrackFrame>();
            foreach (var position in positions)
            {
                var sitrackFrame = new SitrackFrame();
                sitrackFrame.Course = position.course;
                sitrackFrame.DeviceId = position.device_id;              
                sitrackFrame.EventDesc = position.event_desc;
                sitrackFrame.EventId = position.event_id;
                sitrackFrame.HolderDomain = position.holder_domain;
                sitrackFrame.HolderId = position.holder_id;
                sitrackFrame.HolderName = position.holder_name;
                sitrackFrame.Id = position.id;
                sitrackFrame.Latitude = (float)position.latitude;
                sitrackFrame.Longitude = (float)position.longitude;
                sitrackFrame.Location = position.location;
                sitrackFrame.ReportDate = Convert.ToDateTime(position.reportDate);
                sitrackFrame.Speed = position.speed;

                frameReport.AppendFormat("Course : {0}, ", sitrackFrame.Course);
                frameReport.AppendFormat("DeviceId : {0}, ", sitrackFrame.DeviceId);
                frameReport.AppendFormat("EventDesc : {0}, ", sitrackFrame.EventDesc);
                frameReport.AppendFormat("EventId : {0}, ", sitrackFrame.EventId);
                frameReport.AppendFormat("HolderDomain : {0}, ", sitrackFrame.HolderDomain);
                frameReport.AppendFormat("HolderId : {0}, ", sitrackFrame.HolderId);
                frameReport.AppendFormat("HolderName: {0}, ", sitrackFrame.HolderName);
                frameReport.AppendFormat("Id : {0}, ", sitrackFrame.Id);
                frameReport.AppendFormat("Lat: {0}, ", sitrackFrame.Latitude);
                frameReport.AppendFormat("Lon : {0}, ", sitrackFrame.Longitude);
                frameReport.AppendFormat("Location : {0}, ", sitrackFrame.Location);
                frameReport.AppendFormat("ReportDate : {0}, ", sitrackFrame.ReportDate);
                frameReport.AppendFormat("Speed : {0} ", sitrackFrame.Speed);

                Logger.InfoFormat("[ {0} ]", frameReport);
                //sitrackFrame.Validity = position.validity;
                //sitrackFrame.InputDate = position.input_date;

                frameList.Add(sitrackFrame);
            }
            return frameList;
        }
    }
}
