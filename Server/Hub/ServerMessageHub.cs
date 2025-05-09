using Newtonsoft.Json;
using Server.Interfaces;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using Shared.Requests;
using Shared.Responses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Hub;

public class ServerMessageHub : IServerMessageHub
{
    private readonly IServerQueueRepository _serverQueueRepository;
    readonly ICompanyRepository _companyRepository;
    readonly ICarRepository _carRepository;

    public ServerMessageHub(ICompanyRepository companyRepository, ICarRepository carRepository, IServerQueueRepository serverQueueRepository)
    {
        _companyRepository = companyRepository;
        _carRepository = carRepository;
        _serverQueueRepository = serverQueueRepository;
    }

    public async Task ListenForClientMessageAsync()
    {
        while (true)
        {
            var clientMessage = await _serverQueueRepository.GetMessageFromClientQueueAsync();
            if (clientMessage == null) break;

            await HandleMessageFromClientAsync(clientMessage);
        }
    }

    public async Task HandleMessageFromClientAsync(QueueEntity clientMessage)
    {
        string[] classNameParts = clientMessage.TypeName.Split('.');
        string simpleClassName = classNameParts[^1];
        var requestMessage = JsonConvert.DeserializeObject(clientMessage.Content, Helpers.GetType(clientMessage.TypeName));

        Task<object> result = simpleClassName switch
        {
            nameof(CreateCarRequest) => HandleCreateCarRequest((CreateCarRequest)requestMessage).ContinueWith(task => (object)task.Result),
            nameof(CreateCompanyRequest) => HandleCreateCompanyRequest((CreateCompanyRequest)requestMessage).ContinueWith(task => (object)task.Result),
            nameof(DeleteCarRequest) => HandleDeleteCarRequest((DeleteCarRequest)requestMessage).ContinueWith(task => (object)task.Result),
            nameof(DeleteCompanyRequest) => HandleDeleteCompanyRequest((DeleteCompanyRequest)requestMessage).ContinueWith(task => (object)task.Result),
            nameof(GetCarRequest) => HandleGetCarRequest((GetCarRequest)requestMessage).ContinueWith(task => (object)task.Result),
            nameof(GetCarsRequest) => HandleGetCarsRequest((GetCarsRequest)requestMessage).ContinueWith(task => (object)task.Result),
            nameof(GetCompanyRequest) => HandleGetCompanyRequest((GetCompanyRequest)requestMessage).ContinueWith(task => (object)task.Result),
            nameof(GetCompaniesRequest) => HandleGetCompaniesRequest((GetCompaniesRequest)requestMessage).ContinueWith(task => (object)task.Result),
            nameof(UpdateCarRequest) => HandleUpdateCarRequest((UpdateCarRequest)requestMessage).ContinueWith(task => (object)task.Result),
            nameof(UpdateCompanyRequest) => HandleUpdateCompanyRequest((UpdateCompanyRequest)requestMessage).ContinueWith(task => (object)task.Result),
            _ => throw new NotSupportedException($"Request type {clientMessage.TypeName} is not supported.")
        };

        object actualResult = await result;
        await SendMessageToClientAsync(actualResult, clientMessage.CorrelationId);
    }

    public async Task SendMessageToClientAsync(object message, Guid correlationId)
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

        await _serverQueueRepository.AddServerQueueItemAsync(entity);
    }


    private async Task<CreateCarResponse> HandleCreateCarRequest(CreateCarRequest request)
    {
        await _carRepository.AddCarAsync(request.Car);

        var response = new CreateCarResponse()
        {
            DataId = request.DataId,
            Car = request.Car
        };

        return response;
    }

    private async Task<CreateCompanyResponse> HandleCreateCompanyRequest(CreateCompanyRequest request)
    {

        await _companyRepository.AddCompanyAsync(request.Company);

        var response = new CreateCompanyResponse()
        {
            DataId = request.DataId,
            Company = request.Company
        };

        return response;
    }

    private async Task<DeleteCarResponse> HandleDeleteCarRequest(DeleteCarRequest request)
    {

        await _carRepository.RemoveCarAsync(request.CarId);

        var response = new DeleteCarResponse()
        {
            DataId = request.DataId,
        };
        return response;
    }

    private async Task<DeleteCompanyResponse> HandleDeleteCompanyRequest(DeleteCompanyRequest request)
    {

        await _companyRepository.RemoveCompanyAsync(request.CompanyId);

        var response = new DeleteCompanyResponse()
        {
            DataId = request.DataId,
        };
        return response;
    }

    private async Task<GetCarResponse> HandleGetCarRequest(GetCarRequest request)
    {
        var car = await _carRepository.GetCarAsync(request.CarId);

        var response = new GetCarResponse()
        {
            DataId = request.DataId,
            Car = car
        };
        return response;
    }

    private async Task<GetCarsResponse> HandleGetCarsRequest(GetCarsRequest request)
    {
        var cars = await _carRepository.GetAllCarsAsync();

        var response = new GetCarsResponse()
        {
            DataId = request.DataId,
            Cars = cars.ToList()
        };

        return response;
    }

    private async Task<GetCompanyResponse> HandleGetCompanyRequest(GetCompanyRequest request)
    {
        var company = await _companyRepository.GetCompanyAsync(request.CompanyId);

        var response = new GetCompanyResponse()
        {
            DataId = request.DataId,
            Company = company
        };
        return response;
    }

    private async Task<GetCompaniesResponse> HandleGetCompaniesRequest(GetCompaniesRequest request)
    {
        try
        {
            var companies = await _companyRepository.GetAllCompaniesAsync();

            var response = new GetCompaniesResponse()
            {
                DataId = request.DataId,
                Companies = companies.ToList()
            };

            return response;
        }
        catch (Exception ex)
        {
            var a = ex.Message;
            throw;
        }
    }

    private async Task<UpdateCarResponse> HandleUpdateCarRequest(UpdateCarRequest request)
    {
        await _carRepository.UpdateCarAsync(request.Car);

        var response = new UpdateCarResponse()
        {
            DataId = request.DataId,
            Car = request.Car
        };

        return response;
    }

    private async Task<UpdateCompanyResponse> HandleUpdateCompanyRequest(UpdateCompanyRequest request)
    {

        await _companyRepository.UpdateCompanyAsync(request.Company);

        var response = new UpdateCompanyResponse()
        {
            DataId = request.DataId,
            Company = request.Company
        };
        return response;
    }
}
