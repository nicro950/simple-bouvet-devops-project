using System.Text;
using Microsoft.Azure.ServiceBus;

namespace Bouvet.Syndicate.TestProject.Helpers.ServiceBus
{
    public class JsonMessage : Message
    {
        public JsonMessage(object data)
        {
            var jsonMessage = System.Text.Json.JsonSerializer.Serialize(data);
            var jsonMessageBytes = Encoding.UTF8.GetBytes(jsonMessage);
            Body = jsonMessageBytes;
            ContentType = "application/json";
        }
    }
}
