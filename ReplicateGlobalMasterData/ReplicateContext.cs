﻿//using Microsoft.EntityFrameworkCore;

//namespace ReplicateGlobalMasterData
//{
//    public partial class ReplicateContext : DbContext
//    {
//        public ReplicateContext() { }


//        public ReplicateContext(DbContextOptions<ReplicateContext> options) : base(options) { }

//        public virtual DbSet<sku> sku { get; set; } = null!;

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseMySql();
//            }
//        }


//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<sku>(entity =>
//            {
//            });
//        }

//    }
//}
