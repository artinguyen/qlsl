using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using QLXDK.Models;

namespace QLXDK.Models
{
    public class qlslContext : DbContext
    {
        // 1. Khởi tạo và trỏ đến tên Connection String trong Web.config
        public qlslContext() : base("name=qlslConnect")
        {
        }

        // 2. Khai báo bảng Users (Dùng lớp Entity 'User', không phải 'UserVM')
        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.Customer> Customers { get; set; }
        public DbSet<Entities.Depot> Depots { get; set; }
        public DbSet<Entities.Salan> Salans { get; set; }
        public DbSet<Entities.UnloadingLocation> UnloadingLocations { get; set; }
        public DbSet<Entities.PortOfLoading> PortOfLoadings { get; set; }
        public DbSet<Entities.Booking> Bookings { get; set; }
        public DbSet<Entities.Size> Sizes { get; set; }
        //public DbSet<Views.BookingVM> Bookings { get; set; }
        public DbSet<Entities.Voyage> Voyages { get; set; }
        public DbSet<Entities.SubVoyage> SubVoyages { get; set; }
        public DbSet<Entities.VoyageDetail> VoyageDetails { get; set; }
        public DbSet<Entities.Order> Orders { get; set; }

        public DbSet<Entities.DestinationPort> DestinationPorts { get; set; }
    }
}