namespace Arkix.Modules.SellingPoints.Services.ViewModels
{
    public class GetMapRequest
    {
        public int PageIndex { get; set; }
        public int PortalId { get; set; }
        public int PageSize { get; set; } = 10;
        public int Exclusive { get; set; }
        public string Department { get; set; }
        public string City { get; set; }
        public string Status { get; set; } = "PUBLIC";
    }
}