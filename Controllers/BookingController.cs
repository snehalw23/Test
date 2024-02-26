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
    public class BookingController : ControllerBase
    {
        [HttpPost]
        [Route("Booking")]
        public string PNRCreation_OneWay_Agent(BookingRQ bookingRequest)
        {
            BookingRS bookingResponse = new BookingRS();
            try
            {
                #region log
                Log.FromDestination = bookingRequest.Flights[0].OriginDestination.Departure;
                Log.ToDestination = bookingRequest.Flights[0].OriginDestination.Arrival;
                Log.DepartureDateTime = bookingRequest.Flights[0].OriginDestination.DepartureDateTime;
                //Log.TrackID = bookingRequest.Signature.TrackID;
                //Log.OfficeID = request.Signature.OfficeID;
                //Log.APIName = Vendor.Amadeus.ToString();
                #endregion

                WebService webService = new WebService();
                bookingResponse = webService.AERTicketBooking(bookingRequest, bookingRequest.Signature.OfficeID, bookingRequest.Signature.Password);
            }
            catch (Exception ex)
            {
                bookingResponse.ResponseStatusType = Log.BindErrorMessage("8", false, "System error occured,Please cordinate to the support team with trace id : " + bookingRequest.Signature.TrackID);
                Log.AddErrorMessage(bookingRequest, ex, "Getting PNRCreation_OneWay_Agent in Booking Method" + Log.JsonSerializeObject(bookingResponse.ResponseStatusType), "Exception", "", bookingRequest.Signature.OfficeID, Vendor.AERTicket.ToString(), "PNRCreation_OneWay_Agent", GetActions.Environment, bookingRequest.Signature.TrackID);
            }
            return JsonConvert.SerializeObject(bookingResponse);
        }
    }
}
