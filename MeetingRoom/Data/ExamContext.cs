﻿using MeetingRoom.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Threading.Tasks;

namespace MeetingRoom.Data
{
    public class ExamContext : DbContext
    {
        private IDbContextTransaction _currentTransaction;

        public ExamContext(DbContextOptions<ExamContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomAttribute> RoomAttributes { get; set; }
        public DbSet<RoomItem> RoomItems { get; set; }
        public DbSet<Food> FoodItems { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ServingSchedule> ServingSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>().ToTable("Room");
            modelBuilder.Entity<RoomAttribute>().ToTable("RoomAttribute");
            modelBuilder.Entity<RoomItem>().ToTable("RoomItem");
            modelBuilder.Entity<Food>().ToTable("Food");
            modelBuilder.Entity<Schedule>().ToTable("Schedule");
            modelBuilder.Entity<ServingSchedule>().ToTable("ServingSchedule");

            modelBuilder.Entity<RoomItem>()
                .HasKey(r => new { r.RoomId, r.RoomAttributeId });
            modelBuilder.Entity<ServingSchedule>()
                .HasKey(r => new { r.FoodId, r.ScheduleId });
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted).ConfigureAwait(false);
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync().ConfigureAwait(false);

                _currentTransaction?.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
