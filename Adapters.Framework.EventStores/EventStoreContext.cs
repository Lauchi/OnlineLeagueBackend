﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Adapters.Framework.EventStores
{
    public sealed class EventStoreContext : DbContext
    {
        public DbSet<EntityStream> EntityStreams { get; set; }
        public DbSet<TypeStream> TypeStreams { get; set; }

        public EventStoreContext(DbContextOptions<EventStoreContext> options) :
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EntityStream>()
                .Property(b => b.Version)
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<TypeStream>()
                .Property(b => b.Version)
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }

    public class DomainEventDbo
    {
        public Guid Id { get; set; }
        public string Payload { get; set; }
    }

    public class EntityStream
    {
        [Key]
        public Guid EntityId { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }
        public ICollection<DomainEventDbo> DomainEvents { get; set; }
    }

    public class TypeStream
    {
        [Key]
        public string DomainEventType { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }
        public ICollection<DomainEventDbo> DomainEvents { get; set; }
    }
}