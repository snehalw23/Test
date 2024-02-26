using AERTicketWebService.Services;
using InternalClasses.Common;
using InternalClasses.Request;
using InternalClasses.Response;
using InternalClasses.RQ;
using Newtonsoft.Json;
using RestSharp;
using System.Linq;
using System.Reflection.Emit;
using static AERTicketWebService.AERTicket.AERTicket_Common;
using static AERTicketWebService.AERTicket.AERTicket_Common.SearchResponse;
using static AERTicketWebService.Common.Common;
using Vendor = InternalClasses.Common.Vendor;

namespace AERTicketWebService.AERTicket
{
    public class AERTicketRequestBuilder
    {
        #region Availability
        public string AvailabilitySearchRequest(AvailabilityRQ availabilityRQ)
        {
            try
            {
                #region Request

                var SearchRequest = new SearchRequest();
                var root = new SearchRequest.Root();

                root.segmentList = new List<SearchRequest.SegmentList>();
                root.segmentList.Add(new SearchRequest.SegmentList());
                root.segmentList[0].departure = new SearchRequest.Departure();
                root.segmentList[0].departure.iata = availabilityRQ.OriginDestination[0].Departure;
                root.segmentList[0].departure.geoObjectType = "AIRPORT";

                root.segmentList[0].destination = new SearchRequest.Destination();
                root.segmentList[0].destination.iata = availabilityRQ.OriginDestination[0].Arrival;
                root.segmentList[0].destination.geoObjectType = "AIRPORT";

                root.segmentList[0].departureDate = new SearchRequest.DepartureDate();
                root.segmentList[0].departureDate.year = availabilityRQ.OriginDestination[0].DepartureDateTime.Year;
                root.segmentList[0].departureDate.month = availabilityRQ.OriginDestination[0].DepartureDateTime.Month;
                root.segmentList[0].departureDate.day = availabilityRQ.OriginDestination[0].DepartureDateTime.Day;

                if (availabilityRQ.TripType == TripType.R.ToString())
                {
                    root.segmentList.Add(new SearchRequest.SegmentList());

                    root.segmentList[1].departure = new SearchRequest.Departure();
                    root.segmentList[1].departure.iata = availabilityRQ.OriginDestination[0].Arrival;
                    root.segmentList[1].departure.geoObjectType = "AIRPORT";

                    root.segmentList[1].destination = new SearchRequest.Destination();
                    root.segmentList[1].destination.iata = availabilityRQ.OriginDestination[0].Departure;
                    root.segmentList[1].destination.geoObjectType = "AIRPORT";

                    root.segmentList[1].departureDate = new SearchRequest.DepartureDate();
                    root.segmentList[1].departureDate.year = availabilityRQ.OriginDestination[0].ArrivalDateTime.Year;
                    root.segmentList[1].departureDate.month = availabilityRQ.OriginDestination[0].ArrivalDateTime.Month;
                    root.segmentList[1].departureDate.day = availabilityRQ.OriginDestination[0].ArrivalDateTime.Day;
                }


                root.requestPassengerTypeList = new List<SearchRequest.RequestPassengerTypeList>();
                root.requestPassengerTypeList.Add(new SearchRequest.RequestPassengerTypeList());
                root.requestPassengerTypeList[0].passengerTypeCode = "ADT";
                root.requestPassengerTypeList[0].count = availabilityRQ.PaxType.Adult;

                if (availabilityRQ.PaxType.Child > 0)
                {
                    root.requestPassengerTypeList.Add(new SearchRequest.RequestPassengerTypeList());
                    root.requestPassengerTypeList[1].passengerTypeCode = "CHD";
                    root.requestPassengerTypeList[1].count = availabilityRQ.PaxType.Child;
                }

                // Set Condition because get Index Error if only select INF not select CHD 18.09.2023
                if (availabilityRQ.PaxType.Infant> 0 && availabilityRQ.PaxType.Child == 0)
                {
                    root.requestPassengerTypeList.Add(new SearchRequest.RequestPassengerTypeList());
                    root.requestPassengerTypeList[1].passengerTypeCode = "INF";
                    root.requestPassengerTypeList[1].count = availabilityRQ.PaxType.Infant;
                }
                else if (availabilityRQ.PaxType.Infant > 0 && availabilityRQ.PaxType.Child > 0)
                {
                    root.requestPassengerTypeList.Add(new SearchRequest.RequestPassengerTypeList());
                    root.requestPassengerTypeList[2].passengerTypeCode = "INF";
                    root.requestPassengerTypeList[2].count = availabilityRQ.PaxType.Infant;
                }

                root.searchOptions = new SearchRequest.SearchOptions();
                string[] lstcabinClass = null;
           
                var travClass = (from x in availabilityRQ.FareNameDetails
                                 where x.ProductClass.Any(name => name.Equals("C")) || x.ProductClass.Any(name => name.Equals("W")) || x.ProductClass.Any(name => name.Equals("F"))
                                 select x);

                if (Convert.ToString(travClass) == "C")
                    lstcabinClass = new string[1] { "BUSINESS" };
                else if (Convert.ToString(travClass) == "W")
                    lstcabinClass = new string[1] { "ECONOMY_PREMIUM" };
                else if (Convert.ToString(travClass) == "F")
                    lstcabinClass = new string[1] { "FIRST" };
                else
                    lstcabinClass = new string[1] { "ECONOMY" };

                root.searchOptions.cabinClassList = lstcabinClass.ToList();

                root.searchOptions.nonStopFlightsOnly = false;
                root.searchOptions.directFlightsOnly = false;
                root.searchOptions.includedAirlineList = null;
                root.searchOptions.excludedAirlineList = null;
                //root.searchOptions.maxFares = 0;

                root.searchOptions.fareSourceList = new List<SearchRequest.FareSourceList>();
                root.searchOptions.fareSourceList.Add(new SearchRequest.FareSourceList());
                root.searchOptions.fareSourceList[0].id = "FSC_IATA";
                root.searchOptions.fareSourceList.Add(new SearchRequest.FareSourceList());
                root.searchOptions.fareSourceList[1].id = "FSC_NEGO";
                root.searchOptions.fareSourceList.Add(new SearchRequest.FareSourceList());
                root.searchOptions.fareSourceList[2].id = "FSC_CONSO";
                root.searchOptions.fareSourceList.Add(new SearchRequest.FareSourceList());
                root.searchOptions.fareSourceList[3].id = "FSC_WEB";

                root.searchOptions.closedUserGroupTypeList = new List<SearchRequest.ClosedUserGroupTypeList>();
                root.searchOptions.closedUserGroupTypeList.Add(new SearchRequest.ClosedUserGroupTypeList());
                root.searchOptions.closedUserGroupTypeList[0].code = "ALL";

                root.searchOptions.connectingTimeFilter = new SearchRequest.ConnectingTimeFilter();
                root.searchOptions.connectingTimeFilter.allowAirportChange = true;

                var mySerializedClass = JsonConvert.SerializeObject(root);

                return Convert.ToString(mySerializedClass);
                #endregion
            }
            catch (Exception ex)
            {
                Log.AddErrorMessage(availabilityRQ, ex, "Getting Exception in AvailabilitySearchRequest Method", "Exception", "", availabilityRQ.Signature.OfficeID, Vendor.AERTicket.ToString(), "AvailabilitySearchRequest", GetActions.Environment, availabilityRQ.Signature.TrackID);
                return ex.Message;
            }

          
        }
        #endregion

        #region Sell
        public string AERTicketSellRequest(SellRQ sellRequest)
        {
            try
            {
                #region Flight List  Fare List

                var flightIdList = new List<string>();
                var FareSellKey = new List<string>();

                if (sellRequest.Flights[0].Segments != null)
                {
                    foreach (var item in sellRequest.Flights[0].Segments)
                    {
                        flightIdList.Add(sellRequest.Flights[0].JourneyKey);
                        FareSellKey.Add(item.FareSellKey);
                    }
                }
                if (sellRequest.Flights.Count > 1)
                {
                    if (sellRequest.Flights[1].Segments != null)
                    {
                        foreach (var item in sellRequest.Flights[1].Segments)
                        {
                            flightIdList.Add(sellRequest.Flights[1].JourneyKey);
                            FareSellKey.Add(item.FareSellKey);
                        }
                    }
                }

                string strItineraryId = string.Join("\",\n\t\t\"", flightIdList.Distinct());

                #endregion
                string response = Convert.ToString("{\n\t\"fareId\": \"" + FareSellKey.FirstOrDefault() +
                         "\",\n\t\"itineraryIdList\": [\n\t\t\"" + strItineraryId + "\"\n\t]\n}");


                return response;
            }
            catch(Exception ex)
            {
                Log.AddErrorMessage(sellRequest, ex, "Getting Exception in AERTicketSellRequest Method", "Exception", "", sellRequest.Signature.OfficeID, Vendor.AERTicket.ToString(), "AERTicketSellRequest", GetActions.Environment, sellRequest.Signature.TrackID);
                return ex.Message;
            }
        }
        #endregion

        #region Booking
        public string AERTicketBookingRequest(BookingRQ bookingRQ)
        {
            var requestData = "";
            try
            {
                var root = new BookingRequest.Root();
                root.fareId = bookingRQ.Flights[0].Segments[0].FareSellKey;
                root.billingInformation = new BookingRequest.BillingInformation();
                root.billingInformation.email = bookingRQ.EmailId;
                root.billingInformation.city = "Mumbai";
                root.billingInformation.country = "India";
                root.billingInformation.street = "Andheri";
                root.billingInformation.zipCode = "400059";
                root.billingInformation.lastName = bookingRQ.Passengers[0].LastName;
                root.billingInformation.firstName = bookingRQ.Passengers[0].FirstName;
                root.billingInformation.phoneNumber = bookingRQ.ContactNo;

                //root.billingInformation.ruleSetBookingList = new List<BookingRequest.RuleSetBookingList>();
                //root.billingInformation.ruleSetBookingList.Add(new BookingRequest.RuleSetBookingList());
                //root.billingInformation.ruleSetBookingList[0].name = "BILLING_ADDRESS_CITY";
                //root.billingInformation.ruleSetBookingList[0].value = "Mumbai";

                root.passengerList = new List<BookingRequest.PassengerList>();

                for (int i = 0; i < bookingRQ.Passengers.Count; i++)
                {
                    root.passengerList.Add(new BookingRequest.PassengerList());
                    root.passengerList[i].id = i + 1;
                    root.passengerList[i].lastName = bookingRQ.Passengers[i].LastName;
                    root.passengerList[i].firstName = bookingRQ.Passengers[i].FirstName;

                    if (bookingRQ.Passengers[i].PaxType.ToUpper() == "ADULT" || bookingRQ.Passengers[i].PaxType.ToUpper() == "ADT")
                    {
                        root.passengerList[i].passengerTypeCode = "ADT";
                        if (bookingRQ.Passengers[i].Title == "MR")
                            root.passengerList[i].title = "MR";
                        else
                            root.passengerList[i].title = "MRS";
                    }
                    else if (bookingRQ.Passengers[i].PaxType.ToUpper() == "CHILD" || bookingRQ.Passengers[i].PaxType.ToUpper() == "CHD")
                    {
                        root.passengerList[i].passengerTypeCode = "CHD";
                        root.passengerList[i].title = "CHD";
                    }
                    else
                    {
                        root.passengerList[i].passengerTypeCode = "INF";
                        root.passengerList[i].title = "INF";
                    }

                    root.passengerList[i].dateOfBirth = new BookingRequest.DateOfBirth();
                    DateTime dateTime = Convert.ToDateTime(bookingRQ.Passengers[i].DateOfBirth);
                    root.passengerList[i].dateOfBirth.day = dateTime.Day;
                    root.passengerList[i].dateOfBirth.month = dateTime.Month;
                    root.passengerList[i].dateOfBirth.year = dateTime.Year;
                    root.passengerList[i].gender = bookingRQ.Passengers[i].Gender.ToUpper();
                    root.passengerList[i].operationalContactData = new BookingRequest.OperationalContactData();
                    root.passengerList[i].operationalContactData.emailAddressRefused = false;
                    root.passengerList[i].operationalContactData.phoneNumberRefused = false;
                    //if (i == 0)
                    //{
                    root.passengerList[i].operationalContactData.emailAddress = bookingRQ.EmailId;
                    root.passengerList[i].operationalContactData.phoneNumber = bookingRQ.ContactNo;
                    //}
                    root.passengerList[i].travelDocument = new BookingRequest.TravelDocument();
                    root.passengerList[i].travelDocument.type = "PASSENGER_PASSPORT";
                    if (bookingRQ.Signature.string1 == "I")
                    {
                        root.passengerList[i].travelDocument.number = bookingRQ.Passengers[i].PassportDetail.PassportNo;
                        root.passengerList[i].travelDocument.expiration = new BookingRequest.Expiration();
                        root.passengerList[i].travelDocument.expiration.day = Convert.ToInt32("0" + bookingRQ.Passengers[i].PassportDetail.PassportExpiry?.Day);
                        root.passengerList[i].travelDocument.expiration.month = Convert.ToInt32("0" + bookingRQ.Passengers[i].PassportDetail.PassportExpiry?.Month);
                        root.passengerList[i].travelDocument.expiration.year = Convert.ToInt32("0" + bookingRQ.Passengers[i].PassportDetail.PassportExpiry?.Year);
                        root.passengerList[i].travelDocument.nationality = new BookingRequest.Nationality();
                        root.passengerList[i].travelDocument.nationality.iso = bookingRQ.Passengers[i].PassportDetail.Nationalty;
                        root.passengerList[i].travelDocument.issuingCountry = new BookingRequest.IssuingCountry();
                        root.passengerList[i].travelDocument.issuingCountry.iso = bookingRQ.Passengers[i].PassportDetail.PassportIssueCountry;
                    }
                    else
                    {
                        root.passengerList[i].travelDocument.number = "000000000001";
                        root.passengerList[i].travelDocument.expiration = new BookingRequest.Expiration();
                        root.passengerList[i].travelDocument.expiration.day = 01;
                        root.passengerList[i].travelDocument.expiration.month = 01;
                        root.passengerList[i].travelDocument.expiration.year = 1001;
                        root.passengerList[i].travelDocument.nationality = new BookingRequest.Nationality();
                        root.passengerList[i].travelDocument.nationality.iso = "IN";
                        root.passengerList[i].travelDocument.issuingCountry = new BookingRequest.IssuingCountry();
                        root.passengerList[i].travelDocument.issuingCountry.iso = "IN";
                    }
                    //root.passengerList[i].priceList
                }
                
                requestData = JsonConvert.SerializeObject(root);

            }
            catch(Exception ex)
            {
                Log.AddErrorMessage(bookingRQ, ex, "Getting Exception in AERTicketBookingRequest Method", "Exception", "", bookingRQ.Signature.OfficeID, Vendor.AERTicket.ToString(), "AERTicketBookingRequest", GetActions.Environment, bookingRQ.Signature.TrackID);
                return ex.Message;
            }
            return requestData;
        }
        #endregion
    }
}
