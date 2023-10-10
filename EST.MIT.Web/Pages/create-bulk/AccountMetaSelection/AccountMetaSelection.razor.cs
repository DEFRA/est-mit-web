using Entities;
using EST.MIT.Web.Shared;
using Helpers;
using Microsoft.AspNetCore.Components;
using Services;

namespace EST.MIT.Web.Pages.create_bulk.AccountMetaSelection;

public partial class AccountMetaSelection : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] public IPageServices _pageServices { get; set; }

    private Invoice invoice = default!;
    private AccountSelect accountSelect = new();
    readonly Dictionary<string, string> accountTypes = new()
    {
        { "AR", "Accounts Receivable" },
        { "AP", "Accounts Payable" }
    };
    bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();
    private List<string> viewErrors = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        invoice = _invoiceStateContainer.Value;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (_invoiceStateContainer.Value.IsNull())
        {
            _invoiceStateContainer.SetValue(null);
            _nav.NavigateTo("/create-bulk");
        }
    }

    private void SaveAndContinue()
    {
        invoice.AccountType = accountSelect.Account;
        _invoiceStateContainer.SetValue(invoice);
        _nav.NavigateTo("/create-bulk/organisation");
    }

    private void ValidationFailed()
    {
        _pageServices.Validation(accountSelect, out IsErrored, out errors);
        viewErrors = errors[nameof(accountSelect.Account).ToLower()];
    }

    private void Cancel()
    {
        _invoiceStateContainer.SetValue(null);
        _nav.NavigateTo("/");
    }
}