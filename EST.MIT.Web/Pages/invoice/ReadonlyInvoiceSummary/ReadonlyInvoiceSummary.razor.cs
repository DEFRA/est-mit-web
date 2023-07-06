using Microsoft.AspNetCore.Components;
using Entities;
using Helpers;
using Services;

namespace EST.MIT.Web.Pages.invoice.ReadonlyInvoiceSummary;

public partial class ReadonlyInvoiceSummary : ComponentBase
{
    [Parameter] public string Scheme { get; set; } = default!;
    [Parameter] public string Id { get; set; } = default!;
    [Parameter] public bool Approval { get; set; } = false;

    [Inject] private IApprovalService _approvalService { get; set; }
    [Inject] private NavigationManager _nav { get; set; }

    private Invoice invoice = default!;
    private bool IsErrored = false;
    private Dictionary<string, string> Errors = new Dictionary<string, string>();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        try
        {
            if (!Approval)
                invoice = await _approvalService.GetInvoiceAsync(Id, Scheme);
            else
                invoice = await _approvalService.GetApprovalAsync(Id, Scheme);
        }
        catch (Exception ex)
        {
            IsErrored = true;
            Errors.Add("ApprovalService", ex.Message);
            return;
        }

        if (invoice.IsNull())
        {
            IsErrored = true;
            Errors.Add("Invoice", "Invoice not found");
        }
    }

    private async Task ApproveInvoice()
    {
        await _approvalService.ApproveInvoiceAsync(invoice);
    }

    private async Task RejectInvoice()
    {
        await _approvalService.RejectInvoiceAsync(invoice, "Rejected by user");
    }
}