using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace NoAuthenticationBasicService
{
    public class BasicService : IBasicService
    {
        public string GetData(int value)
        {
            return $"You entered: {value}\n{value} + {value} = {value + value}";
        }
    }
}
