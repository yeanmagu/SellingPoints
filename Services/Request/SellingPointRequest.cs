using Arkix.Modules.SellingPoints.Services.ViewModels;
namespace Arkix.Modules.SellingPoints.Services.Request
{
    public class SellingPointRequest: Paginated
    {
        public string Search { get; set; }
        public int PortalId { get; set; }
    }
}