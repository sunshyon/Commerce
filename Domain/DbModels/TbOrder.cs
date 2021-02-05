using System;
using System.Collections.Generic;

#nullable disable

namespace Domain.DbModels
{
    public partial class TbOrder
    {
        public long OrderId { get; set; }
        public long TotalPay { get; set; }
        public long ActualPay { get; set; }
        public string PromotionIds { get; set; }
        public byte PaymentType { get; set; }
        public long PostFee { get; set; }
        public DateTime? CreateTime { get; set; }
        public string ShippingName { get; set; }
        public string ShippingCode { get; set; }
        public string UserId { get; set; }
        public string BuyerMessage { get; set; }
        public string BuyerNick { get; set; }
        public bool? BuyerRate { get; set; }
        public string ReceiverState { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverDistrict { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverMobile { get; set; }
        public string ReceiverZip { get; set; }
        public string Receiver { get; set; }
        public int? InvoiceType { get; set; }
        public int? SourceType { get; set; }
    }
}
