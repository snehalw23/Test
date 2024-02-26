namespace AERTicketWebService.Common
{
    public class Common
    {
        private static IConfiguration? config;

        private static IConfiguration Configuration
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
                config = builder.Build();
                return config;
            }
        }
        public static class GetActions
        {
            public static readonly string AerTicketUrl = Configuration["AppSettings:endpoint:address"];
            //public static readonly string EndPoint1 = Configuration["AppSettings:endpoint:address1"];
            public static readonly string ElasticLogURL = Configuration["AppSettings:elasticDetails:ElasticLogURL"];
            public static readonly string Environment = Configuration["AppSettings:elasticDetails:SetEnvironment"];
        }
        public static string GetGUID()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff") + "_" + Guid.NewGuid().ToString();
        }
    }
}
