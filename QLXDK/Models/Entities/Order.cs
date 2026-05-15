using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLXDK.Models.Entities
{
    public class Order
    {
        [Key]
        public long ID { get; set; }

        public long BookingId { get; set; }

        [MaxLength(50)]
        public string LsgNum { get; set; }

        // SQL DATE => C# DateTime?
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string PlateNum { get; set; }
        public int UserId { get; set; }

        public string ContainerMooc { get; set; }
        public string Container { get; set; }
        public string XN { get; set; }
        public int SizeId { get; set; }
        public int PortOfLoadingId { get; set; }
        public int UnloadingLocationId { get; set; }
        public int TempLocationId { get; set; }
        
        public decimal? DriverFee { get; set; }
        public decimal? PurchaseFee { get; set; }
        public decimal? SellingFee { get; set; }
        public decimal? CustomerFee { get; set; }
        public int? Commission { get; set; }
        public decimal? UnloadingFee { get; set; }
        public long? UnloadingInvoiceNo { get; set; }
        public decimal? LoadingFee { get; set; }
        public long? LoadingInvoiceNo { get; set; }
        public decimal? InspectionFee { get; set; }
        public long? InspectionInvoiceNo { get; set; }
        public decimal? AdditionalFee { get; set; }
        public long? AdditionalInvoiceNo { get; set; }
        public decimal? ContainerCharge { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public Nullable<System.DateTime> DeletedAt { get; set; }
        
    }
}