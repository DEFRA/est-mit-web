using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Helpers;

namespace EST.MIT.Web.Pages.create_bulk.OrganisationMetaSelection;

public partial class OrganisationMetaSelectionBulk : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] public IPageServices _pageServices { get; set; } = default!;
    [Inject] private IReferenceDataAPI _referenceDataAPI { get; set; }
    [Inject] private ILogger<OrganisationMetaSelectionBulk> Logger { get; set; }

    private Invoice invoice = default!;
    private OrganisationSelect organisationSelect = new();
    private Dictionary<string, string> organisations = new();
    bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();
    private List<string> viewErrors = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
            invoice = _invoiceStateContainer.Value;

            if (invoice != null && !invoice.IsNull())
            {
                var result = await _referenceDataAPI.GetOrganisationsAsync(invoice.AccountType);
                if (result.IsSuccess)
                {
                    foreach (var org in result.Data)
                    {
                        organisations.Add(org.code, org.description);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing OrganisationMetaSelectionBulk page");
            _nav.NavigateTo("/error");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            await base.OnAfterRenderAsync(firstRender);
            if (_invoiceStateContainer.Value == null || _invoiceStateContainer.Value.IsNull())
            {
                _nav.NavigateTo("/create-bulk");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in OnAfterRenderAsync of OrganisationMetaSelectionBulk page");
            _nav.NavigateTo("/error");
        }
    }

    private void SaveAndContinue()
    {
        try
        {
            invoice.Organisation = organisationSelect.Organisation;
            _invoiceStateContainer.SetValue(invoice);
            _nav.NavigateTo("/create-bulk/scheme");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in SaveAndContinue of OrganisationMetaSelectionBulk page");
            _nav.NavigateTo("/error");
        }
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