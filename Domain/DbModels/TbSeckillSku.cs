using System;
using System.Collections.Generic;

#nullable disable

namespace Domain.DbModels
{
    public partial class TbSeckillSku
    {
        public long Id { get; set; }
        public long SkuId { get; set; }
        public string Title { get; set; }
        public long SeckillPrice { get; set; }
        public string Image { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Enable { get; set; }
    }
}
