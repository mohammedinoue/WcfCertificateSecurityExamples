using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultipleEndpointAuthenticationClient.BasicService;

namespace MultipleEndpointAuthenticationClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter an endpoint to access (secureTcp | unsecureTcp | secureHttp | unsecureHttp): ");
            var endpoint = Console.ReadLine();
            var client = new BasicServiceClient(endpoint);
            client.Open();
            Console.Write("Write a number: ");
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
