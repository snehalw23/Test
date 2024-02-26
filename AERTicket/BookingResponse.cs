namespace AERTicketWebService.AERTicket
{
    public class BookingResponse
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
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

        public class InfantBaggageAllowance
        {
            public string unit { get; set; }
            public int quantity { get; set; }
        }

        public class Airport
        {
            public string iata { get; set; }
            public string geoObjectType { get; set; }
            public string name { get; set; }
        }

        public class DepartureTerminal
        {
            public Airport airport { get; set; }
            public string name { get; set; }
        }

        public class DestinationTerminal
        {
            public Airport airport { get; set; }
            public string name { get; set; }
        }

        public class SegmentList
        {
            public string externalKey { get; set; }
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
            public InfantBaggageAllowance infantBaggageAllowance { get; set; }
            public string cabinClass { get; set; }
            public string segmentStatus { get; set; }
            public DepartureTerminal departureTerminal { get; set; }
            public DestinationTerminal destinationTerminal { get; set; }
            public int flyingTimeInMinutes { get; set; }
        }

        public class DateOfBirth
        {
            public int year { get; set; }
            public int month { get; set; }
            public int day { get; set; }
        }

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

        public class PassengerList
        {
            public int id { get; set; }
            public string externalKey { get; set; }
            public string title { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string passengerTypeCode { get; set; }
            public DateOfBirth dateOfBirth { get; set; }
            public List<PriceList> priceList { get; set; }
            public List<object> ruleSetBookingList { get; set; }
        }

        public class PassengerTypeFareList
        {
            public string passengerTypeCode { get; set; }
            public int count { get; set; }
            public List<PriceList> priceList { get; set; }
        }

        public class PnrAccessRights
        {
            public bool voidSupported { get; set; }
            public bool getCouponSupported { get; set; }
            public bool refundSupported { get; set; }
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

        public class PnrRuleSet
        {
            public DateTime ticketTimeLimit { get; set; }
            public TicketTimeLimitDate ticketTimeLimitDate { get; set; }
            public TicketTimeLimitTimeOfDay ticketTimeLimitTimeOfDay { get; set; }
            public DateTime fareTicketTimeLimit { get; set; }
            public string timezone { get; set; }
        }

        public class FareRuleList
        {
            public string plainText { get; set; }
            public string htmlText { get; set; }
        }

        public class Pnr
        {
            public string locator { get; set; }
            public List<SegmentList> segmentList { get; set; }
            public List<PassengerList> passengerList { get; set; }
            public List<PassengerTypeFareList> passengerTypeFareList { get; set; }
            public List<object> ticketList { get; set; }
            public List<object> ancillaryList { get; set; }
            public List<object> seatList { get; set; }
            public PnrAccessRights pnrAccessRights { get; set; }
            public ValidatingAirline validatingAirline { get; set; }
            public PnrRuleSet pnrRuleSet { get; set; }
            public List<FareRuleList> fareRuleList { get; set; }
            public List<object> osiRemarkList { get; set; }
        }

        public class Root
        {
            public bool success { get; set; }
            public List<object> providerErrorList { get; set; }
            public Pnr pnr { get; set; }
        }


    }
}
