﻿using System.Threading.Tasks;
using Kledex.Bus;
using Kledex.Commands;
using Kledex.Domain;
using Kledex.Events;
using Kledex.Queries;

namespace Kledex
{
    /// <inheritdoc />
    /// <summary>
    /// Dispatcher
    /// </summary>
    /// <seealso cref="T:Kledex.IDispatcher" />
    public class Dispatcher : IDispatcher
    {
        private readonly ICommandSender _commandSender;
        private readonly IDomainCommandSender _domainCommandSender;
        private readonly IEventPublisher _eventPublisher;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IBusMessageDispatcher _busMessageDispatcher;
        private readonly ITransactionService _transactionService;

        public Dispatcher(ICommandSender commandSender,
            IDomainCommandSender domainCommandSender,
            IEventPublisher eventPublisher, 
            IQueryProcessor queryProcessor, 
            IBusMessageDispatcher busMessageDispatcher,
            ITransactionService transactionService)
        {
            _commandSender = commandSender;
            _domainCommandSender = domainCommandSender;
            _eventPublisher = eventPublisher;
            _queryProcessor = queryProcessor;
            _busMessageDispatcher = busMessageDispatcher;
            _transactionService = transactionService;
        }

        /// <inheritdoc />
        public Task SendAsync<TCommand>(TCommand command) 
            where TCommand : ICommand
        {
            return _transactionService.ProcessAsync((IDomainCommand<IAggregateRoot>)command);
            //return command is IDomainCommand 
            //    ? _domainCommandSender.SendAsync((IDomainCommand<IAggregateRoot>)command) 
            //    : _commandSender.SendAsync(command);
        }

        /// <inheritdoc />
        public Task PublishAsync<TEvent>(TEvent @event) 
            where TEvent : IEvent
        {
            return _eventPublisher.PublishAsync(@event);
        }

        /// <inheritdoc />
        public Task<TResult> GetResultAsync<TResult>(IQuery<TResult> query)
        {
            return _queryProcessor.ProcessAsync(query);
        }

        /// <inheritdoc />
        public Task DispatchBusMessageAsync<TMessage>(TMessage message) 
            where TMessage : IBusMessage
        {
            return _busMessageDispatcher.DispatchAsync(message);
        }

        /// <inheritdoc />
        public void Send<TCommand>(TCommand command) 
            where TCommand : ICommand
        {
            if (command is IDomainCommand)
                _domainCommandSender.Send((IDomainCommand<IAggregateRoot>)command);
            else
                _commandSender.Send(command);
        }

        /// <inheritdoc />
        public void Publish<TEvent>(TEvent @event) 
            where TEvent : IEvent
        {
            _eventPublisher.Publish(@event);
        }

        /// <inheritdoc />
        public TResult GetResult<TResult>(IQuery<TResult> query)
        {
            return _queryProcessor.Process(query);
        }
    }
}