using System.Diagnostics.CodeAnalysis;
using System.Net;
using EST.MIT.Web.Entities;
using System.Text.Json;
using AutoMapper;
using EST.MIT.Web.DTOs;

namespace EST.MIT.Web.Repositories;

public interface IInvoiceRepository
{
    Task<HttpResponseMessage> GetInvoiceAsync(string id, string scheme);
    Task<HttpResponseMessage> PostInvoiceAsync(PaymentRequestsBatchDTO paymentRequestsBatchDto);
    Task<HttpResponseMessage> PutInvoiceAsync(PaymentRequestsBatchDTO paymentRequestsBatchDto);
    Task<HttpResponseMessage> DeleteHeaderAsync(PaymentRequestDTO paymentRequestDto);
    Task<HttpResponseMessage> GetApprovalAsync(string id, string scheme);
    Task<HttpResponseMessage> GetApprovalsAsync();
}

public class InvoiceRepository : IInvoiceRepository
{
    private readonly IHttpClientFactory _clientFactory;

    public InvoiceRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseMessage> GetInvoiceAsync(string id, string scheme) => await GetInvoice(id, scheme);
    public async Task<HttpResponseMessage> PostInvoiceAsync(PaymentRequestsBatchDTO paymentRequestsBatchDto) => await PostInvoice(paymentRequestsBatchDto);
    public async Task<HttpResponseMessage> PutInvoiceAsync(PaymentRequestsBatchDTO paymentRequestsBatchDto) => await PutInvoice(paymentRequestsBatchDto);
    public async Task<HttpResponseMessage> DeleteHeaderAsync(PaymentRequestDTO paymentRequestDto) => await DeleteHeader(paymentRequestDto);
    public async Task<HttpResponseMessage> GetApprovalAsync(string id, string scheme) => await GetApproval(id, scheme);
    public async Task<HttpResponseMessage> GetApprovalsAsync() => await GetApprovals();

    private async Task<HttpResponseMessage> GetInvoice(string id, string scheme)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.GetAsync($"/invoice/{scheme}/{id}");

        await HandleHttpResponseError(response);

        return response;
    }

    private async Task<HttpResponseMessage> PostInvoice(PaymentRequestsBatchDTO paymentRequestsBatchDto)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.PostAsJsonAsync($"/invoice", paymentRequestsBatchDto);

        await HandleHttpResponseError(response);

        return response;
    }

    private async Task<HttpResponseMessage> PutInvoice(PaymentRequestsBatchDTO paymentRequestsBatchDto)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.PutAsJsonAsync($"/invoice/{paymentRequestsBatchDto.Id}", paymentRequestsBatchDto);

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> DeleteHeader(PaymentRequestDTO paymentRequestDto)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.DeleteAsync($"/invoice/header/{paymentRequestDto.PaymentRequestId}");

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> GetApproval(string id, string scheme)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.GetAsync($"/invoice/approval/{scheme}/{id}");

        await HandleHttpResponseError(response);

        return response;
    }

    [ExcludeFromCodeCoverage]
    public async Task<HttpResponseMessage> GetApprovals()
    {

        //placeholder until the API is ready

        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = new HttpResponseMessage();

        var invoices = new List<Invoice>();
        invoices.Add(new Invoice
        {
            SchemeType = "scheme",
            Approver = "approver",
            PaymentType = "invoice",
            AccountType = "account",
            Organisation = "organisation",
            PaymentRequests = new List<PaymentRequest> {
                new PaymentRequest {
                    FRN = "1234567890",
                    Value = 420
                }
            }
        });

        invoices.Add(new Invoice
        {
            SchemeType = "scheme",
            Approver = "approver",
            PaymentType = "invoice",
            AccountType = "account",
            Organisation = "organisation",
            PaymentRequests = new List<PaymentRequest> {
                new PaymentRequest {
                    FRN = "1122334455",
                    Value = 6969
                }
            }
        });

        response.Content = new StringContent(JsonSerializer.Serialize(invoices));
        response.StatusCode = HttpStatusCode.OK;

        await HandleHttpResponseError(response);

        return response;
    }

    private async static Task HandleHttpResponseError(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }
    }
}