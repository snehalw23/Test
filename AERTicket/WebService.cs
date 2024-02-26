using AERTicketWebService.Services;
using InternalClasses.Request;
using InternalClasses.Response;
using InternalClasses.RQ;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using static AERTicketWebService.AERTicket.AERTicket_Common;
using static AERTicketWebService.AERTicket.AERTicket_Common.SearchResponse;
using static AERTicketWebService.Common.Common;
using Vendor = InternalClasses.Common.Vendor;

namespace AERTicketWebService.AERTicket
{
    public class WebService
    {
        AERTicketRequestBuilder aerTicketRequest = new AERTicketRequestBuilder();
        AERTicketResponseBuilder aerTicketResponse = new AERTicketResponseBuilder();
        string requestData = string.Empty;
        string tokenResponseData = string.Empty;
        string sellRequestResponseData = string.Empty;
        string signature = string.Empty;
        AvailabilityRS responseData = new AvailabilityRS();
        string url;
        SellRS sellResponseData = new SellRS();
        string responsedata = string.Empty;
        BookingRS bookingResponse = new BookingRS();

        #region AERTicketAvailability
        public AvailabilityRS AERTicketAvailability(AvailabilityRQ request,string userName,string password)
        {
            try
            {
           
            var Officeidkey = Log.Base64Encode(request.Signature.OfficeID);
            requestData = aerTicketRequest.AvailabilitySearchRequest(request);
            url = WebApi.APIUrl("Search");

            tokenResponseData = WebApi.ExecutePostAPI(url, requestData, userName, password);
    
            #region log  
            Log.AddLogElastic(requestData, tokenResponseData, "API", "", request.Signature.OfficeID,Vendor.AERTicket.ToString(), "Availability", false,GetActions.Environment, request.Signature.TrackID);
            #endregion

            responseData = aerTicketResponse.AvailabilitySearchResponse(tokenResponseData, request, Officeidkey);

            responseData.Signature.TrackID = request.Signature.TrackID + "_" + Officeidkey;
                
            #region log        
                Log.AddLogElastic(request, responseData, "Trvlnxt", "", request.Signature.OfficeID, Vendor.SpiceJet.ToString(), "Availability", false, GetActions.Environment, responseData.Signature.TrackID);
            #endregion

            }
            catch (Exception ex)
            {
                responseData.ResponseStatusType = Log.BindErrorMessage("3", false, "System error occured,Please cordinate to the support team with trace id : " + request.Signature.TrackID);
                Log.AddErrorMessage(request, ex, "Getting Exception in AERTicketAvailability Method (MicroService) " + Log.JsonSerializeObject(responseData.ResponseStatusType), "Exception", "", request.Signature.OfficeID, Vendor.AIExpress.ToString(), "CreateAIExpressToken", GetActions.Environment);
                return responseData;
            }

            return responseData;
        }

        #endregion

        #region Sell
        public SellRS AERTicketSell(SellRQ sellRequest, string userName, string password)
        {
            try
            {
                var Officeidkey = Log.Base64Encode(sellRequest.Signature.OfficeID);
                
                requestData = aerTicketRequest.AERTicketSellRequest(sellRequest);

                url = WebApi.APIUrl("Sell");
                tokenResponseData = WebApi.ExecutePostAPI(url, requestData, userName, password);
               
                #region log  
                Log.AddLogElastic(requestData, tokenResponseData, "API", "", sellRequest.Signature.OfficeID, Vendor.AERTicket.ToString(), "AirSellFromRecommendation", false, GetActions.Environment, sellRequest.Signature.TrackID);
                #endregion

                sellResponseData = aerTicketResponse.SellVerifyResponse(tokenResponseData, sellRequest);

                sellResponseData.Signature.TrackID = sellRequest.Signature.TrackID + "_" + Officeidkey;
                #region log  
                Log.AddLogElastic(requestData, sellResponseData, "Trvlnxt", "", sellRequest.Signature.OfficeID, Vendor.AERTicket.ToString(), "AirSellFromRecommendation", false, GetActions.Environment, sellResponseData.Signature.TrackID);
                #endregion
                sellResponseData.Signature.TrackID = sellRequest.Signature.TrackID;
            }
            catch (Exception ex)
            {
                sellResponseData.ResponseStatusType = Log.BindErrorMessage("6", false, "System error occured,Please cordinate to the support team with trace id : " + sellRequest.Signature.TrackID);
                Log.AddErrorMessage(sellRequest, ex, "Getting Exception in AERTicketSell Method (MicroService) " + Log.JsonSerializeObject(responseData.ResponseStatusType), "Exception", "", sellRequest.Signature.OfficeID, Vendor.AERTicket.ToString(), "AERTicketSell", GetActions.Environment, sellRequest.Signature.TrackID);
               
            }

            return sellResponseData;
        }
        #endregion

        #region Booking
        public BookingRS AERTicketBooking(BookingRQ bookingRQ, string userName, string password)
        {
            try
            {
                var Officeidkey = Log.Base64Encode(bookingRQ.Signature.OfficeID);

                requestData = aerTicketRequest.AERTicketBookingRequest(bookingRQ);
                url = WebApi.APIUrl("Booking");

                responsedata = WebApi.ExecutePostAPI(url, requestData, userName, password);
                #region log  
                Log.AddLogElastic(requestData, responsedata, "API", "", bookingRQ.Signature.OfficeID, Vendor.AERTicket.ToString(), "PnrRetrieve", false, GetActions.Environment, bookingRQ.Signature.TrackID);
                #endregion

                bookingResponse = aerTicketResponse.BookingResponse(responsedata, bookingRQ);
                var TrackId = bookingRQ.Signature.TrackID + "_" + Officeidkey;
                #region log  
                Log.AddLogElastic(requestData, sellResponseData, "Trvlnxt", "", bookingRQ.Signature.OfficeID, Vendor.AERTicket.ToString(), "BookingTicket", false, GetActions.Environment, TrackId);
                #endregion
            }
            catch (Exception ex)
            {
                bookingResponse.ResponseStatusType = Log.BindErrorMessage("9", false, "System error occured,Please cordinate to the support team with trace id : " + bookingRQ.Signature.TrackID);
                Log.AddErrorMessage(bookingRQ, ex, "Getting Exception in AERTicketBooking Method (MicroService) " + Log.JsonSerializeObject(responseData.ResponseStatusType), "Exception", "", bookingRQ.Signature.OfficeID, Vendor.AERTicket.ToString(), "AERTicketSell", GetActions.Environment, bookingRQ.Signature.TrackID);

            }
            return bookingResponse;
        }
        #endregion
    }
}
