using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MutualSSLAuthenticationClient
{
    class Program
    {

        static void Main(string[] args)
        {
            var baseAddress = string.Empty;
            Console.WriteLine("Hit up the \"API\" (e.g. https://localhost:port/APINAME) : ");
            baseAddress = Console.ReadLine();
            //WebRequestHandler requires System.Net.Http.WebRequest included as a reference
            var clientHandler = new WebRequestHandler();

            //Load up our certificate
            X509Certificate2 certificate;
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, "00b329f6a0eb0ea144f69885b1e388dc8000e65f", false);
                certificate = certificates.Find(X509FindType.FindByThumbprint, "00b329f6a0eb0ea144f69885b1e388dc8000e65f", false)[0];
            }
            clientHandler.ClientCertificates.Add(certificate);
            clientHandler.ServerCertificateValidationCallback += ServerCertificateValidationCallback;

            var client = new HttpClient(clientHandler)
            {
                BaseAddress = new Uri(baseAddress)
            };

            var destination = string.Empty;

            do
            {
                Console.Write("Enter the API request (e.g. \"/Get/{value\"");

                destination = Console.ReadLine();
                try
                {
                    //Hit up the API
                    var response =  client.GetAsync(destination).GetAwaiter().GetResult();

                    Console.WriteLine(response);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }while(destination != string.Empty);
        }

        private static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var x509Certificate = new X509Certificate2(certificate);
            if (x509Certificate == null) throw new InvalidOperationException("Certificate is required to access this service");

            //Check if certificate is in the trusted people store
            using (var store = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, x509Certificate.Thumbprint, false);
                if (certificates.Count == 0) return false;
            }
            return true;
        }
    }
}
