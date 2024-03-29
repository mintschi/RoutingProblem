﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RoutingProblem.Services.Data;

namespace RoutingProblem.Models
{
    public partial class GraphContext : DbContext
    {
        public GraphContext()
        {
        }

        public GraphContext(DbContextOptions<GraphContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Edge> Edge { get; set; }
        public virtual DbSet<Node> Node { get; set; }
        public virtual DbSet<DisabledMovement> DisabledMovement { get; set; }
        public virtual DbSet<Data> Data { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=mainpc\\sqlexpress;Database=DopravnaSiet;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Data>(entity =>
            {
                entity.HasKey(e => e.IdData)
                    .HasName("PK__Data__95283FF09EDA09DA");

                entity.Property(e => e.IdData)
                    .HasColumnName("id_data")
                    .HasColumnType("decimal(15, 0)");

                entity.Property(e => e.Title)
                    .HasColumnName("title");

                entity.Property(e => e.MinLat)
                    .HasColumnName("min_lat");

                entity.Property(e => e.MinLon)
                    .HasColumnName("min_lon");

                entity.Property(e => e.MaxLat)
                    .HasColumnName("max_lat");

                entity.Property(e => e.MaxLon)
                    .HasColumnName("max_lon");

                entity.Property(e => e.Active)
                    .HasColumnName("active");
            });

            modelBuilder.Entity<Data>()
            .ForSqlServerIsMemoryOptimized();

            modelBuilder.Entity<Edge>(entity =>
            {
                entity.HasKey(e => e.IdEdge)
                    .HasName("PK__Edge__FB2CA857272772F6");

                entity.Property(e => e.IdEdge)
                    .HasColumnName("id_edge")
                    .HasColumnType("decimal(15, 0)");

                entity.Property(e => e.IdData)
                    .HasColumnName("id_data")
                    .HasColumnType("decimal(15, 0)");

                entity.Property(e => e.EndNode)
                    .HasColumnName("end_node")
                    .HasColumnType("decimal(15, 0)");

                entity.Property(e => e.StartNode)
                    .HasColumnName("start_node")
                    .HasColumnType("decimal(15, 0)");

                entity.Property(e => e.DistanceInMeters)
                    .HasColumnName("distance_in_meters")
                    .HasColumnType("float");

                entity.HasOne(d => d.IdDataNavigation)
                    .WithMany(p => p.EdgeIdDataNavigation)
                    .HasForeignKey(d => d.IdData)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Edge__id_data__6CA31EA0");

                entity.HasOne(d => d.EndNodeNavigation)
                    .WithMany(p => p.EdgeEndNodeNavigation)
                    .HasForeignKey(d => d.EndNode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Edge__end_node__4F7CD00D");

                entity.HasOne(d => d.StartNodeNavigation)
                    .WithMany(p => p.EdgeStartNodeNavigation)
                    .HasForeignKey(d => d.StartNode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Edge__start_node__4E88ABD4");
            });

            modelBuilder.Entity<Edge>()
            .ForSqlServerIsMemoryOptimized();

            modelBuilder.Entity<Node>(entity =>
            {
                entity.HasKey(e => e.IdNode)
                    .HasName("PK__Node__26989F9CAD78BCF3");

                entity.Property(e => e.IdNode)
                    .HasColumnName("id_node")
                    .HasColumnType("decimal(15, 0)");

                entity.Property(e => e.IdData)
                    .HasColumnName("id_data")
                    .HasColumnType("decimal(15, 0)");

                entity.Property(e => e.Lat).HasColumnName("lat");

                entity.Property(e => e.Lon).HasColumnName("lon");

                entity.HasOne(d => d.IdDataNavigation)
                    .WithMany(p => p.NodeIdDataNavigation)
                    .HasForeignKey(d => d.IdData)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Node__id_data__6BAEFA67");
            });

            modelBuilder.Entity<Node>()
            .ForSqlServerIsMemoryOptimized();

            modelBuilder.Entity<DisabledMovement>(entity =>
            {
                entity.HasKey(e => e.IdDisabledMovement)
                    .HasName("PK__Disabled__CD0366550AB8EF84");

                entity.Property(e => e.IdDisabledMovement)
                    .HasColumnName("id_disabled_movement")
                    .HasColumnType("decimal(15, 0)");

                entity.Property(e => e.IdData)
                    .HasColumnName("id_data")
                    .HasColumnType("decimal(15, 0)");

                entity.Property(e => e.StartEdge)
                    .HasColumnName("start_edge")
                    .HasColumnType("decimal(15, 0)");

                entity.Property(e => e.EndEdge)
                    .HasColumnName("end_edge")
                    .HasColumnType("decimal(15, 0)");

                entity.HasOne(d => d.IdDataNavigation)
                    .WithMany(p => p.DisabledMovementIdDataNavigation)
                    .HasForeignKey(d => d.IdData)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DisabledM__id_da__6D9742D9");

                entity.HasOne(d => d.StartEdgeNavigation)
                    .WithMany(p => p.DisabledMovementStartEdgeNavigation)
                    .HasForeignKey(d => d.StartEdge)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DisabledM__start__08B54D69");

                entity.HasOne(d => d.EndEdgeNavigation)
                    .WithMany(p => p.DisabledMovementEndEdgeNavigation)
                    .HasForeignKey(d => d.EndEdge)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DisabledM__end_e__09A971A2");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
