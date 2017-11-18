# **WCF Mutual SSL SelfHosted Authentication**

(Updated: November 17th 2017)

This example project covers how to authenticate both the client and server using certificates in WCF, this example uses self-signed certificates on both the service and client end.

----------

## Service App.config


### < serviceBehavior >

This is where the certificate information is defined for the service. This information includes both where the service certificate is defined and how the client certificate is authenticated.
In this example because we are authenticating to the service through the transport layer, we need to do an extra step for client certificate validation.

        <behavior name="SecureWebBehavior">
          <serviceMetadata httpGetEnabled="false" httpsGetEnabled="True"/>
          <serviceCredentials>
            <clientCertificate>
              <authentication certificateValidationMode="Custom" customCertificateValidatorType="MutualSSLAuthenticationService.PeerTrustValidator, MutualSSLAuthenticationService" />
            </clientCertificate>
            <serviceCertificate x509FindType="FindByThumbprint" findValue ="3c4069c8baa2381a4842d3502865fd7b6643d04e" storeLocation="LocalMachine" storeName="TrustedPeople"/>
          </serviceCredentials>
        </behavior>

We are using a custom validator for the client certificate as shown by **certificateValidationMode="Custom"** the location of which is specified by **customCertificateValidatorType**

The custom validator is defined in **PeerTrustValidator.cs**, it mimics the normal PeerTrust setting for certificate validation, by simply checking if the certificate received from the client exist
in the services **TrustedPeople** store

### < bindings >

This is where the endpoint binding information is defined for the service. The binding uses specific security elements to define the authentication between the service and client


	  <webHttpBinding>
        <binding name="webHttpSecure">
          <security mode="Transport">
            <transport clientCredentialType="Certificate"/>
          </security>
        </binding>
      </webHttpBinding>

To set up the binding for a REST API setting, we need to use webHttpBinding, which only allows security through Transport.
Then under the transport tag, we specify we are looking for a certificate from the client

### < endpoint >

        <service name="MutualSSLAuthenticationService.BasicService" behaviorConfiguration="SecureWebBehavior">
          <host>
            <baseAddresses>
              <add baseAddress = "https://localhost:3432/MutualSSLAuthentication/" />
            </baseAddresses>
          </host>
          <endpoint address="" binding="webHttpBinding" bindingConfiguration="webHttpSecure" behaviorConfiguration="WebHttp" contract="MutualSSLAuthenticationService.IBasicService">
            <identity>
              <dns value="localhost"/>
            </identity>
          </endpoint>
		</service>

The endpoint contains an element called < identity >, for the service certificate it is required that the dns value defined by < identity > matches the Subject Name (sometimes referred to as the Common Name) of the service certificate
In this example we define a base address which we are going to use to connect to the service.

### Netsh

Because we are self-hosting our service we need to bind the port directly using netsh with the server certificate. This can be done through the Command Prompt
The syntax we need to use is 

	netsh http add sslcert ipport=0.0.0.0:3432 appid={00000000-0000-0000-0000-000000000000} certhash=[Certificate Thumbprint] clientcertnegotiation=enable certstorename=TrustedPeople

ipport should match the port we are hosting our service on, in this case **3432** having the ipaddress be set to 0.0.0.0 means we don't care about the ip address we only care about the port
the appid is the id of the application (having it set to all 0s will suffice), 
**certhash** should be set to the thumbprint of our server certificate
**clientcertnegotiation** should be set to enable so the service requests the client's certificate, default is disable
**certstorename** is the location where the server certificate is stored, default is My

-----

## Client Application

Because we are directly connecting to the endpoint, we set the connection directly programmically in the client

Since we set up the port we are connecting through using Netsh, upon connecting the client to the service, the service will make its certificate avaliable, and authentication will occur at that point.

	var clientHandler = new WebRequestHandler();

	X509Certificate2 certificate;

	using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
	{
		store.Open(OpenFlags.ReadOnly);
		var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, "00b329f6a0eb0ea144f69885b1e388dc8000e65f", false);
		certificate = certificates.Find(X509FindType.FindByThumbprint, "00b329f6a0eb0ea144f69885b1e388dc8000e65f", false)[0];
	}

	clientHandler.ClientCertificates.Add(certificate);
	clientHandler.AuthenticationLevel = AuthenticationLevel.MutualAuthRequested;

	var client = new HttpClient(clientHandler)
	{
		BaseAddress = new Uri(baseAddress)
	};

Our client uses WebRequestHandler since that will allow us to tie our HttpClient with our certificate information

We then load the certificate from the machine we are on and attach it to the clientHandler. We set the clientHandlers authenticationLevel to
MutualAuthRequested

Finally we hook our client up with the client handler we have created

Then we can use that client to connect to the service and authentication will occur.

