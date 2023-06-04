namespace Arkix.Modules.SellingPoints.Services.ViewModels
{
    public class GetMapRequest: Paginated
    {
        public int PortalId { get; set; }
        public int Exclusive { get; set; }
        public string Department { get; set; }
        public string City { get; set; }
        public string Status { get; set; } = "PUBLIC";
    }
}