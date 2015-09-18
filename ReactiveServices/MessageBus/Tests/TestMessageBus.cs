using System.Configuration;
using System.Reflection;
using ReactiveServices.MessageBus.Tests.UnitTests;

namespace ReactiveServices.MessageBus.Tests
{
    public static class TestMessageBus
    {
        public static IPublishingBus BuildPublishingBus()
        {
            var publishingBusImplementationAssemblyName = ConfigurationManager.AppSettings["PublishingBusImplementationAssemblyName"];
            var publishingBusImplementationTypeName = ConfigurationManager.AppSettings["PublishingBusImplementationTypeName"];
            var publishingBusImplementationAssembly = Assembly.LoadFrom(publishingBusImplementationAssemblyName);
            var publishingBusImplementationType = publishingBusImplementationAssembly.GetType(publishingBusImplementationTypeName);

            return new PublishingBusBuilder()
                .WithConnectionString(MessageBusTests.ConnectionString)
                .WithImplementation(publishingBusImplementationType)
                .WithPublishConfirms()
                .Build();
        }

        public static ISubscriptionBus BuildSubscriptionBus()
        {
            var subscriptionBusImplementationAssemblyName = ConfigurationManager.AppSettings["SubscriptionBusImplementationAssemblyName"];
            var subscriptionBusImplementationTypeName = ConfigurationManager.AppSettings["SubscriptionBusImplementationTypeName"];
            var subscriptionBusImplementationAssembly = Assembly.LoadFrom(subscriptionBusImplementationAssemblyName);
            var subscriptionBusImplementationType = subscriptionBusImplementationAssembly.GetType(subscriptionBusImplementationTypeName);

            return new SubscriptionBusBuilder()
                .WithConnectionString(MessageBusTests.ConnectionString)
                .WithImplementation(subscriptionBusImplementationType)
                .Build();
        }
    }
}
