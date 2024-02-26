namespace AERTicketWebService.AERTicket
{
    public class AERTicket_Common
    {
        public class SearchRequest
        {
            public class Departure
            {
                public string iata { get; set; }
                public string geoObjectType { get; set; }
            }

            public class Destination
            {
                public string iata { get; set; }
                public string geoObjectType { get; set; }
            }

            public class DepartureDate
            {
                public int year { get; set; }
                public int month { get; set; }
                public int day { get; set; }
            }

            public class SegmentList
            {
                public Departure departure { get; set; }
                public Destination destination { get; set; }
                public DepartureDate departureDate { get; set; }
                public object departureTimeRange { get; set; }
                public object arrivalTimeRange { get; set; }
                public object viaExcluded { get; set; }
                public object viaIncluded { get; set; }
            }

            public class RequestPassengerTypeList
            {
                public string passengerTypeCode { get; set; }
                public int count { get; set; }
            }

            public class FareSourceList
            {
                public string id { get; set; }
            }

            public class ClosedUserGroupTypeList
            {
                public string code { get; set; }
            }

            public class ConnectingTimeFilter
            {
                public bool allowAirportChange { get; set; }
            }

            public class SearchOptions
            {
                public List<string> cabinClassList { get; set; }
                public bool nonStopFlightsOnly { get; set; }
                public bool directFlightsOnly { get; set; }
                public object includedAirlineList { get; set; }
                public object excludedAirlineList { get; set; }
                public List<FareSourceList> fareSourceList { get; set; }
                public List<ClosedUserGroupTypeList> closedUserGroupTypeList { get; set; }
                public ConnectingTimeFilter connectingTimeFilter { get; set; }
            }

            public class Root
            {
                public List<SegmentList> segmentList { get; set; }
                public List<RequestPassengerTypeList> requestPassengerTypeList { get; set; }
                public SearchOptions searchOptions { get; set; }
            }
        }


        public class SearchResponse
        {
            public class Currency
            {
                public string iso { get; set; }
            }

            public class PriceList
            {
                public double value { get; set; }
                public Currency currency { get; set; }
                public string type { get; set; }
            }

            public class PassengerTypeFareList
            {
                public string passengerTypeCode { get; set; }
                public int count { get; set; }
                public List<PriceList> priceList { get; set; }
            }

            public class Departure
            {
                public string iata { get; set; }
                public string geoObjectType { get; set; }
                public string name { get; set; }
            }

            public class Destination
            {
                public string iata { get; set; }
                public string geoObjectType { get; set; }
                public string name { get; set; }
            }

            public class DepartureDate
            {
                public int year { get; set; }
                public int month { get; set; }
                public int day { get; set; }
            }

            public class DepartureTimeOfDay
            {
                public int hour { get; set; }
                public int minute { get; set; }
            }

            public class ArrivalDate
            {
                public int year { get; set; }
                public int month { get; set; }
                public int day { get; set; }
            }

            public class ArrivalTimeOfDay
            {
                public int hour { get; set; }
                public int minute { get; set; }
            }

            public class MarketingAirline
            {
                public string icao { get; set; }
                public string iata { get; set; }
                public string name { get; set; }
            }

            public class OperatingAirline
            {
                public string icao { get; set; }
                public string iata { get; set; }
                public string name { get; set; }
            }

            public class EquipmentType
            {
                public string code { get; set; }
                public string name { get; set; }
            }

            public class BaggageAllowance
            {
                public string unit { get; set; }
                public int quantity { get; set; }
            }

            public class SegmentList
            {
                public Departure departure { get; set; }
                public Destination destination { get; set; }
                public DepartureDate departureDate { get; set; }
                public DepartureTimeOfDay departureTimeOfDay { get; set; }
                public ArrivalDate arrivalDate { get; set; }
                public ArrivalTimeOfDay arrivalTimeOfDay { get; set; }
                public MarketingAirline marketingAirline { get; set; }
                public OperatingAirline operatingAirline { get; set; }
                public int flightNumber { get; set; }
                public string bookingClassCode { get; set; }
                public int numberOfTechnicalStops { get; set; }
                public EquipmentType equipmentType { get; set; }
                public string fareBase { get; set; }
                public string airlineFareFamily { get; set; }
                public BaggageAllowance baggageAllowance { get; set; }
                public string cabinClass { get; set; }
            }

            public class ItineraryList
            {
                public string id { get; set; }
                public int flyingTimeInMinutes { get; set; }
                public List<SegmentList> segmentList { get; set; }
            }

            public class LegList
            {
                public int index { get; set; }
                public List<ItineraryList> itineraryList { get; set; }
            }

            public class ValidatingAirline
            {
                public string icao { get; set; }
                public string iata { get; set; }
                public string name { get; set; }
            }

            public class TicketTimeLimitDate
            {
                public int year { get; set; }
                public int month { get; set; }
                public int day { get; set; }
            }

            public class TicketTimeLimitTimeOfDay
            {
                public int hour { get; set; }
                public int minute { get; set; }
            }

            public class FareSourceList
            {
                public string id { get; set; }
            }

            public class RuleSet
            {
                public bool tsaSuitableFlight { get; set; }
                public string penaltyInfo { get; set; }
                public DateTime ticketTimeLimit { get; set; }
                public TicketTimeLimitDate ticketTimeLimitDate { get; set; }
                public TicketTimeLimitTimeOfDay ticketTimeLimitTimeOfDay { get; set; }
                public bool travelDocumentIsRequired { get; set; }
                public List<FareSourceList> fareSourceList { get; set; }
            }

            public class PnrAccessRights
            {
                public bool voidSupported { get; set; }
                public bool getCouponSupported { get; set; }
                public bool refundSupported { get; set; }
            }

            public class AvailableFareList
            {
                public List<PassengerTypeFareList> passengerTypeFareList { get; set; }
                public List<LegList> legList { get; set; }
                public ValidatingAirline validatingAirline { get; set; }
                public string fareId { get; set; }
                public RuleSet ruleSet { get; set; }
                public List<object> ruleSetBookingList { get; set; }
                public PnrAccessRights pnrAccessRights { get; set; }
                public bool instantPurchaseRequired { get; set; }
            }

            public class Root
            {
                public bool success { get; set; }
                public List<object> providerErrorList { get; set; }
                public List<AvailableFareList> availableFareList { get; set; }
            }
        }
    }
}

