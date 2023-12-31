using System.Text;
using System.Text.Json;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Models;

namespace EST.MIT.Web.Services;

public class EventQueueService : IEventQueueService
{
    private readonly ILogger<IEventQueueService> _logger;
    private readonly QueueClient _queueClient;

    public EventQueueService(QueueClient queueClient, ILogger<IEventQueueService> logger)
    {
        _queueClient = queueClient;
        _logger = logger;
    }

    public async Task<bool> AddMessageToQueueAsync(string message, string data)
    {
        var eventRequest = new Event()
        {
            Name = "Payments",
            Properties = new EventProperties()
            {
                Status = "",
                Checkpoint = "",
                Action = new EventAction()
                {
                    Type = "",
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                    Data = data
                }
            }
        };

        try
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventRequest));
            await _queueClient.SendMessageAsync(Convert.ToBase64String(bytes));

            return true;

        }
        catch (RequestFailedException ex)
                when (ex.ErrorCode == QueueErrorCode.QueueAlreadyExists)
        {
            // Ignore any errors if the queue already exists

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error {ex.Message} sending \"{message}\" message to Event Queue.");
            return false;
        }
    }
}