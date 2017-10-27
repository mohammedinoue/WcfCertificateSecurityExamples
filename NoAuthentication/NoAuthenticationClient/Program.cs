using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoAuthenticationClient.BasicService;

namespace NoAuthenticationClient
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
