using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MutualSSLAuthenticationService
{
    [ServiceContract]
    public interface IBasicService
    {
        [OperationContract]
        [WebGet(UriTemplate = "Get/{value}", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Xml)]
        string GetData(string value);
    }
}
