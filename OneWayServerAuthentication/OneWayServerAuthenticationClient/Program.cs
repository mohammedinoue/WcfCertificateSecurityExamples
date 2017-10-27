using System;
using OneWayServerAuthenticationClient.BasicService;

namespace OneWayServerAuthenticationClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new BasicServiceClient();
            client.Open();
            var input = Console.ReadLine();
            if (int.TryParse(input, out int intput))
            {
                var output = client.GetData(intput);
                Console.WriteLine(output);
            }
            Console.ReadLine();
            client.Close();
        }
    }
}
