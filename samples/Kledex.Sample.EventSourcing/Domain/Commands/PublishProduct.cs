﻿using System;
using Kledex.Domain;

namespace Kledex.Samples.EventSourcing.Domain.Commands
{
    public class PublishProduct : DomainCommand
    {
        public Guid ProductId { get; set; }
    }
}
