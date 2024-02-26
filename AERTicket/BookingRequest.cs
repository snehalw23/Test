namespace AERTicketWebService.AERTicket
{
    public class BookingRequest
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class RuleSetBookingList
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public class BillingInformation
        {
            public string email { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public string street { get; set; }
            public string zipCode { get; set; }
            public string lastName { get; set; }
            public string firstName { get; set; }
            public string phoneNumber { get; set; }
            public List<RuleSetBookingList> ruleSetBookingList { get; set; }
        }

        public class DateOfBirth
        {
            public int year { get; set; }
            public int month { get; set; }
            public int day { get; set; }
        }

        public class Airline
        {
            public string iata { get; set; }
            public string icao { get; set; }
        }

        public class FrequentFlyerNumberList
        {
            public Airline airline { get; set; }
            public string number { get; set; }
        }

        public class IssuingCountry
        {
            public string iso { get; set; }
        }

        public class Nationality
        {
            public string iso { get; set; }
        }

        public class Expiration
        {
            public int year { get; set; }
            public int month { get; set; }
            public int day { get; set; }
        }

        public class TravelDocument
        {
            public IssuingCountry issuingCountry { get; set; }
            public Nationality nationality { get; set; }
            public string number { get; set; }
            public Expiration expiration { get; set; }
            public string type { get; set; }
        }

        public class Currency
        {
            public string iso { get; set; }
        }

        public class PriceList
        {
            public int value { get; set; }
            public string type { get; set; }
            public Currency currency { get; set; }
            public string subType { get; set; }
        }

        public class OperationalContactData
        {
            public bool emailAddressRefused { get; set; }
            public bool phoneNumberRefused { get; set; }
            public string emailAddress { get; set; }
            public string phoneNumber { get; set; }
        }

        public class PassengerList
        {
            public int id { get; set; }
            public string lastName { get; set; }
            public string firstName { get; set; }
            public string passengerTypeCode { get; set; }
            public DateOfBirth dateOfBirth { get; set; }
            public string gender { get; set; }
            public string title { get; set; }
            public FrequentFlyerNumberList frequentFlyerNumberList { get; set; }
            public TravelDocument travelDocument { get; set; }
            public List<PriceList> priceList { get; set; }
            public OperationalContactData operationalContactData { get; set; }
            public List<RuleSetBookingList> ruleSetBookingList { get; set; }
        }

        public class BookingOptions
        {
            public bool instantTicketOrder { get; set; }
            public bool pullRules { get; set; }
        }

        public class Root
        {
            public string fareId { get; set; }
            public BillingInformation billingInformation { get; set; }
            public List<PassengerList> passengerList { get; set; }
            public BookingOptions bookingOptions { get; set; }
        }


    }
}
