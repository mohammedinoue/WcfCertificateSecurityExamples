

namespace TwoWayAuthenticationBasicService
{
    public class BasicService : IBasicService
    {
        public string GetData(int value)
        {
            return $"You entered: {value}\n{value} + {value} = {value + value}";
        }
    }
}
