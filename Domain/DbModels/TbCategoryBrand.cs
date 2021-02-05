using System;
using System.Collections.Generic;

#nullable disable

namespace Domain.DbModels
{
    public partial class TbCategoryBrand
    {
        public long CategoryId { get; set; }
        public long BrandId { get; set; }
    }
}
