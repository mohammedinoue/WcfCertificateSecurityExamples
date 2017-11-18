using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MultipleEndpointBasicService
{
    [ServiceContract]
    public interface IBasicService
    {
        [OperationContract]
        string GetData(int value);
    }
}
