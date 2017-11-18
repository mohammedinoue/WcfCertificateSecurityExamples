using System;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;

namespace MutualSSLAuthenticationService
{
    class PeerTrustValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            if (certificate == null) throw new InvalidOperationException("Certificate is required to access this service");

            //Check if certificate is in the trusted people store
            using (var store = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
                if (certificates.Count == 0) throw new InvalidOperationException("Not a valid Certificate");
            }
        }
    }
}
