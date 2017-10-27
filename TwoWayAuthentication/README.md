#**WCF Two-way Certificate Authentication**

(Updated: October 26th 2017)

This example project covers how to authenticate both the client and server using certificates in WCF, all of the certificate and binding information is done in the app.config files, this example uses self-signed certificates on both the service and client end.

----------

[TOC]

##Service App.config


### < serviceBehavior >

This is where the certificate information is defined for the service. This information includes both where the service certificate is defined and how the client certificate is authenticated

    <serviceCredentials>
        <serviceCertificate findValue="9AA0366F9853B65919E765906113696BD253E5AA" storeLocation="LocalMachine" storeName="TrustedPeople" x509FindType="FindByThumbprint"/>
        <clientCertificate>
        <authentication certificateValidationMode="PeerTrust" trustedStoreLocation="LocalMachine" revocationMode="NoCheck"/>
        </clientCertificate>
    </serviceCredentials>

With WCF there are two requirements to work with **self-signed certificates**:

 1. The certificate needs to be installed in the **trustedPeople** store
 2. The validationMode needs to be set to either **PeerTrust** or **PeerOrChainTrust**
 
 In this example both the client and service certificates are installed in the **trustedPeople** store as we are just running it on a single machine.
 >**Note**: If we were using multiple machines, it would be acceptable if the service certificate was not installed in the **trustedPeople** store on the service machine and vice versa for the client machine and its certificate. 
 **However** the client machine needs the service certificate in its **trustedPeople** store and vice versa for the service machine its copy of the client certificate.
 

### < bindings >

This is where the endpoint binding information is defined for the service. The binding uses specific security elements to define the authentication between the service and client

(stuff here later, code examples etc)   

     <customBinding>
        <binding name="SecureBinding">
          <security authenticationMode="SecureConversation">
            <secureConversationBootstrap authenticationMode="MutualSslNegotiated"/>
          </security>
          <sslStreamSecurity/>
          <binaryMessageEncoding />
          <tcpTransport />
        </binding>
     </customBinding>

To properly set up a binding where we have both the client and service authenticate against each other using x509 certificates, we will use **MutualSslNegotiated** as the bootstrap for **SecureConversation**. We also need an **sslStreamSecurity** binding element.

> **Note:** When using **customBindings**, the order of binding elements is important, it should follow the order:
> 1. **Transactions**
> 2. **Reliable Messaging**
> 3. **Security**
> 4. **Encoding**
> 5. **Transport**
> This is important to note because if the security element is not in the correct place, it will not work correctly. Another thing to consider is the only element actually required in a custom binding is the **Transport** element.


### < endpoint >

    <endpoint address="" binding="customBinding" bindingConfiguration="SecureBinding" contract="TwoWayAuthenticationBasicService.IBasicService">
        <identity>
            <dns value="localhost"/>
        </identity>
    </endpoint>

The endpoint contains an element called '< identity >', for the service certificate it is required that the dns value defined by < identity > matches the Subject Name (sometimes referred to as the Common Name) of the service certificate

-----

##Client App.Config

### < endpointBehavior >

The important part of the client config is configuring the endpoint behavior to the service correctly. This is where we will define the client certificate's information and how to authenticate the service certificate.

    <clientCredentials>
        <clientCertificate findValue="28564D41EFBC34EEA87AA4A256BC3DC699DAB5B5" storeLocation ="LocalMachine" storeName="TrustedPeople" x509FindType="FindByThumbprint"/>
        <serviceCertificate>
            <authentication certificateValidationMode="PeerTrust" trustedStoreLocation="LocalMachine" revocationMode="NoCheck"/>
        </serviceCertificate>
    </clientCredentials>

Like the definition in the Service App.config, we specify where to find the certificate, then under < serviceCertificate >  we specify how to authenticate against the service certificate.


##Troubleshooting 



> Written with [StackEdit](https://stackedit.io/). 