using Azure.Messaging.ServiceBus;
using CineWorld.MessageBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineWorld.MessageBus
{
  public class MessageBus : IMessageBus
  {
    private const string ConnectionString = "Endpoint=sb://hquoanmangoweb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=FGy2hc5x7zJtewTKB3q7JYF2BLAFqSz8D+ASbPQeE8g=";

    public async Task PublishMessage(object message, string topic_queue_Name)
    {
      await using var client = new ServiceBusClient(ConnectionString);

      ServiceBusSender sender = client.CreateSender(topic_queue_Name);

      var jsonMessage = JsonConvert.SerializeObject(message);
      ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
      {
        CorrelationId = Guid.NewGuid().ToString(),
      };

      await sender.SendMessageAsync(finalMessage);
      await client.DisposeAsync();
    }
  }
}
