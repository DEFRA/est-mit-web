using Entities;
using Services;
using EST.MIT.Web.Shared;
using Microsoft.AspNetCore.Components;
using Helpers;

namespace EST.MIT.Web.Pages.create_bulk.OrganisationMetaSelection;

public partial class OrganisationMetaSelection : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] public IPageServices _pageServices { get; set; } = default!;
    [Inject] private IReferenceDataAPI _referenceDataAPI { get; set; }

    private Invoice invoice = default!;
    private OrganisationSelect organisationSelect = new();
    private Dictionary<string, string> organisations = new();
    bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();
    private List<string> viewErrors = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        invoice = _invoiceStateContainer.Value;

        if (!invoice.IsNull())
        {
            await _referenceDataAPI.GetOrganisationsAsync(invoice.AccountType).ContinueWith(x =>
            {
                if (x.Result.IsSuccess)
                {
                    foreach (var org in x.Result.Data)
                    {
                        organisations.Add(org.code, org.description);
                    }
                }
            });
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (_invoiceStateContainer.Value.IsNull())
        {
            _nav.NavigateTo("/create-bulk");
        }
    }

    private void SaveAndContinue()
    {
        invoice.Organisation = organisationSelect.Organisation;
        _invoiceStateContainer.SetValue(invoice);
        _nav.NavigateTo("/create-bulk/scheme");
    }

    private void ValidationFailed()
    {
        _pageServices.Validation(organisationSelect, out IsErrored, out errors);
        viewErrors = errors[nameof(organisationSelect.Organisation).ToLower()];
    }

    private void Cancel()
    {
        _invoiceStateContainer.SetValue(null);
        _nav.NavigateTo("/");
    }
}