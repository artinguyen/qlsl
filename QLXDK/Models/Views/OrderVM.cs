using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLXDK.Models.Views
{
    public class OrderVM
    {
        [Key]
        public long ID { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn booking")]
        public long BookingId { get; set; }

        [MaxLength(50)]
        public string LsgNum { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        public DateTime OrderDate { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        public int CustomerId { get; set; }
        public string PlateNum { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn tài xế")]
        public int UserId { get; set; }
        public string XN { get; set; }
        public string ContainerMooc { get; set; }
        public string Container { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn kích cỡ")]
        public int SizeId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn cảng lấy")]
        public int PortOfLoadingId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn kho")]
        public int TempLocationId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn cảng hạ")]
        public int UnloadingLocationId { get; set; }

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
    }

    public class OrderListVM
    {
        [Key]
        public long ID { get; set; }
        public long BookingId { get; set; }

        public string LsgNum { get; set; }

        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string PlateNum { get; set; }

        public string UserName { get; set; }
        public string XN { get; set; }
        public string ContainerMooc { get; set; }
        public string Container { get; set; }
        public string Size { get; set; }
        public string PortOfLoadingName { get; set; }
        public string TempLocationName { get; set; }
        public string UnloadingLocationName { get; set; }

        public decimal DriverFee { get; set; }
        public decimal PurchaseFee { get; set; }
        public decimal SellingFee { get; set; }
        public decimal CustomerFee { get; set; }
        public int Commission { get; set; }
        public decimal UnloadingFee { get; set; }
        public long UnloadingInvoiceNo { get; set; }
        public decimal LoadingFee { get; set; }
        public long LoadingInvoiceNo { get; set; }
        public decimal InspectionFee { get; set; }
        public long InspectionInvoiceNo { get; set; }
        public decimal AdditionalFee { get; set; }
        public long AdditionalInvoiceNo { get; set; }
        public decimal ContainerCharge { get; set; }
    }
}