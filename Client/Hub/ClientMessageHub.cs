using Client.Interfaces;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using System;
using System.Threading.Tasks;

namespace Client.Hub
{
    public class ClientMessageHub : IClientMessageHub
    {
        private readonly IClientQueueRepository _clientQueueRepository;

        public ClientMessageHub(IClientQueueRepository clientQueueRepository)
        {
            _clientQueueRepository = clientQueueRepository;
        }

        public async Task SendMessageToServerAsync(object message, Guid correlationId)
        {
            var result = Helpers.ConvertObjectToJson(message);
            var entity = new QueueEntity
            {
                CorrelationId = correlationId,
                Content = result.Item1,
                TypeName = result.Item2.ToString(),
                Created = DateTime.Now,
                StatusDate = DateTime.Now,
                QueueStatus = QueueStatus.New
            };

            await _clientQueueRepository.AddClientQueueItemAsync(entity);
        }

        public async Task<TResponse> ListenForMessageFromServerAsync<TResponse>(Guid correlationId)
        {
            var response = await ReceiveServerMessage(correlationId);
            return (TResponse)Helpers.ConvertJsonToObject(response.Content, Helpers.GetType(response.TypeName));
        }

        private async Task<QueueEntity> ReceiveServerMessage(Guid correlationId)
        {
            var response = await _clientQueueRepository.GetMessageFromServerByCorrelationIdAsync(correlationId);
            while (response == null)
            {
                await Task.Delay(100);
                response = await _clientQueueRepository.GetMessageFromServerByCorrelationIdAsync(correlationId);
            }

            return response;
        }
    }
}
