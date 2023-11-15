using EST.MIT.Web.DTOs;

namespace EST.MIT.Web.Interfaces;

public interface IInvoiceRepository
{
    Task<HttpResponseMessage> GetInvoiceAsync(string id, string scheme);
    Task<HttpResponseMessage> PostInvoiceAsync(PaymentRequestsBatchDTO paymentRequestsBatchDto);
    Task<HttpResponseMessage> PutInvoiceAsync(PaymentRequestsBatchDTO paymentRequestsBatchDto);
    Task<HttpResponseMessage> DeleteHeaderAsync(PaymentRequestDTO paymentRequestDto);
    Task<HttpResponseMessage> GetApprovalAsync(string id);
    Task<HttpResponseMessage> GetAllApprovalsAsync();
    Task<HttpResponseMessage> GetInvoicesAsync();
}
