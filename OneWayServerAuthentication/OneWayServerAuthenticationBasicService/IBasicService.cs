using System.ServiceModel;

namespace OneWayServerAuthenticationBasicService
{
    [ServiceContract]
    public interface IBasicService
    {
        [OperationContract]
        string GetData(int value);

    }
}
