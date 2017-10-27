#**WCF One-way Service Certificate Authentication **

(Updated: October 26th 2017)

This example project covers how to authenticate a server using certificates in WCF, all of the certificate and binding information is done in the app.config files, this example uses self-signed certificates for the service.

----------

[TOC]

##Service App.config


### < serviceBehavior >

This is where the certificate information is defined for the service. This information includes both where the service certificate is defined

    <serviceCredentials>
        <serviceCertificate findValue="9AA0366F9853B65919E765906113696BD253E5AA" storeLocation="LocalMachine" storeName="TrustedPeople" x509FindType="FindByThumbprint"/>
    </serviceCredentials>

With WCF there are two requirements to work with **self-signed certificates**:

 1. The certificate needs to be installed in the **trustedPeople** store
 2. The validationMode needs to be set to either **PeerTrust** or **PeerOrChainTrust**
 
 In this example the service certificates are installed in the **trustedPeople** store as we are just running it on a single machine.
 >**Note**: If we were using multiple machines, it would be acceptable if the service certificate was not installed in the **trustedPeople** store on the service machine. 
 **However** the client machine needs the service certificate in its **trustedPeople** store.
 

### < bindings >

This is where the endpoint binding information is defined for the service. The binding uses specific security elements to define the authentication between the service and client


     <customBinding>
        <binding name="SecureBinding">
          <security authenticationMode="SecureConversation">
            <secureConversationBootstrap authenticationMode="SspiNegotiated"/>
          </security>
          <sslStreamSecurity/>
          <binaryMessageEncoding />
          <tcpTransport />
        </binding>
     </customBinding>

To properly set up a binding where we the service authenticate against the client using a x509 certificate, we will use **SspiNegotiated** as the bootstrap for **SecureConversation**. We also need an **sslStreamSecurity** binding element.

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

The important part of the client config is configuring the endpoint behavior to the service correctly. This is where we will define how to authenticate the service certificate.

    <clientCredentials>
        <serviceCertificate>
            <authentication certificateValidationMode="PeerTrust" trustedStoreLocation="LocalMachine" revocationMode="NoCheck"/>
        </serviceCertificate>
    </clientCredentials>

Under < serviceCertificate >  we specify how to authenticate against the service certificate. Because we are using self-signed certificates, the **certificateValidationMode** is set to **PeerTrust**, **PeerOrChainTrust** would also work as that will check to see if the certificate will fail against **ChainTrust**, which it will, it will fallback and then use **PeerTrust** where it should succeed 


##Troubleshooting 



> Written with [StackEdit](https://stackedit.io/). 