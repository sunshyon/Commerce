using System;
using System.Collections.Generic;

#nullable disable

namespace Domain.DbModels
{
    public partial class TbBrand
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Letter { get; set; }
    }
}
