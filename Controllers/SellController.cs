using AERTicketWebService.AERTicket;
using AERTicketWebService.Services;
using InternalClasses.Request;
using InternalClasses.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static AERTicketWebService.Common.Common;
using Vendor = InternalClasses.Common.Vendor;


namespace AERTicketWebService.Controllers
{
    [Route("ws/aerticket")]
    [ApiController]
    public class SellController : ControllerBase
    {
        [HttpPost]
        [Route("AirSell")]
        public string Air_SellFromRecommendation_Both(SellRQ sellRequest)
        {
            SellRS Response = new SellRS();
            try
            {
                #region log
                Log.FromDestination = sellRequest.Flights[0].OriginDestination.Departure;
                Log.ToDestination = sellRequest.Flights[0].OriginDestination.Arrival;
                Log.DepartureDateTime = sellRequest.Flights[0].OriginDestination.DepartureDateTime;
                #endregion

                WebService webService = new WebService();
                Response = webService.AERTicketSell(sellRequest, sellRequest.Signature.OfficeID, sellRequest.Signature.Password);
            }
            catch (Exception ex)
            {
                Response.ResponseStatusType = Log.BindErrorMessage("5", false, "System error occured,Please cordinate to the support team with trace id : " + sellRequest.Signature.TrackID);
                Log.AddErrorMessage(sellRequest, ex, "Getting Exception in Air_SellFromRecommendation_Both Method (MicroService) " + Log.JsonSerializeObject(Response.ResponseStatusType), "Exception", "", sellRequest.Signature.OfficeID, Vendor.AERTicket.ToString(), "Air_SellFromRecommendation_Both", GetActions.Environment, sellRequest.Signature.TrackID);
            }
            return JsonConvert.SerializeObject(Response);
        }

    }
}
