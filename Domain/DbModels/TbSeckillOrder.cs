using System;
using System.Collections.Generic;

#nullable disable

namespace Domain.DbModels
{
    public partial class TbSeckillOrder
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long OrderId { get; set; }
        public long SkuId { get; set; }
    }
}
