using AERTicketWebService.Services;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using static AERTicketWebService.Common.Common;

namespace AERTicketWebService.AERTicket
{
    public class WebApi
    {

        public static string ExecutePostAPI(string urlStr, string jsondata,string username,string password)
        {
         
            try
            {
                try
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlStr);
                    request.ContentType = "application/json";
                    request.Method = "POST";
                    request.Headers.Add("login", username);
                    request.Headers.Add("password", password);
                    if (jsondata != "")
                    {
                        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                        {
                            streamWriter.Write(jsondata);
                        }
                    }
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream resStream = response.GetResponseStream();
                    var responseString = new StreamReader(resStream).ReadToEnd();
                    return responseString;
                }
                catch (Exception ex)
                {
                    // The remote site is currently down. Try again next time. 
                    //Airline log and kibana exception log
                    return ex.ToString();
                }
            }
            catch (Exception ex)
            {
                //Log.AddErrorMessage(jsondata, ex, "Getting Exception in ExecutePostAPI Method (urlStr : " + urlStr + " , token : "  + " , Requestairline : " + Requestairline + ")", "Exception", "", OfficeID, "", "ExecutePostAPI", InternalClassesAppSettings.GetEnvironment());
                return ex.ToString();
                //Airline log and kibana exception log 
            }
            
        }

        #region API URL
        public static string APIUrl(string Method)
        {
            string url = GetActions.AerTicketUrl;
            switch (Method)
            {
               
                case "Search":
                    url += "search";
                    break;

                case "Sell":
                    url += "verify-fare";
                    break;
                case "Booking":
                    url += "create-booking";
                    break;
                default:
                    url = "";
                    break;
            }
            return url;
        }
        #endregion
    }
}
