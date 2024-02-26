using InternalClasses.Response;
using InternalClasses.RQ.InternalClasses;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using static AERTicketWebService.Common.Common;

namespace AERTicketWebService.Services
{
    public class Log
    {
        public static string? FromDestination { get; set; }
        public static string? ToDestination { get; set; }
        public static DateTime? DepartureDateTime { get; set; }
        //public static string? MethodName { get; set; }
        //public static string? OfficeID { get; set; }
        //public static string? APIName { get; set; }
        //public static string? TrackID { get; set; }
        public class ElasticLogEntity
        {
            public string? Id { get; set; }
            public string? XMLRequest { get; set; }
            public string? XMLResponse { get; set; }
            public string? FromDestination { get; set; }
            public string? ToDestination { get; set; }
            public DateTime? DepartureDateTime { get; set; }
            public string? TrackID { get; set; }
            public string? APIName { get; set; }
            public DateTime? InsertedDateTime { get; set; }
            public string? RiyaPNR { get; set; }
            public string? LogType { get; set; }
            public string? MethodName { get; set; }
            public string? OfficeID { get; set; }
            public string? ErrorMessage { get; set; }
            public bool? isFromCaching { get; set; }
            public string? environment { get; set; }
        }
        public static void AddLogElastic(object? RQ, object? RS, string? LogType, string? ErrorMessage, string OfficeID, string APIName, string MethodName, bool isFromCache = false, string environment = "", string TrackID = "")
        {
            try
            {
                ElasticLogEntity model = new ElasticLogEntity();
                model.TrackID = TrackID;

                if (LogType == "OUT" || LogType == "Trvlnxt")
                {
                    model.XMLRequest = JsonConvert.SerializeObject(RQ);
                    model.XMLResponse = JsonConvert.SerializeObject(RS);
                }
                else
                {
                    model.XMLRequest = SerializeObject(RQ);
                    model.XMLResponse = SerializeObject(RS);
                }
                model.FromDestination = FromDestination;
                model.ToDestination = ToDestination;
                model.DepartureDateTime = DepartureDateTime;
                model.APIName = APIName;

                model.ErrorMessage = ErrorMessage;
                model.OfficeID = OfficeID;
                model.LogType = LogType;
                model.MethodName = MethodName;
                model.InsertedDateTime = DateTime.Now;
                model.RiyaPNR = "";
                model.Id = "";
                model.isFromCaching = isFromCache;
                model.environment = environment;

                var client = new RestClient(GetActions.ElasticLogURL);
                var request = new RestRequest("/KibanaLog/AddLogKibana", Method.Post);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", Newtonsoft.Json.JsonConvert.SerializeObject(model), ParameterType.RequestBody);
                var response = client.Execute(request);
            }
            catch (Exception ex)
            {
                AddErrorMessage(RQ, ex, "Getting Error in AddLogElastic LogType : " + LogType + " , ErrorMessage : " + ErrorMessage + ")", "Exception", "", OfficeID, APIName, "InternalClasses_AddLogElastic_" + MethodName, GetActions.Environment, TrackID);
            }
        }

        public static string SerializeObject(object obj)
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                serializer.Serialize(ms, obj);
                ms.Position = 0;
                xmlDoc.Load(ms);
                return xmlDoc.InnerXml;
            }
        }
        public static string JsonSerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static AvailabilityRS DeSerializeObject(string xmlString)
        {
            var reader = new StringReader(xmlString);
            var serializer = new XmlSerializer(typeof(AvailabilityRS));
            var instance = (AvailabilityRS)serializer.Deserialize(reader);

            return instance;
        }
        public static string XmlToJson(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            string jsonText = JsonConvert.SerializeXmlNode(doc);
            return jsonText;
        }
        public static void AddErrorMessage(object? Request, Exception? ex, string? ErrorMessage, string? LogType, string? GDSPNR, string OfficeID, string APIName, string MethodName, string environment = "", string TrackID = "")
        {
            try
            {
                StackFrame frame = new StackFrame(1);
                var method = frame.GetMethod();

                //dynamic Obj = new System.Dynamic.ExpandoObject();
                //Obj.PageName = method.DeclaringType.FullName;
                //Obj.MethodName = method.Name;
                //Obj.GDSPNR = GDSPNR;                

                StringBuilder sb = new StringBuilder();
                sb.Append("Hi " + GDSPNR);
                sb.Append("<br/><table style='border-collapse:collapse;background-color:#F5F5F5' width='auto' border='1'>");
                sb.Append("<tr><td>Error: <br/>");
                sb.Append(ex.Message == "Exception of type 'System.Exception' was thrown." ? "" : ex.Message + "</td></tr>");
                sb.Append("<tr><td>InnerException: <br/>");
                sb.Append(ex.InnerException + "</td></tr>");
                sb.Append("<tr><td>StackTrace: <br/>");
                sb.Append(ex.StackTrace + "</td></tr>");
                sb.Append("<tr><td>PageName: <br/> </td> " + method.DeclaringType.FullName + "</tr>");
                sb.Append("<tr><td>MethodName: <br/> </td> " + method.Name + "</tr>");
                sb.Append("<tr><td>ErrorMessage : <br/> </td> " + ErrorMessage + "</tr>");
                sb.Append("<tr><td>Date Time <br/>");
                sb.Append(DateTime.Now.ToString("dd MMM yy hh:mm tt") + "</td></tr>");
                sb.Append("</table>");
                sb.Append("<br/><br/><br/>");
                sb.Append("Regards,<br/>API Service");


                ElasticLogEntity model = new ElasticLogEntity();
                model.TrackID = TrackID;
                model.XMLRequest = JsonConvert.SerializeObject(Request);
                model.XMLResponse = JsonConvert.SerializeObject("");
                model.FromDestination = FromDestination;
                model.ToDestination = ToDestination;
                model.DepartureDateTime = DepartureDateTime;
                model.APIName = APIName;
                model.OfficeID = OfficeID;
                model.LogType = LogType;
                model.MethodName = MethodName;
                model.ErrorMessage = sb.ToString();
                model.InsertedDateTime = DateTime.Now;
                model.RiyaPNR = GDSPNR;
                model.Id = "";
                model.environment = environment;

                var client = new RestClient(GetActions.ElasticLogURL);
                var request = new RestRequest("/KibanaLog/AddLogKibana", Method.Post);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", Newtonsoft.Json.JsonConvert.SerializeObject(model), ParameterType.RequestBody);
                var response = client.Execute(request);

            }
            catch (Exception ex1)
            {

            }
        }
        public static bool AirSellAndBookingTicketTimeCount(string TrackID, string? MethodName, string? LogType, string? OfficeID, string? APIName)
        {
            bool result = true;
            try
            {
                var client = new RestClient(GetActions.ElasticLogURL);
                var request = new RestRequest("/KibanaLog/AirSellAndBookingTicketTimeCount", Method.Get);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("TrackID", TrackID);
                request.AddParameter("MethodName", MethodName);
                request.AddParameter("LogType", LogType);
                var response = client.Execute(request);

                if (response.Content == "0")
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                AddErrorMessage("", ex, "Getting Error in AirSellAndBookingTicketTimeCount (MethodName : " + MethodName + " , LogType : " + LogType + " , OfficeID : " + OfficeID + " , APIName : " + APIName + ")", "Exception", "", OfficeID, APIName, "InternalClasses_AirSellAndBookingTicketTimeCount_" + MethodName, GetActions.Environment, TrackID);
            }
            return result;
        }

        public static ResponseStatusType BindErrorMessage(string Id, bool IsSuccess, string Message)
        {
            ResponseStatusType responseStatusType = new ResponseStatusType();
            responseStatusType.Id = Id;
            responseStatusType.Success = IsSuccess;
            responseStatusType.Message = Message;
            return responseStatusType;
        }
        public static string Base64Encode(string plainText)
        {
            plainText = "MR" + plainText;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
    public class CreateToken
    {
        public static string GetGUID()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffffff") + "_" + Guid.NewGuid().ToString();
        }
    }
}
