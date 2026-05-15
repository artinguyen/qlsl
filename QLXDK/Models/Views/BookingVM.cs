using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLXDK.Models.Views
{
    public class BookingVM
    {
        [Key]
        public long ID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số booking")]
        [MaxLength(50)]
        public string BookingNo { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số chuyến")]
        [MaxLength(100)]
        public string Vessel { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        public DateTime SailingDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        public DateTime ClosingDate { get; set; }

        public DateTime? SlClosingTime { get; set; }

        public DateTime? VgmClosingDate { get; set; }
        public TimeSpan? VgmClosingTime { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng tối thiểu là 1")]
        public int Quantity { get; set; }

        [MaxLength(300)]
        public string Remark { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Depot")]
        public long? DepotId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn cảng lấy")]
        public long? PortOfLoadingId { get; set; }

        [MaxLength(100)]
        public string PickupAt { get; set; }

        [MaxLength(100)]
        public string ReturnAt { get; set; }
    }

    public class BookingListVm
    {
        public long ID { get; set; }
        public string BookingNo { get; set; }
        public string Vessel { get; set; }
        public string DepotName { get; set; }
        public string PortName { get; set; }
        public DateTime SailingDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public DateTime? SlClosingTime { get; set; }

        public DateTime? VgmClosingDate { get; set; }
        public TimeSpan? VgmClosingTime { get; set; }
        public int Quantity { get; set; }
        public string PickupAt { get; set; }

        public string ReturnAt { get; set; }
        public string Remark { get; set; }
    }
}