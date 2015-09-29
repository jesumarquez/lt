using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LogicTracker.App.Web.Api.Models;
using Logictracker.Tracker.Services;
using Logitracker.Codecs.Sitrack;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class ReceptionController : ApiController
    {
        public IReceptionService ReceptionService { get; set; }

        [Route("api/reception/positions")]
        [HttpPut]
        public IHttpActionResult ReceivePosition([FromBody]SitrackReceptionModel[] positions)
        {
            try
            {
                var frameList = SitrackModelToFrame(positions);
                ReceptionService.ParseSitrackPositions(frameList);
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
                //sitrackFrame.InputDate = position.input_date;
                sitrackFrame.Latitude = (float)position.latitude;
                sitrackFrame.Longitude = (float)position.longitude;
                sitrackFrame.Location = position.location;
                sitrackFrame.ReportDate = Convert.ToDateTime(position.reportDate);
                sitrackFrame.Speed = position.speed;
                //sitrackFrame.Validity = position.validity;

                frameList.Add(sitrackFrame);
            }
            return frameList;
        }
    }
}
