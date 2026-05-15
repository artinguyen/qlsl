using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLXDK.Models.Entities
{
    public class Booking
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string BookingNo { get; set; }

        [MaxLength(100)]
        public string Vessel { get; set; }

        // SQL DATE => C# DateTime?
        public DateTime SailingDate { get; set; }
        public DateTime ClosingDate { get; set; }

        // SQL TIME(0) => C# TimeSpan?
        public DateTime? SlClosingTime { get; set; }

        public DateTime? VgmClosingDate { get; set; }
        public TimeSpan? VgmClosingTime { get; set; }

        public int Quantity { get; set; }

        [MaxLength(300)]
        public string Remark { get; set; }

        public long? DepotId { get; set; }
        public long? PortOfLoadingId { get; set; }

        // Trong SQL của bạn đang là NVARCHAR(100) => model để string
        [MaxLength(100)]
        public string PickupAt { get; set; }

        [MaxLength(100)]
        public string ReturnAt { get; set; }
    }
}