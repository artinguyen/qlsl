using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLXDK.Models.Entities
{
    public class VoyageDetail
    {
        [Key]
        public int ID { get; set; }
        //[Required(ErrorMessage = "Vui lòng nhập tên kho")]
        public int SubVoyageId { get; set; }

        [ForeignKey("SubVoyageId")]
        public virtual Voyage SubVoyage { get; set; }

        //public string Date { get; set; }
        public string ContainerNo { get; set; }
        public string SizeType { get; set; }
        public string Line { get; set; }
        public string VesVoyName { get; set; }
        public string PortOfLoad { get; set; }
        public string PortOfDischarge { get; set; }
        public string BookingBillNo { get; set; }
        public string SealNo { get; set; }
        public string FullEmpty { get; set; }
        public string TemperatureC { get; set; }
        public string Commodity { get; set; }
        public string Category { get; set; }
        public double GrossWeight { get; set; }
        public double VGM { get; set; }
        public string IMO { get; set; }
        public string UN { get; set; }
        public string Remarks { get; set; }
        public string User { get; set; }
        public string ICDs { get; set; }
        public string Terminal { get; set; }
        public string BargeName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public int? ApprovedBy { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public int? DeletedBy { get; set; }

        //public System.DateTime? CreatedAt { get; set; }
        //public System.DateTime? UpdatedAt { get; set; }
    }
}