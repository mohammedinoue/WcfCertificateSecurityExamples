#### **WCF Multiple Endpoints Authentication**

(Updated: November 18th 2017)

This example shows how to use multiple endpoints to access the same service where each endpoint differs through binding and security

----------

## Service App.config


### < endpoints >

Under the same service in the services app.config file, we are going to create 4 endpoints that are slightly different but all have the same contract **MultipleEndpointBasicService.IBasicService**

	<endpoint name="secureTcp" address="secure" binding="netTcpBinding" bindingConfiguration="secureNetTcpBinding" contract="MultipleEndpointBasicService.IBasicService">
		<identity>
			<dns value="localhost"/>
		</identity>
	</endpoint>
	<endpoint name="unsecureTcp" address="unsecure" binding="netTcpBinding" bindingConfiguration="unsecureNetTcpBinding" contract="MultipleEndpointBasicService.IBasicService">
		<identity>
			<dns value="localhost"/>
		</identity>
	</endpoint>
	<endpoint name="secureHttp" address="secure" binding="wsHttpBinding" bindingConfiguration="secureHttpBinding" contract="MultipleEndpointBasicService.IBasicService">
		<identity>
			<dns value="localhost"/>
		</identity>
	</endpoint>
	<endpoint name="unsecureHttp" address="unsecure" binding="wsHttpBinding" bindingConfiguration="unsecureHttpBinding" contract="MultipleEndpointBasicService.IBasicService">
		<identity>
			<dns value="localhost"/>
		</identity>
	</endpoint>

The important parts to note is both the name of the endpoint and the address.
To take advantage of multiple endpoints, we can use baseAddress in our service. Within the < service > we have defined the endpoint, we can add this: 

	<host>
		<baseAddresses>
			<add baseAddress = "http://localhost:8733/MultipleEndpoints/" />
            <add baseAddress = "https://localhost:8734/MultipleEndpoints/" />
            <add baseAddress = "net.tcp://localhost:8735/MultipleEndpoints/" />
		</baseAddresses>
	</host>

Each base address look very similar, but they differ in the prefix (http, https, and net.tcp) as well as the port, we don not have to worry about this too much
as it is something that wcf wil take care of us automatically. The unsecureHttp endpoint will be mapped to the http base address, the secureHttp endpoint to the https base address, and 
both of the net tcp endpoints will be mapped to the net.tcp base address.

### < bindings >

This is where the endpoint binding information is defined for the service. The binding uses specific security elements to define the authentication between the service and client
To take advantage of our multiple endpoints we can define unique bindings to each of them and define them under the < binding > tag within the app.config file
 
	<netTcpBinding>
        <binding name ="secureNetTcpBinding">
          <security mode="Transport">
            <transport clientCredentialType="None"/>
          </security>
        </binding>
			<binding name ="unsecureNetTcpBinding">
        </binding>
	</netTcpBinding>
	<wsHttpBinding>
        <binding name ="secureHttpBinding">
          <security mode="Transport">
            <transport clientCredentialType="None"/>
          </security>
        </binding>
			<binding name ="unsecureHttpBinding">
        </binding>
	</wsHttpBinding>

As you can see we have two bindings that are unsecure, and two bindings that are secure. This means that if a user tries to connect to secureNetTcpBinding or secureHttpBinding, 
the service needs to present a certificate for the client. If the user tries to access the unsecureHttpBding or unsecureNetTcpBinding, the certificate will not be used.

>*Note* upon starting up the service, a certificate will still be needed if any of the services/bindings are using it, however for the purpose of connecting to a specific endpoint, it will not be used


### < serviceBehaviors >

Every endpoint defined is under 1 service, thus we only need to use one service behavior

	<behavior name="serviceBehavior">
		<serviceMetadata httpGetEnabled="True" httpsGetEnabled="True"/>
		<serviceDebug includeExceptionDetailInFaults="False" />
		<serviceCredentials>
			<serviceCertificate x509FindType="FindByThumbprint" findValue ="3c4069c8baa2381a4842d3502865fd7b6643d04e" storeLocation="LocalMachine" storeName="TrustedPeople"/>
        </serviceCredentials>
	</behavior>

As noted above, since we are using a certificate for some of our bindings, *we stil need to define a certificate*. 
-----

## Client App.Config

### < endpointBehavior >

The important part of the client config is configuring the endpoint behavior to the service correctly. This is where we will define the client certificate's information and how to authenticate the service certificate.
Since only some of the bindings use certificates, this behavior will only need to be attached to the endpoints that are secure.

    <clientCredentials>
		<serviceCertificate>
            <authentication certificateValidationMode="PeerTrust" trustedStoreLocation="LocalMachine" revocationMode="NoCheck"/>
        </serviceCertificate>
    </clientCredentials>

### < bindings >

The bindings that are within the client config should look the same as the one on the services config file. But there is an important step that needs to be taken.
As stated above, we will only be using certificates upon accessing a secure binding, so we add a **behaviorConfiguration** called securePeerTrust. This behaviorConfiguration is defined above.
The other two bindings *do not require any behaviorConfiguration.*

            <endpoint address="net.tcp://localhost:8735/MultipleEndpoints/secure"
                binding="netTcpBinding" behaviorConfiguration="securePeerTrust" bindingConfiguration="secureTcp" contract="BasicService.IBasicService"
                name="secureTcp">
                <identity>
                    <dns value="localhost" />
                </identity>
            </endpoint>
            <endpoint address="net.tcp://localhost:8735/MultipleEndpoints/unsecure"
                binding="netTcpBinding" bindingConfiguration="unsecureTcp" contract="BasicService.IBasicService"
                name="unsecureTcp">
                <identity>
                    <dns value="localhost" />
                </identity>
            </endpoint>
            <endpoint address="https://localhost:8734/MultipleEndpoints/secure"
                binding="wsHttpBinding" behaviorConfiguration="securePeerTrust" bindingConfiguration="secureHttp"
                contract="BasicService.IBasicService" name="secureHttp">
                <identity>
                    <dns value="localhost" />
                </identity>
            </endpoint>
            <endpoint address="http://localhost:8733/MultipleEndpoints/unsecure"
                binding="wsHttpBinding" bindingConfiguration="unsecureHttp"
                contract="BasicService.IBasicService" name="unsecureHttp">
                <identity>
                    <dns value="localhost" />
                </identity>

Like the definition in the Service App.config, we specify where to find the certificate, then under < serviceCertificate >  we specify how to authenticate against the service certificate.

