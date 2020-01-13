using System;
using Microsoft.EntityFrameworkCore;
using WaterMangoApp.Model.QuartzModels;

namespace WaterMangoApp.Data
{
    public class QuartzDbContext : DbContext
    {
        public QuartzDbContext(DbContextOptions<QuartzDbContext> options) : base(options)
        {
            
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<QRTZ_BLOB_TRIGGERS>(entity =>
            {
                entity.HasKey(e => new { e.SCHED_NAME, e.TRIGGER_NAME, e.TRIGGER_GROUP });

                entity.Property(e => e.SCHED_NAME).HasMaxLength(120);

                entity.Property(e => e.TRIGGER_NAME).HasMaxLength(150);

                entity.Property(e => e.TRIGGER_GROUP).HasMaxLength(150);
            });

            modelBuilder.Entity<QRTZ_CALENDARS>(entity =>
            {
                entity.HasKey(e => new { e.SCHED_NAME, e.CALENDAR_NAME });

                entity.Property(e => e.SCHED_NAME).HasMaxLength(120);

                entity.Property(e => e.CALENDAR_NAME).HasMaxLength(200);

                entity.Property(e => e.CALENDAR).IsRequired();
            });

            modelBuilder.Entity<QRTZ_CRON_TRIGGERS>(entity =>
            {
                entity.HasKey(e => new { e.SCHED_NAME, e.TRIGGER_NAME, e.TRIGGER_GROUP });

                entity.Property(e => e.SCHED_NAME).HasMaxLength(120);

                entity.Property(e => e.TRIGGER_NAME).HasMaxLength(150);

                entity.Property(e => e.TRIGGER_GROUP).HasMaxLength(150);

                entity.Property(e => e.CRON_EXPRESSION)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(e => e.TIME_ZONE_ID).HasMaxLength(80);

                entity.HasOne(d => d.QRTZ_TRIGGERS)
                    .WithOne(p => p.QRTZ_CRON_TRIGGERS)
                    .HasForeignKey<QRTZ_CRON_TRIGGERS>(d => new { d.SCHED_NAME, d.TRIGGER_NAME, d.TRIGGER_GROUP })
                    .HasConstraintName("FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QRTZ_FIRED_TRIGGERS>(entity =>
            {
                entity.HasKey(e => new { e.SCHED_NAME, e.ENTRY_ID });

                entity.HasIndex(e => new { e.SCHED_NAME, e.INSTANCE_NAME })
                    .HasName("IDX_QRTZ_FT_TRIG_INST_NAME");

                entity.HasIndex(e => new { e.SCHED_NAME, e.JOB_GROUP })
                    .HasName("IDX_QRTZ_FT_JG");

                entity.HasIndex(e => new { e.SCHED_NAME, e.TRIGGER_GROUP })
                    .HasName("IDX_QRTZ_FT_TG");

                entity.HasIndex(e => new { e.SCHED_NAME, e.INSTANCE_NAME, e.REQUESTS_RECOVERY })
                    .HasName("IDX_QRTZ_FT_INST_JOB_REQ_RCVRY");

                entity.HasIndex(e => new { e.SCHED_NAME, e.JOB_NAME, e.JOB_GROUP })
                    .HasName("IDX_QRTZ_FT_J_G");

                entity.HasIndex(e => new { e.SCHED_NAME, e.TRIGGER_NAME, e.TRIGGER_GROUP })
                    .HasName("IDX_QRTZ_FT_T_G");

                entity.Property(e => e.SCHED_NAME).HasMaxLength(120);

                entity.Property(e => e.ENTRY_ID).HasMaxLength(140);

                entity.Property(e => e.INSTANCE_NAME)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.JOB_GROUP).HasMaxLength(150);

                entity.Property(e => e.JOB_NAME).HasMaxLength(150);

                entity.Property(e => e.STATE)
                    .IsRequired()
                    .HasMaxLength(16);

                entity.Property(e => e.TRIGGER_GROUP)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.TRIGGER_NAME)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<QRTZ_JOB_DETAILS>(entity =>
            {
                entity.HasKey(e => new { e.SCHED_NAME, e.JOB_NAME, e.JOB_GROUP });

                entity.Property(e => e.SCHED_NAME).HasMaxLength(120);

                entity.Property(e => e.JOB_NAME).HasMaxLength(150);

                entity.Property(e => e.JOB_GROUP).HasMaxLength(150);

                entity.Property(e => e.DESCRIPTION).HasMaxLength(250);

                entity.Property(e => e.JOB_CLASS_NAME)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<QRTZ_LOCKS>(entity =>
            {
                entity.HasKey(e => new { e.SCHED_NAME, e.LOCK_NAME });

                entity.Property(e => e.SCHED_NAME).HasMaxLength(120);

                entity.Property(e => e.LOCK_NAME).HasMaxLength(40);
            });

            modelBuilder.Entity<QRTZ_PAUSED_TRIGGER_GRPS>(entity =>
            {
                entity.HasKey(e => new { e.SCHED_NAME, e.TRIGGER_GROUP });

                entity.Property(e => e.SCHED_NAME).HasMaxLength(120);

                entity.Property(e => e.TRIGGER_GROUP).HasMaxLength(150);
            });

            modelBuilder.Entity<QRTZ_SCHEDULER_STATE>(entity =>
            {
                entity.HasKey(e => new { e.SCHED_NAME, e.INSTANCE_NAME });

                entity.Property(e => e.SCHED_NAME).HasMaxLength(120);

                entity.Property(e => e.INSTANCE_NAME).HasMaxLength(200);
            });

            modelBuilder.Entity<QRTZ_SIMPLE_TRIGGERS>(entity =>
            {
                entity.HasKey(e => new { e.SCHED_NAME, e.TRIGGER_NAME, e.TRIGGER_GROUP });

                entity.Property(e => e.SCHED_NAME).HasMaxLength(120);

                entity.Property(e => e.TRIGGER_NAME).HasMaxLength(150);

                entity.Property(e => e.TRIGGER_GROUP).HasMaxLength(150);

                entity.HasOne(d => d.QRTZ_TRIGGERS)
                    .WithOne(p => p.QRTZ_SIMPLE_TRIGGERS)
                    .HasForeignKey<QRTZ_SIMPLE_TRIGGERS>(d => new { d.SCHED_NAME, d.TRIGGER_NAME, d.TRIGGER_GROUP })
                    .HasConstraintName("FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QRTZ_SIMPROP_TRIGGERS>(entity =>
            {
                entity.HasKey(e => new { e.SCHED_NAME, e.TRIGGER_NAME, e.TRIGGER_GROUP });

                entity.Property(e => e.SCHED_NAME).HasMaxLength(120);

                entity.Property(e => e.TRIGGER_NAME).HasMaxLength(150);

                entity.Property(e => e.TRIGGER_GROUP).HasMaxLength(150);

                entity.Property(e => e.DEC_PROP_1).HasColumnType("numeric(13, 4)");

                entity.Property(e => e.DEC_PROP_2).HasColumnType("numeric(13, 4)");

                entity.Property(e => e.STR_PROP_1).HasMaxLength(512);

                entity.Property(e => e.STR_PROP_2).HasMaxLength(512);

                entity.Property(e => e.STR_PROP_3).HasMaxLength(512);

                entity.Property(e => e.TIME_ZONE_ID).HasMaxLength(80);

                entity.HasOne(d => d.QRTZ_TRIGGERS)
                    .WithOne(p => p.QRTZ_SIMPROP_TRIGGERS)
                    .HasForeignKey<QRTZ_SIMPROP_TRIGGERS>(d => new { d.SCHED_NAME, d.TRIGGER_NAME, d.TRIGGER_GROUP })
                    .HasConstraintName("FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QRTZ_TRIGGERS>(entity =>
            {
                entity.HasKey(e => new { e.SCHED_NAME, e.TRIGGER_NAME, e.TRIGGER_GROUP });

                entity.HasIndex(e => new { e.SCHED_NAME, e.CALENDAR_NAME })
                    .HasName("IDX_QRTZ_T_C");

                entity.HasIndex(e => new { e.SCHED_NAME, e.JOB_GROUP })
                    .HasName("IDX_QRTZ_T_JG");

                entity.HasIndex(e => new { e.SCHED_NAME, e.NEXT_FIRE_TIME })
                    .HasName("IDX_QRTZ_T_NEXT_FIRE_TIME");

                entity.HasIndex(e => new { e.SCHED_NAME, e.TRIGGER_GROUP })
                    .HasName("IDX_QRTZ_T_G");

                entity.HasIndex(e => new { e.SCHED_NAME, e.TRIGGER_STATE })
                    .HasName("IDX_QRTZ_T_STATE");

                entity.HasIndex(e => new { e.SCHED_NAME, e.JOB_NAME, e.JOB_GROUP })
                    .HasName("IDX_QRTZ_T_J");

                entity.HasIndex(e => new { e.SCHED_NAME, e.MISFIRE_INSTR, e.NEXT_FIRE_TIME })
                    .HasName("IDX_QRTZ_T_NFT_MISFIRE");

                entity.HasIndex(e => new { e.SCHED_NAME, e.TRIGGER_GROUP, e.TRIGGER_STATE })
                    .HasName("IDX_QRTZ_T_N_G_STATE");

                entity.HasIndex(e => new { e.SCHED_NAME, e.TRIGGER_STATE, e.NEXT_FIRE_TIME })
                    .HasName("IDX_QRTZ_T_NFT_ST");

                entity.HasIndex(e => new { e.SCHED_NAME, e.MISFIRE_INSTR, e.NEXT_FIRE_TIME, e.TRIGGER_STATE })
                    .HasName("IDX_QRTZ_T_NFT_ST_MISFIRE");

                entity.HasIndex(e => new { e.SCHED_NAME, e.TRIGGER_NAME, e.TRIGGER_GROUP, e.TRIGGER_STATE })
                    .HasName("IDX_QRTZ_T_N_STATE");

                entity.HasIndex(e => new { e.SCHED_NAME, e.MISFIRE_INSTR, e.NEXT_FIRE_TIME, e.TRIGGER_GROUP, e.TRIGGER_STATE })
                    .HasName("IDX_QRTZ_T_NFT_ST_MISFIRE_GRP");

                entity.Property(e => e.SCHED_NAME).HasMaxLength(120);

                entity.Property(e => e.TRIGGER_NAME).HasMaxLength(150);

                entity.Property(e => e.TRIGGER_GROUP).HasMaxLength(150);

                entity.Property(e => e.CALENDAR_NAME).HasMaxLength(200);

                entity.Property(e => e.DESCRIPTION).HasMaxLength(250);

                entity.Property(e => e.JOB_GROUP)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.JOB_NAME)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.TRIGGER_STATE)
                    .IsRequired()
                    .HasMaxLength(16);

                entity.Property(e => e.TRIGGER_TYPE)
                    .IsRequired()
                    .HasMaxLength(8);

                entity.HasOne(d => d.QRTZ_JOB_DETAILS)
                    .WithMany(p => p.QRTZ_TRIGGERS)
                    .HasForeignKey(d => new { d.SCHED_NAME, d.JOB_NAME, d.JOB_GROUP })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS");
            });
        }
        

        public virtual DbSet<QRTZ_BLOB_TRIGGERS> QRTZ_BLOB_TRIGGERS { get; set; }
        public virtual DbSet<QRTZ_CALENDARS> QRTZ_CALENDARS { get; set; }
        public virtual DbSet<QRTZ_CRON_TRIGGERS> QRTZ_CRON_TRIGGERS { get; set; }
        public virtual DbSet<QRTZ_FIRED_TRIGGERS> QRTZ_FIRED_TRIGGERS { get; set; }
        public virtual DbSet<QRTZ_JOB_DETAILS> QRTZ_JOB_DETAILS { get; set; }
        public virtual DbSet<QRTZ_LOCKS> QRTZ_LOCKS { get; set; }
        public virtual DbSet<QRTZ_PAUSED_TRIGGER_GRPS> QRTZ_PAUSED_TRIGGER_GRPS { get; set; }
        public virtual DbSet<QRTZ_SCHEDULER_STATE> QRTZ_SCHEDULER_STATE { get; set; }
        public virtual DbSet<QRTZ_SIMPLE_TRIGGERS> QRTZ_SIMPLE_TRIGGERS { get; set; }
        public virtual DbSet<QRTZ_SIMPROP_TRIGGERS> QRTZ_SIMPROP_TRIGGERS { get; set; }
        public virtual DbSet<QRTZ_TRIGGERS> QRTZ_TRIGGERS { get; set; }

    }
}