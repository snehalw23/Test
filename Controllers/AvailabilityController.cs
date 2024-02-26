using AERTicketWebService.AERTicket;
using AERTicketWebService.Services;
using InternalClasses.Response;
using InternalClasses.RQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static AERTicketWebService.Common.Common;
using Vendor = InternalClasses.Common.Vendor;

namespace AERTicketWebService.Controllers
{
    [Route("ws/aerticket")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        [HttpPost]
        [Route("Availability")]
        public string Travelsearch(AvailabilityRQ request)
        {
            AvailabilityRS Response = new AvailabilityRS();
            try
            {
                #region log
                Log.FromDestination = request.OriginDestination[0].Departure;
                Log.ToDestination = request.OriginDestination[request.OriginDestination.Count - 1].Arrival;
                Log.DepartureDateTime = request.OriginDestination[0].DepartureDateTime;
                #endregion

                WebService webService = new WebService();
                Response = webService.AERTicketAvailability(request,request.Signature.OfficeID, request.Signature.Password);

            }
            catch (Exception ex)
            {
                Response.ResponseStatusType = Log.BindErrorMessage("2", false, "System error occured,Please cordinate to the support team with trace id : " + request.Signature.TrackID);
                Log.AddErrorMessage(request, ex, "Getting Exception in Travelsearch Method (MicroService) " + Log.JsonSerializeObject(Response.ResponseStatusType), "Exception", "", request.Signature.OfficeID, Vendor.AERTicket.ToString(), "Availability", GetActions.Environment, request.Signature.TrackID);
            }
            return JsonConvert.SerializeObject(Response);
        }

    }
}
