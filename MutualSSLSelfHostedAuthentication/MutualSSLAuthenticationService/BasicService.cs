using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MutualSSLAuthenticationService
{
    public class BasicService : IBasicService
    {
        public string GetData(string value)
        {
            return $"You entered: {value}";
        }
    }
}
