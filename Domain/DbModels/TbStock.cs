using System;
using System.Collections.Generic;

#nullable disable

namespace Domain.DbModels
{
    public partial class TbStock
    {
        public long SkuId { get; set; }
        public int? SeckillStock { get; set; }
        public int? SeckillTotal { get; set; }
        public int Stock { get; set; }
    }
}
