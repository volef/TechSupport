using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RequestSender
{
    internal static class Program
    {
        private static int minTimeMs, maxTimeMs, count;
        private static readonly Random random = new Random();
        private static HttpClient client;
        private const string host = "https://localhost:5001/api/SupportRequest";

        private static void Main(string[] args)
        {
            int.TryParse(Console.ReadLine(), out minTimeMs);
            int.TryParse(Console.ReadLine(), out maxTimeMs);
            int.TryParse(Console.ReadLine(), out count);
            //
            client = new HttpClient();
            var task = Task.Run(Test);
            Console.ReadLine();
        }

        public static async Task Test()
        {
            for (var i = 0; i < count; i++)
            {
                await Task.Delay(random.Next(minTimeMs, maxTimeMs));
                var json = JsonConvert.SerializeObject(new
                    {head = StringGenerator.Get(6), text = StringGenerator.Get(15)});
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                try
                {
                    var response = await client.PostAsync(host, data);
                    Console.WriteLine($"json {json}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}