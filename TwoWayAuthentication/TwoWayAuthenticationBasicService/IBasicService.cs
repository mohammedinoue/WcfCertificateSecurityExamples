using System.ServiceModel;


namespace TwoWayAuthenticationBasicService
{
    [ServiceContract]
    public interface IBasicService
    {
        [OperationContract]
        string GetData(int value);
    }
}
