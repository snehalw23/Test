using AERTicketWebService.Services;
using InternalClasses.Common;
using InternalClasses.Request;
using InternalClasses.Response;
using InternalClasses.RQ;
using InternalClasses.RQ.InternalClasses;
using Newtonsoft.Json;
using System.Globalization;
using static AERTicketWebService.AERTicket.AERTicket_Common;
using static AERTicketWebService.Common.Common;
using Vendor = InternalClasses.Common.Vendor;

namespace AERTicketWebService.AERTicket
{
    public class AERTicketResponseBuilder
    {
        public AvailabilityRS AvailabilitySearchResponse(string responseData, AvailabilityRQ availabilityRQ, string officeidkey)
        {
            AvailabilityRS availRS = new AvailabilityRS();
            try
            {
                ResponseStatusType responseStatusType = new ResponseStatusType();
                responseStatusType.Id = "1";
                responseStatusType.Success = true;
                responseStatusType.Message = "";
                availRS.ResponseStatusType = responseStatusType;
                SearchResponse.Root resp = JsonConvert.DeserializeObject<SearchResponse.Root>(responseData);
                //string flowId = response.Headers.ToList().Find(x => x.Name == "apihubFlowId").Value.ToString();

                #region Binding

                availRS.Signature = availabilityRQ.Signature;

                List<Flight> lstJourneyOneWay = new List<Flight>();
                List<Flight> lstJourneyReturn = new List<Flight>();

                if (resp != null)
                {
                    availRS.Journeys = new List<Journey>();
                    Journey journeyReturn = new Journey();
                    Journey journeyOneway = new Journey();
                    if (resp.success && resp.availableFareList.Count() > 0)
                    {
                        for (int i = 0; i < resp.availableFareList.Count(); i++)
                        {
                           
                            for (int j = 0; j < resp.availableFareList[i].legList.Count(); j++)
                            {

                                for (int z = 0; z < resp.availableFareList[i].legList[j].itineraryList.Count(); z++)
                                {
                                    string JourneyKey = "";
                                    JourneyKey = resp.availableFareList[i].legList[j].itineraryList[z].id;
                                    string flightNumbers = string.Empty;
                                    var flyingTimeInMinutes = resp.availableFareList[i].legList[j].itineraryList[z].flyingTimeInMinutes;
                                    TimeSpan ts = TimeSpan.FromMinutes(flyingTimeInMinutes);
                                    Flight flight = new Flight();

                                    List<Segment> OneWayFlight = new List<Segment>();
                                    List<Segment> ReturnFlight = new List<Segment>();

                                    flight.OriginDestination = new OriginDestination();

                                    for (int seg = 0; seg < resp.availableFareList[i].legList[j].itineraryList[z].segmentList.Count(); seg++)
                                    {
                                        

                                        var segment = resp.availableFareList[i].legList[j].itineraryList[z].segmentList[seg];
                                        if (j == 0)
                                        {
                                            #region Single
                                            flight.ValidatingCarrier = resp.availableFareList[i].validatingAirline.iata;
                                            flight.PCC = availabilityRQ.Signature.OfficeID;
                                            flight.SupplierName = "AerTicket";

                                            Segment flightsingle = new Segment();
                                            flightsingle.OriginDestination = new OriginDestination();
                                            flightsingle.Baggage = new Baggage();
                                            //if (FSPara.InAirlineList.Contains(segment.marketingAirline.iata) || FSPara.InAirlineList == "ALL")
                                            //{

                                            //flightsingle.WebServiceFlag = "AERTicket";
                                            string dateString = segment.departureDate.day.ToString("d2") + "/" + segment.departureDate.month.ToString("d2") + "/" + segment.departureDate.year + " " + segment.departureTimeOfDay.hour.ToString("d2") + ":" + segment.departureTimeOfDay.minute.ToString("d2");
                                            DateTime dtDepartureDateTime = DateTime.ParseExact(dateString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                                            flightsingle.OriginDestination.DepartureDateTime = dtDepartureDateTime;
                                            string dateStringArr = segment.arrivalDate.day.ToString("d2") + "/" + segment.arrivalDate.month.ToString("d2") + "/" + segment.arrivalDate.year + " " + segment.arrivalTimeOfDay.hour.ToString("d2") + ":" + segment.arrivalTimeOfDay.minute.ToString("d2");
                                            DateTime dtArrivalDateTime = DateTime.ParseExact(dateStringArr, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                                            flightsingle.OriginDestination.ArrivalDateTime = dtArrivalDateTime;
                                            TimeSpan tsStopover = dtArrivalDateTime - dtDepartureDateTime;

                                            //flightsingle.TotalTime = Convert.ToString(ts);
                                            //flightsingle.TotalTimeStopOver = Convert.ToString(tsStopover);
                                            flightsingle.OriginDestination.Departure = segment.departure.iata;
                                            flightsingle.OriginDestination.Arrival = segment.destination.iata;
                                            flightsingle.FlightNumber = segment.flightNumber.ToString();
                                             flightNumbers += "_" + flightsingle.FlightNumber;
                                            flightsingle.OriginDestination.TotalTime = Convert.ToString(ts);
                                            //if (segment.equipmentType != null)
                                            //    flightsingle.Equipment = segment.equipmentType.code;
                                            //else
                                            //    flightsingle.Equipment = "";
                                            flightsingle.CabinBaggage = "7 KG";
                                            flightsingle.Carrier = segment.marketingAirline.iata;
                                            flightsingle.OperationgCarrier = segment.operatingAirline.iata;
                                            flightsingle.MarketingCarrier = segment.marketingAirline.iata;
                                            
                                            flightsingle.ProductClass = segment.fareBase;

                                            flightsingle.Cabin = segment.bookingClassCode + "-" + segment.cabinClass;
                                            //flightsingle.Sector = availabilityRQ.Sector;

                                            // Code Modify by Hardik 18.10.2023 Get NULL baggageAllowance
                                            if (segment.baggageAllowance != null)
                                            {
                                                flightsingle.Baggage.Weight = segment.baggageAllowance.quantity.ToString();
                                                flightsingle.Baggage.Unit = segment.baggageAllowance.unit;
                                            }
                                            else
                                            {
                                                flightsingle.Baggage.Weight = "";
                                                flightsingle.Baggage.Unit = "";
                                            }
                                            var paxstring = availabilityRQ.PaxType.Adult + "ADT_" + availabilityRQ.PaxType.Child + "CHD_" + availabilityRQ.PaxType.Infant + "INF";
                                           
                                                flight.FlightKey = "66" + "_" + flight.OriginDestination.Departure + "_" + flight.OriginDestination.Arrival
                                                                    + "_" + paxstring + flightNumbers + "_" + Common.Common.GetGUID() + "_$MR_" + officeidkey;
                                            
                                            flightsingle.FareSellKey = resp.availableFareList[i].fareId;
                                            OneWayFlight.Add(flightsingle);
                                            #endregion
                                        }
                                        else
                                        {
                                            #region ReturnJourney
                                            Segment flightreturn = new Segment();
                                            string flightNumbers1 = string.Empty;
                                            flightreturn.OriginDestination = new OriginDestination();
                                            flightreturn.Baggage = new Baggage();
                                            //if (FSPara.InAirlineList.Contains(segment.marketingAirline.iata) || FSPara.InAirlineList == "ALL")
                                            //{

                                            string dateString = segment.departureDate.day.ToString("d2") + "/" + segment.departureDate.month.ToString("d2") + "/" + segment.departureDate.year + " " + segment.departureTimeOfDay.hour.ToString("d2") + ":" + segment.departureTimeOfDay.minute.ToString("d2");
                                            DateTime dtDepartureDateTime = DateTime.ParseExact(dateString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                                            flightreturn.OriginDestination.DepartureDateTime = dtDepartureDateTime;
                                            string dateStringArr = segment.arrivalDate.day.ToString("d2") + "/" + segment.arrivalDate.month.ToString("d2") + "/" + segment.arrivalDate.year + " " + segment.arrivalTimeOfDay.hour.ToString("d2") + ":" + segment.arrivalTimeOfDay.minute.ToString("d2");
                                            DateTime dtArrivalDateTime = DateTime.ParseExact(dateStringArr, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                                            flightreturn.OriginDestination.ArrivalDateTime = dtArrivalDateTime;
                                            TimeSpan tsStopover = dtArrivalDateTime - dtDepartureDateTime;

                                            //flightsingle.TotalTime = Convert.ToString(ts);
                                            //flightsingle.TotalTimeStopOver = Convert.ToString(tsStopover);
                                            flightreturn.OriginDestination.Departure = segment.departure.iata;
                                            flightreturn.OriginDestination.Arrival = segment.destination.iata;
                                            flightreturn.FlightNumber = segment.flightNumber.ToString();
                                            flightNumbers1 += "_" + flightreturn.FlightNumber;
                                            flightreturn.OriginDestination.TotalTime = Convert.ToString(ts);

                                            //if (segment.equipmentType != null)
                                            //    flightsingle.Equipment = segment.equipmentType.code;
                                            //else
                                            //    flightsingle.Equipment = "";

                                            flightreturn.Carrier = segment.marketingAirline.iata;
                                            flightreturn.OperationgCarrier = segment.operatingAirline.iata;
                                            //flightsingle.AirlineName = segment.marketingAirline.name;
                                            //flightsingle.OperatingCarrier = segment.operatingAirline.name;
                                            flightreturn.ProductClass = segment.fareBase;

                                            flightreturn.Cabin = segment.bookingClassCode + "-" + segment.cabinClass;
                                            //flightsingle.Sector = availabilityRQ.Sector;
                                            flightreturn.CabinBaggage = "7 KG";
                                            // Code Modify by Hardik 18.10.2023 Get NULL baggageAllowance
                                            if (segment.baggageAllowance != null)
                                            {
                                                flightreturn.Baggage.Weight = segment.baggageAllowance.quantity.ToString();
                                                flightreturn.Baggage.Unit = segment.baggageAllowance.unit;
                                            }
                                            else
                                            {
                                                flightreturn.Baggage.Weight = "";
                                                flightreturn.Baggage.Unit = "";
                                            }
                                            var paxstringR = availabilityRQ.PaxType.Adult + "ADT_" + availabilityRQ.PaxType.Child + "CHD_" + availabilityRQ.PaxType.Infant + "INF";

                                            flight.FlightKey = "66" + "_" + flight.OriginDestination.Departure + "_" + flight.OriginDestination.Arrival
                                                                + "_" + paxstringR + flightNumbers1 + "_" + Common.Common.GetGUID() + "_$MR_" + officeidkey;

                                            flightreturn.FareSellKey = resp.availableFareList[i].fareId;
                                            ReturnFlight.Add(flightreturn);
                                            #endregion
                                            //}
                                        }
                                    }

                                    #region Fare
                                    //if (j == 0)
                                    //{
                                    //flightsingle.JourneySellKey = JourneySellKey;


                                    decimal basicfareCHD = 0, totaltaxCHD = 0, totalpriceCHD = 0;
                                    decimal basicfareINF = 0, totaltaxINF = 0, totalpriceINF = 0;

                                    decimal totalBasicfare = 0, TotalTax = 0, totalFare = 0;
                                    decimal basicfareADT = 0, totaltaxADT = 0, totalpriceADT = 0;
                                    decimal TOTAL_TAX_ADT = 0, TOTAL_TAX_INF = 0, TOTAL_TAX_CHD = 0, AGENCY_PURCHASE_PRICE_ADT = 0, 
                                        AGENCY_PURCHASE_PRICE_CHD = 0, AGENCY_PURCHASE_PRICE_INF = 0,AGENCY_MARGIN_ADT = 0, 
                                        AGENCY_MARGIN_CHD = 0, AGENCY_MARGIN_INF = 0, Extra_ADT = 0, Extra_CHD = 0, Extra_INF = 0;
                                   
                                    flight.FlightPricingInfo = new FlightPricingInfo();
                                    flight.FlightPricingInfo.PaxFareDetails = new List<FareDetail>();
                                    List<TaxDetail> taxDetailslst = new List<TaxDetail>();
                                    
                                    if (resp.availableFareList[i]?.passengerTypeFareList.Count > 0)
                                    {
                                        foreach (var item in resp.availableFareList[i]?.passengerTypeFareList)
                                        {
                                            FareDetail fareDetail = new FareDetail();
                                            TaxDetail taxDetails = new TaxDetail();
                                            fareDetail.PaxTaxDetails = new List<TaxDetail>();

                                            if (item.passengerTypeCode == "ADT")
                                            {
                                                foreach (var pl in item.priceList)
                                                {
                                                    if (pl.type == "TOTAL_TAX")
                                                    {
                                                        taxDetails = new TaxDetail();
                                                        totaltaxADT = Convert.ToDecimal(pl.value);
                                                        taxDetails.TaxCode = pl.type;
                                                        taxDetails.TaxAmount = Convert.ToDecimal(pl.value);
                                                        TOTAL_TAX_ADT = taxDetails.TaxAmount;
                                                        fareDetail.PaxTaxDetails.Add(taxDetails);
                                                    }
                                                    else if (pl.type == "AGENCY_PURCHASE_PRICE" || pl.type == "AGENCY_MARGIN")
                                                    {
                                                        basicfareADT += Convert.ToDecimal(pl.value);
                                                    }
                                                    else
                                                    {
                                                        taxDetails = new TaxDetail();
                                                        taxDetails.TaxCode = "Extra";
                                                        taxDetails.TaxAmount = Convert.ToDecimal(pl.value);
                                                        Extra_ADT = taxDetails.TaxAmount;
                                                        if (taxDetails.TaxAmount > 0)
                                                        {
                                                            fareDetail.PaxTaxDetails.Add(taxDetails);
                                                        }
                                                    }
                                                    
                                                }

                                                totalBasicfare += (basicfareADT * availabilityRQ.PaxType.Adult);

                                                TotalTax = (totaltaxADT * availabilityRQ.PaxType.Adult);
                                                totalpriceADT = (totaltaxADT + basicfareADT);
                                                totalFare = (totalpriceADT * availabilityRQ.PaxType.Adult);

                                                fareDetail.PaxType = item.passengerTypeCode;
                                                fareDetail.TotalFare = totalpriceADT;
                                                fareDetail.BasicFare = basicfareADT;
                                                fareDetail.TotalTax = totaltaxADT;


                                                flight.FlightPricingInfo.PaxFareDetails.Add(fareDetail);

                                            }

                                            if (item.passengerTypeCode == "CHD")
                                            {
                                                foreach (var pl in item.priceList)
                                                {
                                                    if (pl.type == "TOTAL_TAX")
                                                    {
                                                        taxDetails = new TaxDetail();
                                                        totaltaxCHD = Convert.ToDecimal(pl.value);
                                                        taxDetails.TaxCode = pl.type;
                                                        taxDetails.TaxAmount = Convert.ToDecimal(pl.value);
                                                        TOTAL_TAX_CHD = taxDetails.TaxAmount;
                                                        fareDetail.PaxTaxDetails.Add(taxDetails);
                                                    }
                                                    else if (pl.type == "AGENCY_PURCHASE_PRICE" || pl.type == "AGENCY_MARGIN")
                                                    {
                                                        basicfareCHD += Convert.ToDecimal(pl.value);
                                                    }
                                                    else
                                                    {
                                                        taxDetails = new TaxDetail();
                                                        taxDetails.TaxCode = "Extra";
                                                        taxDetails.TaxAmount = Convert.ToDecimal(pl.value);
                                                        Extra_CHD = taxDetails.TaxAmount;
                                                        if (taxDetails.TaxAmount > 0)
                                                        {
                                                            fareDetail.PaxTaxDetails.Add(taxDetails);
                                                        }
                                                    }
                                                   
                                                }

                                                TotalTax += (totaltaxCHD * availabilityRQ.PaxType.Child);
                                                totalpriceCHD = (basicfareCHD + totaltaxCHD);
                                                totalFare += (totalpriceCHD * availabilityRQ.PaxType.Child);
                                                totalBasicfare += (basicfareCHD * availabilityRQ.PaxType.Child);
                                                //flightsingle.BasicFare_CHD = (basicfareCHD);
                                                //flightsingle.TotalTax_CHD = (totaltaxCHD);
                                                //flightsingle.TotalPrice_CHD = (totalpriceCHD);
                                                //flightsingle.ExtraTax_CHD = totaltaxCHD;
                                                fareDetail.PaxType = item.passengerTypeCode;
                                                fareDetail.TotalFare = totalpriceCHD;
                                                fareDetail.BasicFare = basicfareCHD;
                                                fareDetail.TotalTax = totaltaxCHD;

                                                flight.FlightPricingInfo.PaxFareDetails.Add(fareDetail);

                                            }

                                            if (item.passengerTypeCode == "INF")
                                            {
                                                foreach (var pl in item.priceList)
                                                {
                                                    if (pl.type == "TOTAL_TAX")
                                                    {
                                                        taxDetails = new TaxDetail();
                                                        totaltaxINF = Convert.ToDecimal(pl.value);
                                                        taxDetails.TaxCode = pl.type;
                                                        taxDetails.TaxAmount = Convert.ToDecimal(pl.value);
                                                        TOTAL_TAX_INF = taxDetails.TaxAmount;
                                                        fareDetail.PaxTaxDetails.Add(taxDetails);
                                                    }
                                                    else if (pl.type == "AGENCY_PURCHASE_PRICE" || pl.type == "AGENCY_MARGIN")
                                                    {
                                                        basicfareINF += Convert.ToDecimal(pl.value);
                                                    }
                                                    else
                                                    {
                                                        taxDetails = new TaxDetail();
                                                        taxDetails.TaxCode = "Extra";
                                                        taxDetails.TaxAmount = Convert.ToDecimal(pl.value);
                                                        Extra_INF = taxDetails.TaxAmount;
                                                        if (taxDetails.TaxAmount > 0)
                                                        {
                                                            fareDetail.PaxTaxDetails.Add(taxDetails);
                                                        }
                                                    }
                                                }
                                                TotalTax += (totaltaxINF * availabilityRQ.PaxType.Infant);
                                                totalpriceINF += (basicfareINF + totaltaxINF);
                                                totalBasicfare += (basicfareINF * availabilityRQ.PaxType.Infant);
                                                totalFare += (totalpriceINF * availabilityRQ.PaxType.Infant);

                                                //flightsingle.BasicFare_INF = (basicfareINF);
                                                //flightsingle.TotalTax_INF = (totaltaxINF);
                                                //flightsingle.TotalPrice_INF = (totalpriceINF);
                                                //flightsingle.ExtraTax_INF = totaltaxINF;
                                                fareDetail.PaxType = item.passengerTypeCode;
                                                fareDetail.TotalFare = totalpriceINF;
                                                fareDetail.BasicFare = basicfareINF;
                                                fareDetail.TotalTax = totaltaxINF;

                                                flight.FlightPricingInfo.PaxFareDetails.Add(fareDetail);
                                            }

                                            #region FlightTaxDetails
                                            flight.FlightPricingInfo.FlightTaxDetails = new List<TaxDetail>();
                                            decimal TOTAL_TAX = TOTAL_TAX_ADT * availabilityRQ.PaxType.Adult + TOTAL_TAX_CHD * availabilityRQ.PaxType.Child + TOTAL_TAX_INF * availabilityRQ.PaxType.Infant;
                                           
                                            decimal Extra = Extra_ADT * availabilityRQ.PaxType.Adult + Extra_CHD * availabilityRQ.PaxType.Child + Extra_INF * availabilityRQ.PaxType.Infant;


                                            foreach (var pl in item.priceList)
                                            {
                                                taxDetails = new TaxDetail();
                                                if (pl.type == "TOTAL_TAX" && TotalTax >0 )
                                                {
                                                    taxDetails.TaxCode = pl.type;
                                                    taxDetails.TaxAmount = TOTAL_TAX;
                                                    flight.FlightPricingInfo.FlightTaxDetails.Add(taxDetails);
                                                }
                                                else if(pl.type == "AGENCY_PURCHASE_PRICE" || pl.type == "AGENCY_MARGIN")
                                                {
                                                    taxDetails = new TaxDetail();
                                                   
                                                }
                                                else if(Extra > 0)
                                                {
                                                    taxDetails.TaxCode = "Extra";
                                                    taxDetails.TaxAmount = Extra;
                                                    flight.FlightPricingInfo.FlightTaxDetails.Add(taxDetails);
                                                }
                                            }
                                            #endregion
                                            //flight.TotalFare += (totalFare);
                                            //flight.TotalTax += (TotalTax);
                                            //flight.BasicFare += (totalBasicfare);


                                        }
                                        flight.TotalFare += (totalFare);
                                        flight.TotalTax += (TotalTax);
                                        flight.BasicFare += (totalBasicfare);
                                    }

                                    if (resp.availableFareList[i].ruleSet.fareSourceList != null && resp.availableFareList[i].ruleSet.fareSourceList.Count > 0)
                                    {
                                        string FareType = resp.availableFareList[i].ruleSet.fareSourceList.FirstOrDefault().id.ToUpper();

                                        string FareIndicaters = string.Empty, FltFareName = string.Empty, farecolor = string.Empty, FareTypeNew = string.Empty;

                                        switch (FareType)
                                        {
                                            case "FSC_IATA":
                                                FareIndicaters = "Publish Fare";
                                                FltFareName = "Publish Fare";
                                                farecolor = "red-bg";
                                                FareTypeNew = "N";
                                                break;
                                            case "FSC_NEGO":
                                                FareIndicaters = "Private Fare";
                                                FltFareName = "Private Fare";
                                                farecolor = "blue-bg";
                                                FareTypeNew = "S";
                                                break;
                                            case "FSC_CONSO":
                                                FareIndicaters = "Conso Fare";
                                                FltFareName = "Conso Fare";
                                                farecolor = "orange-bg";
                                                FareTypeNew = "C";
                                                break;
                                            case "FSC_WEB":
                                                FareIndicaters = "Web Fare";
                                                FltFareName = "Web Fare";
                                                farecolor = "green-bg";
                                                FareTypeNew = "W";
                                                break;
                                        }

                                        // flightsingle.FareIndicaters = FareIndicaters;
                                        // flightsingle.FltFareName = FltFareName;
                                        //flightsingle.FareType = FareTypeNew;
                                        //flightsingle.FareColor = farecolor;
                                        flight.FareName = FltFareName;
                                    }

                                    if (j == 0)
                                    {
                                        flight.OriginDestination.Departure = availabilityRQ.OriginDestination[0].Departure;
                                        flight.OriginDestination.Arrival = availabilityRQ.OriginDestination[0].Arrival;
                                        flight.OriginDestination.DepartureDateTime = availabilityRQ.OriginDestination[0].DepartureDateTime;
                                        flight.OriginDestination.ArrivalDateTime = availabilityRQ.OriginDestination[0].ArrivalDateTime;
                                        flight.OriginDestination.TotalTime = availabilityRQ.OriginDestination[0].TotalTime;

                                        flight.Segments = OneWayFlight;
                                        flight.JourneyKey = JourneyKey;
                                    }
                                    else
                                    {
                                        flight.OriginDestination.Departure = availabilityRQ.OriginDestination[0].Arrival;
                                        flight.OriginDestination.Arrival = availabilityRQ.OriginDestination[0].Departure;
                                        flight.OriginDestination.DepartureDateTime = availabilityRQ.OriginDestination[0].ArrivalDateTime;
                                        flight.OriginDestination.ArrivalDateTime = availabilityRQ.OriginDestination[0].DepartureDateTime;
                                        flight.OriginDestination.TotalTime = availabilityRQ.OriginDestination[0].TotalTime;

                                        flight.Segments = ReturnFlight;
                                        flight.JourneyKey = JourneyKey;
                                    }
                                    //}
                                    #endregion

                                    if (j == 0)
                                    {
                                        lstJourneyOneWay.Add(flight);
                                        lstJourneyOneWay = lstJourneyOneWay.OrderBy(x => x.TotalFare).Take(200).ToList();
                                    }
                                    else
                                    {
                                        lstJourneyReturn.Add(flight);
                                        lstJourneyReturn = lstJourneyReturn.OrderBy(x => x.TotalFare).Take(200).ToList();
                                    }

                                    //}
                                }


                            }
                        }
                    }
                    journeyOneway.Flights = new List<Flight>();
                    journeyReturn.Flights = new List<Flight>();
                    if (availabilityRQ.TripType == TripType.R.ToString())
                    {
                        foreach (var ow in lstJourneyOneWay)
                        {
                            foreach (var rt in lstJourneyReturn)
                            {
                                if (ow.Segments[0].ProductClass == rt.Segments[0].ProductClass)
                                {
                                    //    if (ow.OriginDestination.ArrivalDateTime.AddHours(5) <  )
                                    //    {

                                    journeyOneway.Flights.Add(ow);

                                    journeyReturn.Flights.Add(rt);

                                //    }
                                }
                            }
                        }
                        journeyOneway.Flights = journeyOneway.Flights.Take(200).ToList();
                        availRS.Journeys.Add(journeyOneway);
                        journeyReturn.Flights = journeyReturn.Flights.Take(200).ToList();
                        availRS.Journeys.Add(journeyReturn);
                    }
                    else
                    {
                        journeyOneway.Flights = lstJourneyOneWay;
                        availRS.Journeys.Add(journeyOneway);
                    }
                    
                }
                #endregion
            }
            catch (Exception ex)
            {
                availRS.ResponseStatusType = Log.BindErrorMessage("4", false, "System error occured,Please cordinate to the support team with trace id : " + availabilityRQ.Signature.TrackID);
                Log.AddErrorMessage(availabilityRQ, ex, "Getting Exception in AvailabilitySearchResponse Method", "Exception", "", availabilityRQ.Signature.OfficeID, Vendor.AERTicket.ToString(), "AvailabilitySearchResponse", GetActions.Environment);
            }
            return availRS;
        }


        public SellRS SellVerifyResponse(string responseData, SellRQ sellRQ)
        {
            SellRS sellRS = new SellRS();

            ResponseStatusType responseStatusType = new ResponseStatusType();
            responseStatusType.Id = "1";
            responseStatusType.Success = true;
            responseStatusType.Message = "";
            sellRS.ResponseStatusType = responseStatusType;

            sellRS.Signature = sellRQ.Signature;
            sellRS.SellKey = "S" + CreateToken.GetGUID();

            string ErrorMessage = "";

            decimal basicfareCHD = 0, totaltaxCHD = 0, totalpriceCHD = 0;
            decimal basicfareINF = 0, totaltaxINF = 0, totalpriceINF = 0;

            decimal totalBasicfare = 0, TotalTax = 0, totalFare = 0;
            decimal basicfareADT = 0, totaltaxADT = 0, totalpriceADT = 0;
            decimal TOTAL_TAX_ADT = 0, TOTAL_TAX_INF = 0, TOTAL_TAX_CHD = 0, Extra_ADT = 0, Extra_CHD = 0, Extra_INF = 0;

            try
            {
                VerifyResponse.Root resp = JsonConvert.DeserializeObject<VerifyResponse.Root>(responseData);

                #region Amount Checking
                decimal TotalFareOnGetAvailability = 0;
                if (sellRQ.Flights != null)
                {
                    if (sellRQ.Flights[0].TotalPrice_ADT != 0)
                        TotalFareOnGetAvailability += (sellRQ.Flights[0].TotalPrice_ADT);
                    else
                        TotalFareOnGetAvailability += sellRQ.Flights[0].TotalPrice_ADT;
                }
                if (sellRQ.Flights.Count > 1 && sellRQ.Flights[1] != null)
                {
                    if (sellRQ.Flights[1].TotalPrice_ADT != 0)
                        TotalFareOnGetAvailability += (sellRQ.Flights[1].TotalPrice_ADT);
                    else
                        TotalFareOnGetAvailability += sellRQ.Flights[1].TotalPrice_ADT;
                }
                #endregion
                #region Response
                if (responseData != null)
                {
                   

                    if (resp.success && resp.fare != null)
                    {
                        #region sellbinding
                        Flight flight = new Flight();
                        

                        flight.FlightPricingInfo = new FlightPricingInfo();
                        flight.FlightPricingInfo.PaxFareDetails = new List<FareDetail>();
                        List<TaxDetail> taxDetailslst = new List<TaxDetail>();

                        if (resp.success && resp.fare.passengerTypeFareList.Count() > 0)
                        {
                            foreach (var item in resp.fare.passengerTypeFareList)
                            {
                                FareDetail fareDetail = new FareDetail();
                                TaxDetail taxDetails = new TaxDetail();
                                fareDetail.PaxTaxDetails = new List<TaxDetail>();

                                if (item.passengerTypeCode == "ADT")
                                {
                                    foreach (var pl in item.priceList)
                                    {
                                        if (pl.type == "TOTAL_TAX")
                                        {
                                            taxDetails = new TaxDetail();
                                            totaltaxADT = Convert.ToDecimal(pl.value);
                                            taxDetails.TaxCode = pl.type;
                                            taxDetails.TaxAmount = Convert.ToDecimal(pl.value);
                                            TOTAL_TAX_ADT = taxDetails.TaxAmount;
                                            fareDetail.PaxTaxDetails.Add(taxDetails);
                                        }
                                        else if (pl.type == "AGENCY_PURCHASE_PRICE" || pl.type == "AGENCY_MARGIN")
                                        {
                                            basicfareADT += Convert.ToDecimal(pl.value);
                                        }
                                        else
                                        {
                                            taxDetails = new TaxDetail();
                                            taxDetails.TaxCode = "Extra";
                                            taxDetails.TaxAmount = Convert.ToDecimal(pl.value);
                                            Extra_ADT = taxDetails.TaxAmount;
                                            if (taxDetails.TaxAmount > 0)
                                            {
                                                fareDetail.PaxTaxDetails.Add(taxDetails);
                                            }
                                        }

                                    }

                                    totalBasicfare += (basicfareADT * sellRQ.PaxType.Adult);

                                    TotalTax = (totaltaxADT * sellRQ.PaxType.Adult);
                                    totalpriceADT = (totaltaxADT + basicfareADT);
                                    totalFare = (totalpriceADT * sellRQ.PaxType.Adult);

                                    fareDetail.PaxType = item.passengerTypeCode;
                                    fareDetail.TotalFare = totalpriceADT;
                                    fareDetail.BasicFare = basicfareADT;
                                    fareDetail.TotalTax = totaltaxADT;


                                    flight.FlightPricingInfo.PaxFareDetails.Add(fareDetail);

                                }

                                if (item.passengerTypeCode == "CHD")
                                {
                                    foreach (var pl in item.priceList)
                                    {
                                        if (pl.type == "TOTAL_TAX")
                                        {
                                            taxDetails = new TaxDetail();
                                            totaltaxCHD = Convert.ToDecimal(pl.value);
                                            taxDetails.TaxCode = pl.type;
                                            taxDetails.TaxAmount = Convert.ToDecimal(pl.value);
                                            TOTAL_TAX_CHD = taxDetails.TaxAmount;
                                            fareDetail.PaxTaxDetails.Add(taxDetails);
                                        }
                                        else if (pl.type == "AGENCY_PURCHASE_PRICE" || pl.type == "AGENCY_MARGIN")
                                        {
                                            basicfareCHD += Convert.ToDecimal(pl.value);
                                        }
                                        else
                                        {
                                            taxDetails = new TaxDetail();
                                            taxDetails.TaxCode = "Extra";
                                            taxDetails.TaxAmount = Convert.ToDecimal(pl.value);
                                            Extra_CHD = taxDetails.TaxAmount;
                                            if (taxDetails.TaxAmount > 0)
                                            {
                                                fareDetail.PaxTaxDetails.Add(taxDetails);
                                            }
                                        }

                                    }

                                    TotalTax += (totaltaxCHD * sellRQ.PaxType.Child);
                                    totalpriceCHD = (basicfareCHD + totaltaxCHD);
                                    totalFare += (totalpriceCHD * sellRQ.PaxType.Child);
                                    totalBasicfare += (basicfareCHD * sellRQ.PaxType.Child);
                                    //flightsingle.BasicFare_CHD = (basicfareCHD);
                                    //flightsingle.TotalTax_CHD = (totaltaxCHD);
                                    //flightsingle.TotalPrice_CHD = (totalpriceCHD);
                                    //flightsingle.ExtraTax_CHD = totaltaxCHD;
                                    fareDetail.PaxType = item.passengerTypeCode;
                                    fareDetail.TotalFare = totalpriceCHD;
                                    fareDetail.BasicFare = basicfareCHD;
                                    fareDetail.TotalTax = totaltaxCHD;

                                    flight.FlightPricingInfo.PaxFareDetails.Add(fareDetail);

                                }

                                if (item.passengerTypeCode == "INF")
                                {
                                    foreach (var pl in item.priceList)
                                    {
                                        if (pl.type == "TOTAL_TAX")
                                        {
                                            taxDetails = new TaxDetail();
                                            totaltaxINF = Convert.ToDecimal(pl.value);
                                            taxDetails.TaxCode = pl.type;
                                            taxDetails.TaxAmount = Convert.ToDecimal(pl.value);
                                            TOTAL_TAX_INF = taxDetails.TaxAmount;
                                            fareDetail.PaxTaxDetails.Add(taxDetails);
                                        }
                                        else if (pl.type == "AGENCY_PURCHASE_PRICE" || pl.type == "AGENCY_MARGIN")
                                        {
                                            basicfareINF += Convert.ToDecimal(pl.value);
                                        }
                                        else
                                        {
                                            taxDetails = new TaxDetail();
                                            taxDetails.TaxCode = "Extra";
                                            taxDetails.TaxAmount = Convert.ToDecimal(pl.value);
                                            Extra_INF = taxDetails.TaxAmount;
                                            if (taxDetails.TaxAmount > 0)
                                            {
                                                fareDetail.PaxTaxDetails.Add(taxDetails);
                                            }
                                        }
                                    }
                                    TotalTax += (totaltaxINF * sellRQ.PaxType.Infant);
                                    totalpriceINF += (basicfareINF + totaltaxINF);
                                    totalBasicfare += (basicfareINF * sellRQ.PaxType.Infant);
                                    totalFare += (totalpriceINF * sellRQ.PaxType.Infant);

                                    //flightsingle.BasicFare_INF = (basicfareINF);
                                    //flightsingle.TotalTax_INF = (totaltaxINF);
                                    //flightsingle.TotalPrice_INF = (totalpriceINF);
                                    //flightsingle.ExtraTax_INF = totaltaxINF;
                                    fareDetail.PaxType = item.passengerTypeCode;
                                    fareDetail.TotalFare = totalpriceINF;
                                    fareDetail.BasicFare = basicfareINF;
                                    fareDetail.TotalTax = totaltaxINF;

                                    flight.FlightPricingInfo.PaxFareDetails.Add(fareDetail);
                                }

                                #region FlightTaxDetails
                                flight.FlightPricingInfo.FlightTaxDetails = new List<TaxDetail>();
                                decimal TOTAL_TAX = TOTAL_TAX_ADT * sellRQ.PaxType.Adult + TOTAL_TAX_CHD * sellRQ.PaxType.Child + TOTAL_TAX_INF * sellRQ.PaxType.Infant;

                                decimal Extra = Extra_ADT * sellRQ.PaxType.Adult + Extra_CHD * sellRQ.PaxType.Child + Extra_INF * sellRQ.PaxType.Infant;


                                foreach (var pl in item.priceList)
                                {
                                    taxDetails = new TaxDetail();
                                    if (pl.type == "TOTAL_TAX" && TotalTax > 0)
                                    {
                                        taxDetails.TaxCode = pl.type;
                                        taxDetails.TaxAmount = TOTAL_TAX;
                                        flight.FlightPricingInfo.FlightTaxDetails.Add(taxDetails);
                                    }
                                    else if (pl.type == "AGENCY_PURCHASE_PRICE" || pl.type == "AGENCY_MARGIN")
                                    {
                                        taxDetails = new TaxDetail();

                                    }
                                    else if (Extra > 0)
                                    {
                                        taxDetails.TaxCode = "Extra";
                                        taxDetails.TaxAmount = Extra;
                                        flight.FlightPricingInfo.FlightTaxDetails.Add(taxDetails);
                                    }
                                }
                                #endregion
                            }
                        }
                        

                        if (resp.success && resp.fare.legList.Count() > 0)
                        {
                            List<Flight> lstFlight = new List<Flight>();
                            foreach (var leg in resp.fare.legList)
                            {
                                foreach (var f in leg.itineraryList)
                                {
                                    Flight flight1 = new Flight();
                                    List<Segment> lstSegment = new List<Segment>();
                                    //foreach (var seg in f.segmentList)
                                    for (int i = 0; i < f.segmentList.Count(); i++)
                                    {
                                        Segment segment = new Segment();
                                        flight1.FlightKey = f.id;
                                        flight1.FlightIndex = leg.index;
                                        flight1.OriginDestination = new OriginDestination();
                                        flight1.OriginDestination.Departure = f.segmentList[0].departure.iata;
                                        flight1.OriginDestination.Arrival = f.segmentList[f.segmentList.Count-1].destination.iata;

                                        string dateString = f.segmentList[i].departureDate.day.ToString("d2") + "/" + f.segmentList[i].departureDate.month.ToString("d2") + "/" + f.segmentList[i].departureDate.year + " " + f.segmentList[i].departureTimeOfDay.hour.ToString("d2") + ":" + f.segmentList[i].departureTimeOfDay.minute.ToString("d2");
                                        DateTime dtDepartureDateTime = DateTime.ParseExact(dateString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                                        string dateStringArr = f.segmentList[i].arrivalDate.day.ToString("d2") + "/" + f.segmentList[i].arrivalDate.month.ToString("d2") + "/" + f.segmentList[i].arrivalDate.year + " " + f.segmentList[i].arrivalTimeOfDay.hour.ToString("d2") + ":" + f.segmentList[i].arrivalTimeOfDay.minute.ToString("d2");
                                        DateTime dtArrivalDateTime = DateTime.ParseExact(dateStringArr, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                                        //flight1.OriginDestination.ArrivalDateTime = dtArrivalDateTime;
                                        //flight1.OriginDestination.DepartureDateTime = dtDepartureDateTime;
                                        TimeSpan spflightMin = TimeSpan.FromMinutes(f.flyingTimeInMinutes);
                                        string totaltime = string.Format("{0}:{1}", (int)spflightMin.TotalHours, spflightMin.Minutes);
                                       

                                        segment.OriginDestination = new OriginDestination();
                                        segment.OriginDestination.Departure = f.segmentList[i].departure.iata;
                                        segment.OriginDestination.Arrival = f.segmentList[i].destination.iata;
                                        segment.OriginDestination.DepartureDateTime = dtDepartureDateTime;
                                        segment.OriginDestination.ArrivalDateTime = dtArrivalDateTime;
                                        TimeSpan tsStopover = dtArrivalDateTime - dtDepartureDateTime;
                                        segment.OriginDestination.TotalTime = totaltime;

                                        segment.MarketingCarrier = f.segmentList[i].marketingAirline.iata;
                                        segment.OperationgCarrier = f.segmentList[i].operatingAirline.iata;
                                        segment.Cabin = f.segmentList[i].cabinClass;
                                        segment.CabinBaggage = "7 KG";
                                        segment.EquipmentType = f.segmentList[i].equipmentType.code;
                                        segment.Baggage = new Baggage();
                                        segment.Baggage.Weight = f.segmentList[i].baggageAllowance.quantity.ToString();
                                        segment.Baggage.Unit = f.segmentList[i].baggageAllowance.unit;
                                        segment.FareSellKey = resp.fare.fareId;

                                        lstSegment.Add(segment);

                                        flight1.OriginDestination.TotalTime += totaltime;
                                        flight1.Segments = lstSegment;
                                    }
                                    flight1.ValidatingCarrier = resp.fare.validatingAirline.iata;
                                    flight1.FlightPricingInfo = flight.FlightPricingInfo;
                                    
                                    lstFlight.Add(flight1);
                                    
                                }

                                sellRS.Flights = lstFlight;
                            }
                        }
                        #endregion
                        
                    }
                    
                }

                else
                {
                    sellRS.ResponseStatusType.Success = true;
                }
                #endregion

                if (totalFare != TotalFareOnGetAvailability)
                {
                    sellRS.ResponseStatusType.Message = "seat sold out";
                    sellRS.ResponseStatusType.Success = false;
                }
                
                else
                {
                   sellRS.ResponseStatusType.Success = true;
                }

            }
            catch(Exception ex)
            {
                sellRS.ResponseStatusType = Log.BindErrorMessage("7", false, "System error occured,Please cordinate to the support team with trace id : " + sellRQ.Signature.TrackID);
                Log.AddErrorMessage(sellRQ, ex, "Getting Exception in SellVerifyResponse Method", "Exception", "", sellRQ.Signature.OfficeID, Vendor.AERTicket.ToString(), "SellVerifyResponse", GetActions.Environment);

            }
            return sellRS;
        }


        public BookingRS BookingResponse(string responseData, BookingRQ bookingRQ)
        {
            BookingRS bookingRS = new BookingRS();
            ResponseStatusType responseStatusType = new ResponseStatusType();
            responseStatusType.Id = "1";
            responseStatusType.Success = true;
            responseStatusType.Message = "";
            bookingRS.ResponseStatusType = responseStatusType;

            BookingResponse.Root resp = JsonConvert.DeserializeObject<BookingResponse.Root>(responseData);
            string PNR = "";
            try
            {
                if (resp != null)
                {
                    if (resp.success && resp.pnr != null)
                    {
                        
                        //objBookDetails.AERTicketFlowID = response.Headers.ToList().Find(x => x.Name == "apihubFlowId").Value.ToString();
                        PNR = resp.pnr.locator;

                        var timezoneArr = resp.pnr?.pnrRuleSet?.timezone.Split('/');
                        var timeSpanGiven = new TimeSpan(0, 0, 0);
                        double timeDifference = 0;

                        if (timezoneArr.Count() > 0 && timezoneArr != null)
                        {
                            var TimeZoneDetails = TimeZoneInfo.GetSystemTimeZones().Where(x => x.DisplayName.ToLower().Contains(timezoneArr[1].ToLower().ToString())).ToList();

                            if (TimeZoneDetails.Count > 0 && TimeZoneDetails != null)
                            {
                                timeSpanGiven = TimeZoneDetails.FirstOrDefault().BaseUtcOffset;

                                var IndiaTime = TimeZoneInfo.GetSystemTimeZones().Where(x => x.DisplayName.ToLower().Contains("mumbai")).ToList();
                                var IndiaTimeSpan = IndiaTime.FirstOrDefault().BaseUtcOffset;

                                timeDifference = IndiaTimeSpan.TotalMinutes - timeSpanGiven.TotalMinutes;
                            }
                        }

                        //bookingRS.IssueDate = (DateTime)(resp.pnr?.pnrRuleSet?.fareTicketTimeLimit.AddMinutes(timeDifference));
                        bookingRS.HoldDate = (DateTime)(resp.pnr?.pnrRuleSet?.ticketTimeLimit.AddMinutes(timeDifference));
                        bookingRS.IssueDate = DateTime.Now;
                        bookingRS.Flights = new List<BookingFlight>();
                        for (int i = 0; i < bookingRQ.Flights.Count; i++)
                        {
                            BookingFlight bookingFlight = new BookingFlight();
                            bookingFlight.OriginDestination = bookingRQ.Flights[i].OriginDestination;
                            bookingFlight.SupplierName = bookingRQ.Flights[i].SupplierName;
                            bookingFlight.BasicFare = bookingRQ.Flights[i].BasicFare;
                            bookingFlight.TotalTax = bookingRQ.Flights[i].TotalTax;
                            bookingFlight.TotalFare = bookingRQ.Flights[i].TotalFare;
                            bookingFlight.TourCode = bookingRQ.Flights[i].TourCode;
                            bookingFlight.Endorsementline = bookingRQ.Flights[i].Endorsementline;
                            bookingFlight.FlightKey =bookingRQ.Flights[i].FlightKey;
                            bookingFlight.FareName = bookingRQ.Flights[i].FareName;
                            bookingFlight.ValidatingCarrier = bookingRQ.Flights[i].ValidatingCarrier;
                            bookingFlight.Segments = bookingRQ.Flights[i].Segments;
                            bookingRS.Flights.Add(bookingFlight);
                        }
                        bookingRS.AirlineType = AirlineType.LCC.ToString();
                        bookingRS.Passengers = bookingRQ.Passengers;
                        bookingRS.ValidatingCarrier = bookingRQ.Flights[0].ValidatingCarrier;
                        string FareNames = string.Empty, IataCode = string.Empty, ErrorMessage = string.Empty;
                        decimal TotalMarkUp = 0, TotalMCO = 0;

                        List<string> CorporateID = new List<string>();
                        decimal commission = 0;

                        PNRretrieve PNRRetrieve = new PNRretrieve();
                        List<Passenger> Passengers = bookingRQ.Passengers/*.FindAll(q => q.isRtnPassenger == false)*/;
                        int passengercount = Passengers.Count;

                        bookingRS.GDSPNR = PNR;
                        foreach (var a in bookingRS.Flights)
                        {
                            a.AirlinePNR = PNR;
                            foreach (var aseg in a.Segments)
                            {
                                aseg.AirlinePNR = PNR;
                            }
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                bookingRS.ResponseStatusType = Log.BindErrorMessage("10", false, "System error occured,Please cordinate to the support team with trace id : " + bookingRQ.Signature.TrackID);
                Log.AddErrorMessage(bookingRQ, ex, "Getting Exception in BookingResponse Method", "Exception", "", bookingRQ.Signature.OfficeID, Vendor.AERTicket.ToString(), "BookingResponse", GetActions.Environment, bookingRQ.Signature.TrackID);

            }
            return bookingRS;
        }
    }



}


