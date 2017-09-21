using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace Api.Client
{
    class Program
    {
        private const string authority = "http://localhost:5005";
        private const string clientId = "client";
        private const string clientSecret = "secret";
        private const string scope = "";
        private const string apiUrl = "http://localhost:5001/identity";
        private static string ApiToken = "";


        static void Main(string[] args)
        {
            int userInput = 0;
            do
            {
                userInput = DisplayMenu();
                if (userInput == 1)
                {
                    RequestToken();
                }
                if (userInput == 2)
                {
                    AttemptAccess();
                }
                if (userInput == 3)
                {
                    ClearToken();
                }

            } while (userInput != 4);
        }

        static void RequestToken()
        {
            Console.WriteLine("Getting Token");
            ApiToken = GetTokenAsync().GetAwaiter().GetResult();
            Console.ReadLine();
        }

        static void AttemptAccess()
        {
            Console.WriteLine("Calling API");
            CallApiAsync(ApiToken).GetAwaiter().GetResult();
            Console.ReadLine();
        }

        static void ClearToken()
        {
            ApiToken = string.Empty;
            Console.WriteLine("Cleared Token");
        }

        static public int DisplayMenu()
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine();
                Console.WriteLine("1. Request Token");
                Console.WriteLine("2. Attempt API access");
                Console.WriteLine("3. Clear Token");
                int result;
                if (Int32.TryParse(Console.ReadLine(), out result))
                    return result;
                else
                    Console.WriteLine("Please enter a number");
            }
        }

        private static async Task MainAsync()
        {
            Console.Title = "Client";
            var token = await GetTokenAsync();

            Console.WriteLine();

            await CallApiAsync(token);

        }

        private static async Task<string> GetTokenAsync()
        {
            var disco = new DiscoveryClient(authority);
            var discoResponse = await disco.GetAsync();

            if (discoResponse.IsError)
            {
                Console.WriteLine(discoResponse.Error);
                return null;
            }

            var tokenClient = new TokenClient(discoResponse.TokenEndpoint, clientId, clientSecret);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync(scope);

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return null;
            }

            Console.WriteLine(tokenResponse.Json);

            return tokenResponse.AccessToken;
        }

        public static async Task CallApiAsync(string token)
        {
            var client = new HttpClient();
            client.SetBearerToken(token);

            var response = await client.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}
