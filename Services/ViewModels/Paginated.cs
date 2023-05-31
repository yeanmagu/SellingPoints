using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arkix.Modules.SellingPoints.Services.ViewModels
{
    public class Paginated
    {
        const int maxPageSize = 100;
        private int Size { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get
            {
                return Size > 0 ? Size : 10;
            }
            set
            {
                Size = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}