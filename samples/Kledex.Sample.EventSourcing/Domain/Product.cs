﻿using System;
using Kledex.Domain;
using Kledex.Samples.EventSourcing.Domain.Events;

namespace Kledex.Samples.EventSourcing.Domain
{
    public class Product : AggregateRoot
    {
        public string Title { get; private set; }
        public ProductStatus Status { get; private set; }

        public Product()
        {            
        }

        public Product(Guid id, string title) : base(id)
        {
            if (string.IsNullOrEmpty(title))
                throw new ApplicationException("Product title is required.");

            // If you want the event to be dispatched to the service bus,
            // use ProductCreatedBusMessage instead of ProductCreated.
            // Remember to update the connection string in the ServiceBusConfiguration
            // section in the appsettings.json file.
            AddAndApplyEvent(new ProductCreated
            {
                AggregateRootId = Id,
                Title = title,
                Status = ProductStatus.Draft
            });
        }

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new ApplicationException("Product title is required.");

            AddAndApplyEvent(new ProductTitleUpdated
            {
                AggregateRootId = Id,
                Title = title
            });
        }

        public void Publish()
        {
            AddAndApplyEvent(new ProductPublished
            {
                AggregateRootId = Id
            });
        }

        public void Delete()
        {
            AddAndApplyEvent(new ProductDeleted
            {
                AggregateRootId = Id
            });
        }

        private void Apply(ProductCreated @event)
        {
            Id = @event.AggregateRootId;
            Title = @event.Title;
            Status = @event.Status;
        }

        private void Apply(ProductCreatedBusMessage @event)
        {
            Id = @event.AggregateRootId;
            Title = @event.Title;
            Status = @event.Status;
        }

        private void Apply(ProductTitleUpdated @event)
        {
            Title = @event.Title;
        }

        private void Apply(ProductPublished @event)
        {
            Status = ProductStatus.Published;
        }

        private void Apply(ProductDeleted @event)
        {
            Status = ProductStatus.Deleted;
        }
    }
}
