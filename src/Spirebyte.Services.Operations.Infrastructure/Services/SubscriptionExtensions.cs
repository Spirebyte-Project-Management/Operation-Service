using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.MessageBrokers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Spirebyte.Services.Operations.Application.Types;
using Spirebyte.Services.Operations.Application.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Spirebyte.Services.Operations.Infrastructure.Services
{
    public static class SubscriptionExtensions
    {
        public static IBusSubscriber SubscribeMessages(this IBusSubscriber subscriber)
        {
            const string path = "messages.json";
            if (!File.Exists(path))
            {
                return subscriber;
            }

            var messages = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(messages))
            {
                return subscriber;
            }

            var servicesMessages = JsonConvert.DeserializeObject<IDictionary<string, ServiceMessages>>(messages);
            if (!servicesMessages.Any())
            {
                return subscriber;
            }

            var commands = new List<Command>();
            var events = new List<TestEvent>();
            var rejectedEvents = new List<Application.Types.RejectedEvent>();
            var assemblyName = new AssemblyName("Sprirebyte.Services.Operations.Api.Messages");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            foreach (var (_, serviceMessages) in servicesMessages)
            {
                var exchange = serviceMessages.Exchange;
                commands.AddRange(BindMessages<Command>(moduleBuilder, exchange, serviceMessages.Commands));
                events.AddRange(BindMessages<TestEvent>(moduleBuilder, exchange, serviceMessages.Events));
                rejectedEvents.AddRange(BindMessages<Application.Types.RejectedEvent>(moduleBuilder, exchange,
                    serviceMessages.RejectedEvents));
            }

            SubscribeCommands(subscriber, commands);
            SubscribeEvents(subscriber, events);
            SubscribeRejectedEvents(subscriber, rejectedEvents);

            return subscriber;
        }

        private static IEnumerable<T> BindMessages<T>(ModuleBuilder moduleBuilder, string exchange,
            IEnumerable<string> messages) where T : class, IMessage, new()
        {
            if (messages is null)
            {
                yield break;
            }

            foreach (var message in messages)
            {
                var type = typeof(T);
                var typeBuilder = moduleBuilder.DefineType(message, TypeAttributes.Public, type);
                var attributeConstructorParams = new[] { typeof(string), typeof(string), typeof(string), typeof(bool) };
                var constructorInfo = typeof(MessageAttribute).GetConstructor(attributeConstructorParams);
                var customAttributeBuilder = new CustomAttributeBuilder(constructorInfo,
                    new object[] { exchange, null, null, true });
                typeBuilder.SetCustomAttribute(customAttributeBuilder);
                var newType = typeBuilder.CreateType();
                var instance = Activator.CreateInstance(newType);

                yield return instance as T;
            }
        }

        private static void SubscribeCommands(IBusSubscriber subscriber, IEnumerable<ICommand> messages)
        {
            if (messages is null)
            {
                return;
            }

            var subscribeMethod = subscriber.GetType().GetMethod(nameof(IBusSubscriber.Subscribe));
            if (subscribeMethod is null)
            {
                return;
            }

            foreach (var message in messages)
            {
                subscribeMethod.MakeGenericMethod(message.GetType()).Invoke(subscriber,
                    new object[] { (Func<IServiceProvider, ICommand, object, Task>)Handle });
            }

            static Task Handle(IServiceProvider sp, ICommand command, object ctx) =>
                sp.GetService<ICommandHandler<ICommand>>().HandleAsync(command);
        }

        private static void SubscribeEvents(IBusSubscriber subscriber, IEnumerable<ITestEvent> messages)
        {
            if (messages is null)
            {
                return;
            }

            var subscribeMethod = subscriber.GetType().GetMethod(nameof(IBusSubscriber.Subscribe));
            if (subscribeMethod is null)
            {
                return;
            }

            foreach (var message in messages)
            {
                subscribeMethod.MakeGenericMethod(message.GetType()).Invoke(subscriber,
                    new object[] { (Func<IServiceProvider, ITestEvent, object, Task>)Handle });
            }

            static Task Handle(IServiceProvider sp, ITestEvent @event, object ctx) =>
                sp.GetService<IEventHandler<ITestEvent>>().HandleAsync(@event);
        }

        private static void SubscribeRejectedEvents(IBusSubscriber subscriber, IEnumerable<IRejectedEvent> messages)
        {
            if (messages is null)
            {
                return;
            }

            var subscribeMethod = subscriber.GetType().GetMethod(nameof(IBusSubscriber.Subscribe));
            if (subscribeMethod is null)
            {
                return;
            }

            foreach (var message in messages)
            {
                subscribeMethod.MakeGenericMethod(message.GetType()).Invoke(subscriber,
                    new object[] { (Func<IServiceProvider, IRejectedEvent, object, Task>)Handle });
            }

            static Task Handle(IServiceProvider sp, IRejectedEvent @event, object ctx) =>
                sp.GetService<IEventHandler<IRejectedEvent>>().HandleAsync(@event);
        }

        private class ServiceMessages
        {
            public string Exchange { get; set; }
            public IEnumerable<string> Commands { get; set; }
            public IEnumerable<string> Events { get; set; }
            public IEnumerable<string> RejectedEvents { get; set; }
        }
    }
}
