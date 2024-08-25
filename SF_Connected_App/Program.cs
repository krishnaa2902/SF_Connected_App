using Newtonsoft.Json;

namespace SF_Connected_App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var salesforceClient = new SalesforceClient();
            var accessToken = await salesforceClient.GetAccessTokenAsync();
            if(accessToken == null)
            {
                Console.WriteLine("Authentication failed");
            }
            else
            {
                string jsonContent = await File.ReadAllTextAsync("CaseDetails.json");
                var casesObject = JsonConvert.DeserializeObject<List<object>>(jsonContent);
                await salesforceClient.GetAllCasesAsync();
                await salesforceClient.CreateCasesAsync(casesObject);
            }
        }
    }
}