using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLXDK.Models.Views
{
    public class VoyageVM
    {
        [Key]
        public int ID { get; set; }
        
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn sà lan")]
        public int SalanId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn cảng đến")]
        public int DestinationPortId { get; set; }
    }

    public class VoyageEditVM
    {
        [Key]
        public int ID { get; set; }
        public int Teus { get; set; }
        public int SalanId { get; set; }
        public int DestinationPortId { get; set; }
    }

    public class VoyageListVm
    {
        public int ID { get; set; }
        //[Required(ErrorMessage = "Vui lòng nhập tên kho")]
        public string Name { get; set; }
        public string SalanName { get; set; }
        public string DestinationPortName { get; set; }
        public System.DateTime? CreatedAt { get; set; }
    }

    public class SubVoyageVM
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn cảng đi")]
        public int PortOfLoadingId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn cảng đến")]
        public int DestinationPortId { get; set; }
    }

    public class SubVoyageDetailVM
    {
        //[Key]
        public int ID { get; set; }
        //[Required(ErrorMessage = "Vui lòng chọn cảng đi")]
        public string PortOfLoadingName { get; set; }
        public string DestinationPortName { get; set; }
        public int Amount { get; set; }
    }

    public class VoyageDetailRow
    {
        public QLXDK.Models.Entities.VoyageDetail Detail { get; set; }
        public string ApprovedByUsername { get; set; }
    }
}