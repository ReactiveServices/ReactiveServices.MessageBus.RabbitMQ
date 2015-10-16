using System;
using NUnit.Framework;
using FluentAssertions;
using ReactiveServices.Configuration;
using ReactiveServices.MessageBus.RabbitMQ.Tests.Specifications;
using System.Diagnostics;
using System.Threading;
using ReactiveServices.Configuration.ConfigurationFiles;
using ReactiveServices.Extensions;
using Renci.SshNet;

namespace ReactiveServices.MessageBus.RabbitMQ.Tests.UnitTests
{
    [TestFixture]
    public class MessageBusTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            DependencyResolver.Reset();
            DependencyResolver.Initialize();
        }

        [Test]
        [Category("stable")]
        [Category("fast")]
        public void TestMessageBusInitialization()
        {
            var bus = DependencyResolver.Get<ISubscriptionBus>();
            try
            {
                bus.Should().NotBeNull();
                var subscriptionId = SubscriptionId.FromString("TestMessageBusInitialization");
                bus.SubscribeTo<AMessage>(subscriptionId, m => { });
                bus.IsListenningTo(typeof(AMessage), subscriptionId).Should().BeTrue();
                DeleteQueues();
            }
            finally
            {
                bus.Dispose();
            }
        }

        [Test]
        [Category("stable")]
        [Category("fast")]
        public void TestSendingOneMessageToOneSubscriberFollowingToOneMessageToTwoSubscribers()
        {
            using (var executor = new StepDefinitionsExecutor())
            {
                executor.InitializeMessageBus();

                PublishOneMessageToOneSubscriber(executor);
                AssertOnlyOneMessageWasReceived(executor);

                PublishOneMessageToTwoSubscribers(executor);
                AssertTheSameMessageWasReceivedByBothSubscribers(executor);
            }
        }

        private static void PublishOneMessageToOneSubscriber(StepDefinitionsExecutor executor)
        {
            executor.ResetInMemoryTestData();

            executor.PerformSubscription<EventOccurred>(
                executor.MessagesReceivedBySubscriber1,
                executor.MessageHandlerForSubscriber1,
                "DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurred",
                null
            );

            executor.PerformPublication<EventOccurred>();

            //Dá algum tempo para que as filas recebam as mensagens, antes de remover a inscrição
            executor.Wait(3);

            executor.RemoveAllSubscriptions();
        }

        private static void PublishOneMessageToTwoSubscribers(StepDefinitionsExecutor executor)
        {
            executor.ResetInMemoryTestData();

            executor.PerformSubscription<EventOccurred>(
                executor.MessagesReceivedBySubscriber1,
                executor.MessageHandlerForSubscriber1,
                "DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurred",
                null
            );
            executor.PerformSubscription<EventOccurred>(
                executor.MessagesReceivedBySubscriber2,
                executor.MessageHandlerForSubscriber2,
                "DadoQueOutroAssinanteSeInscrevaParaReceberAMensagemEventOccurred",
                null
            );

            executor.PerformPublication<EventOccurred>();

            //Dá algum tempo para que as filas recebam as mensagens, antes de remover a inscrição
            executor.Wait(3);

            executor.RemoveAllSubscriptions();
        }

        private static void AssertOnlyOneMessageWasReceived(StepDefinitionsExecutor executor)
        {
            executor.MessagesReceivedBySubscriber1.Should().NotBeEmpty();
            executor.MessagesReceivedBySubscriber1.Should().ContainSingle(m => m is EventOccurred);
        }

        private static void AssertTheSameMessageWasReceivedByBothSubscribers(StepDefinitionsExecutor executor)
        {
            executor.MessagesReceivedBySubscriber1.Should().NotBeEmpty();
            executor.MessagesReceivedBySubscriber1.Should().ContainSingle(m => m is EventOccurred);
            executor.MessagesReceivedBySubscriber2.Should().NotBeEmpty();
            executor.MessagesReceivedBySubscriber2.Should().ContainSingle(m => m is EventOccurred);
        }

        [Test]
        [Category("stable")]
        [Category("fast")]
        public void TestPublishToTopicWhenHandlingMessageFromSameTopic()
        {
            var ack = false;
            var firstMessage = true;
            using (var subscriptionBus = DependencyResolver.Get<ISubscriptionBus>())
            {
                var subscriptionId = SubscriptionId.FromString("SubscriptionId");
                var topicId = TopicId.FromString("TopicId");

                subscriptionBus.SubscribeTo<EventOccurred>(topicId,
                    subscriptionId,
                    m =>
                    {
                        if (firstMessage)
                        {
                            firstMessage = false;
                            using (var publishBus = DependencyResolver.Get<IPublishingBus>())
                            {
                                var message = new EventOccurred();
                                //Publish other message in the same topic
                                publishBus.Publish(topicId, message, StorageType.NonPersistent);
                            }
                        }
                        else
                        {
                            ack = true;
                        }
                    }, SubscriptionMode.Exclusive);

                using (var publishBus = DependencyResolver.Get<IPublishingBus>())
                {
                    var message = new EventOccurred();
                    publishBus.Publish(topicId, message, StorageType.NonPersistent);
                }

                Thread.Sleep(3000);
            }

            ack.Should().BeTrue();
        }

        /// <summary>
        /// Este teste deve ser usado como referência de uso para o MessageBus
        /// </summary>
        [Test]
        [Category("stable")]
        [Category("fast")]
        public void TestMessageBusUsageExample()
        {
            var ack = false;
            /*
             * Cria um MessageBus para fazer as assinaturas
             * Este MessageBus deve ficar aberto enquanto se quiser que as mensagens assinadas sejam recebidas
             * Uma vez que ele seja fechado, as mensagens não serão mais recebidas
             * Se possível use sempre a cláusula using para instanciar um MessageBus, 
             * caso contrário você precisará liberar a conexão chamando o método Dispose
             * quando o MessageBus não precisar mais ser usado.
             */
            using (var subscriptionBus = DependencyResolver.Get<ISubscriptionBus>())
            {
                // Gerar id de inscrição para identificar uma fila de recebimento de mensagens
                var subscriptionId = SubscriptionId.FromString("TestMessageBusUsageExample");

                /* 
                 * Cria inscrição para receber mensagens do tipo EventOccurred
                 * Um TopicId pode ser informado também caso se queira filtrar as mensagens
                 */
                subscriptionBus.SubscribeTo<EventOccurred>(
                    /*
                     * Informa o id da inscrição
                     * Caso mais de uma inscrição seja feita com o mesmo id, as mensagens serão entregues
                     * através de RoundRobin, ou seja, cada mensagem será entregue apenas a uma das inscrições.
                     * Caso não deseje o comportamento de RoundRobin, use subscriptionIds diferentes.
                     */
                    subscriptionId,
                    /*
                     * Informa o callcack a ser executado quando a mensagem for recebida
                     * Lembre-se que o MessageBus precisa ainda estar conectado para que o callback seja executado
                     * O MessageBus fica conectado até que seu método Dispose seja executado
                     * O método Dispose é executado automaticamente ao final da cláusula using
                     */
                    m =>
                    {
                        ack = true;
                    },
                    /*
                     * Informa que a fila criada para a inscrição será de uso exclusivo. Isso implica em tal fila ser excluída no Dispose do SubscriptionBus
                     */
                    SubscriptionMode.Exclusive
                );

                /* 
                 * Instancia MessageBus para envio de mensagens
                 * Deve sempre ser criado usando uma cláusula using
                 */
                using (var publishBus = DependencyResolver.Get<IPublishingBus>())
                {
                    /*
                     * Cria mensagem com um id aleatório
                     * Caso necessário o id pode ser passado como parâmetro
                     */
                    var message = new EventOccurred();
                    /*
                     * Publica a mensagem
                     * Caso o MessageBus tenha sido criado WithPublishConfirms, 
                     * a execução desse método será sincrona, e haverá uma excessão 
                     * caso a mensagem não possa ser publicada
                     * caso contrário, 
                     * a execução desse método será assincrona e não haverá qualquer confirmação 
                     * de que a mensagem foi publicada
                     */
                    publishBus.Publish(message, StorageType.NonPersistent);
                }

                /* 
                 * Aguarda um segundo antes de fechar o MessageBus de recebimento, 
                 * caso contrário a mensagem pode não ter tempo de ser entregue
                 */
                Thread.Sleep(1000);
            }

            // Confirma que a mensagem foi recebida pelo callcack inscrito
            ack.Should().BeTrue();
        }

        [Test]
        [TestCase("WithPublishConfirms", "Generics", StorageType.Persistent, SubscriptionMode.Shared)]
        [TestCase("", "Generics", StorageType.Persistent, SubscriptionMode.Shared)]
        [TestCase("WithPublishConfirms", "Generics", StorageType.NonPersistent, SubscriptionMode.Shared)]
        [TestCase("", "Generics", StorageType.NonPersistent, SubscriptionMode.Shared)]
        [TestCase("WithPublishConfirms", "Generics", StorageType.Persistent, SubscriptionMode.Exclusive)]
        [TestCase("", "Generics", StorageType.Persistent, SubscriptionMode.Exclusive)]
        [TestCase("WithPublishConfirms", "Generics", StorageType.NonPersistent, SubscriptionMode.Exclusive)]
        [TestCase("", "Generics", StorageType.NonPersistent, SubscriptionMode.Exclusive)]
        [Category("load")]
        [Category("unstable")]
        [Category("slow")]
        public void TestMessageTransferRate(string publishConfirmationMode, string messageTypeIdentificationMode, StorageType storageType, SubscriptionMode subscriptionMode)
        {
            DeleteQueues();

            var requirePublishConfirmation = publishConfirmationMode == "WithPublishConfirms";

            const int cicles = 60;
            const int runtime = cicles * 1000;
            var receivedMessageCount = 0;
            var publishedMessageCount = 0;

            using (var subscriptionBus = DependencyResolver.Get<ISubscriptionBus>())
            {
                switch (messageTypeIdentificationMode)
                {
                    case "Generics":
                        subscriptionBus.SubscribeTo<EventOccurred>(
                            SubscriptionId.FromString("TestMessageTransferRate"),
                            m =>
                            {
                                receivedMessageCount++;
                            },
                            subscriptionMode
                        );
                        break;
                    case "NonGenerics":
                        subscriptionBus.SubscribeTo(
                            typeof(EventOccurred),
                            SubscriptionId.FromString("TestMessageTransferRate"),
                            m =>
                            {
                                receivedMessageCount++;
                            },
                            subscriptionMode
                        );
                        break;
                }

                using (var publishBus = DependencyResolver.Get<IPublishingBus>())
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    while (stopwatch.ElapsedMilliseconds < runtime)
                    {
                        var message = new EventOccurred();

                        switch (messageTypeIdentificationMode)
                        {
                            case "Generics":
                                publishBus.Publish(message, storageType, requirePublishConfirmation);
                                break;
                            case "NonGenerics":
                                publishBus.Publish(typeof(EventOccurred), message, storageType, requirePublishConfirmation);
                                break;
                        }

                        publishedMessageCount++;
                    }
                    stopwatch.Stop();
                }

                Thread.Sleep(1000);
            }

            Console.WriteLine();
            Console.WriteLine("Published: {0}/s, Received: {1}/s, Runtime: {2}", publishedMessageCount / cicles, receivedMessageCount / cicles, runtime);
            Console.WriteLine();

            DeleteQueues();

            if (requirePublishConfirmation)
            {
                receivedMessageCount.Should().Be(publishedMessageCount);
            }
        }



        [Test]
        [Category("unstable")]
        [Category("fast")]
        public void TestDisposeAfterPublish()
        {
            DeleteQueues();
            try
            {
                var publishingBus = DependencyResolver.Get<IPublishingBus>();

                publishingBus.Publish(new AMessage(), StorageType.NonPersistent);

                try
                {
                    publishingBus.Dispose();
                }
                catch
                {
                    Assert.Fail("Exception disposing PublishingBus just after Publish!");
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        [Test]
        [Category("unstable")]
        [Category("fast")]
        public void TestDisposeAfterSend()
        {
            DeleteQueues();
            try
            {
                var sendingBus = DependencyResolver.Get<ISendingBus>();

                var subscriptionId = SubscriptionId.FromString("TestAutoConnectBetweenSends");

                sendingBus.Send(new AMessage(), subscriptionId, StorageType.NonPersistent);

                try
                {
                    sendingBus.Dispose();
                }
                catch
                {
                    Assert.Fail("Exception disposing SendingBus just after Publish!");
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        [Test]
        [Category("unstable")]
        [Category("fast")]
        public void TestDisposeAfterSubscribeTo()
        {
            DeleteQueues();
            try
            {
                var subscriptionBus = DependencyResolver.Get<ISubscriptionBus>();

                var subscriptionId = SubscriptionId.FromString("TestAutoConnectAfterSubscribeTo");

                subscriptionBus.SubscribeTo<AMessage>(subscriptionId, m => { });

                try
                {
                    subscriptionBus.Dispose();
                }
                catch
                {
                    Assert.Fail("Exception disposing SubscriptionBus just after Publish!");
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        [Test]
        [Category("unstable")]
        [Category("fast")]
        public void TestDisposeAfterReceive()
        {
            DeleteQueues();
            try
            {
                var receivingBus = DependencyResolver.Get<IReceivingBus>();

                var subscriptionId = SubscriptionId.FromString("TestDisposeAfterReceive");

                receivingBus.Receive<AMessage>(subscriptionId, m => { });

                try
                {
                    receivingBus.Dispose();
                }
                catch
                {
                    Assert.Fail("Exception disposing ReceivingBus just after Publish!");
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        [Test]
        [Category("unstable")]
        [Category("slow")]
        public void TestDisposeAfterRequest()
        {
            DeleteQueues();
            try
            {
                var requestBus = DependencyResolver.Get<IRequestBus>();

                try
                {
                    requestBus.Request<RequestX, ResponseX>(new RequestX(RequesterId.New(), RequestId.New()));
                }
                catch (TimeoutException)
                {
                }
                catch (Exception)
                {
                    Assert.Fail("Request should be published and timeout!");
                }

                try
                {
                    requestBus.Dispose();
                }
                catch
                {
                    Assert.Fail("Exception disposing RequestBus just after Publish!");
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        [Test]
        [Category("unstable")]
        [Category("fast")]
        public void TestDisposeAfterStartRespondingTo()
        {
            DeleteQueues();
            try
            {
                var responseBus = DependencyResolver.Get<IResponseBus>();

                responseBus.StartRespondingTo<RequestX, ResponseX>(r => new ResponseX(ResponderId.New(), ResponseId.New(), r));

                try
                {
                    responseBus.Dispose();
                }
                catch
                {
                    Assert.Fail("Exception disposing ResponseBus just after Publish!");
                }
            }
            finally
            {
                DeleteQueues();
            }
        }



        [Test]
        [Category("unstable")]
        [Category("fast")]
        public void TestDisposeWhileHandlingMessageOnSubscribeTo()
        {
            DeleteQueues();
            try
            {
                var publishedMessage = new AMessage();

                using (var publishingBus = DependencyResolver.Get<IPublishingBus>())
                {
                    var subscriptionBus = DependencyResolver.Get<ISubscriptionBus>();

                    var subscriptionId = SubscriptionId.FromString("TestDisposeWhileHandlingMessageOnSubscribeTo");

                    var messageReceived = false;

                    subscriptionBus.SubscribeTo<AMessage>(subscriptionId, m =>
                    {
                        messageReceived = true;
                        Thread.Sleep(10000); // It will take 10 seconds, but the SubscriptionBus will be disposed after just 1 second
                    });

                    publishingBus.Publish(publishedMessage, StorageType.NonPersistent);

                    Thread.Sleep(1000);

                    Assert.IsTrue(messageReceived, "Message should be received by the message handler!");

                    try
                    {
                        subscriptionBus.Dispose();
                    }
                    catch
                    {
                        Assert.Fail("Exception disposing SubscriptionBus while handling received message!");
                    }
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        [Test]
        [Category("unstable")]
        [Category("fast")]
        public void TestDisposeWhileHandlingMessageOnReceive()
        {
            DeleteQueues();
            try
            {
                var publishedMessage = new AMessage();

                using (var sendingBus = DependencyResolver.Get<ISendingBus>())
                {
                    var receivingBus = DependencyResolver.Get<IReceivingBus>();

                    var subscriptionId = SubscriptionId.FromString("TestDisposeWhileHandlingMessageOnReceive");

                    var messageReceived = false;

                    receivingBus.Receive<AMessage>(subscriptionId, m =>
                    {
                        messageReceived = true;
                        Thread.Sleep(10000); // It will take 10 seconds, but the ReceivingBus will be disposed after just 1 second
                    });

                    sendingBus.Send(publishedMessage, subscriptionId, StorageType.NonPersistent);

                    Thread.Sleep(1000);

                    Assert.IsTrue(messageReceived, "Message should be received by the message handler!");

                    try
                    {
                        receivingBus.Dispose();
                    }
                    catch
                    {
                        Assert.Fail("Exception disposing ReceivingBus while handling received message!");
                    }
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        [Test]
        [Category("unstable")]
        [Category("slow")]
        public void TestDisposeWhileHandlingMessageOnStartRespondingTo()
        {
            DeleteQueues();
            try
            {
                using (var requestBus = DependencyResolver.Get<IRequestBus>())
                {
                    var responseBus = DependencyResolver.Get<IResponseBus>();

                    var messageReceived = false;

                    var subscriptionId = SubscriptionId.FromString("TestDisposeWhileHandlingMessageOnStartRespondingTo");

                    responseBus.StartRespondingTo<RequestX, ResponseX>(r =>
                    {
                        messageReceived = true;
                        Thread.Sleep(10000); // It will take 10 seconds, but the ResponseBus will be disposed after just 1 second
                        return null;
                    }, subscriptionId);

                    try
                    {
                        requestBus.Request<RequestX, ResponseX>(new RequestX(RequesterId.New(), RequestId.New()),
                            subscriptionId);
                    }
                    catch (TimeoutException)
                    {
                    }
                    catch (Exception)
                    {
                        Assert.Fail("Request should be published and timeout!");
                    }
                    finally
                    {
                        try
                        {
                            responseBus.Dispose();
                        }
                        catch
                        {
                            Assert.Fail("Exception disposing ResponseBus while handling received message!");
                        }

                        Thread.Sleep(1000);

                        Assert.IsTrue(messageReceived, "Message should be received by the message handler!");
                    }
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        [Test]
        [Category("unstable")]
        [Category("fast")]
        public void TestAutoConnectBetweenPublishes()
        {
            DeleteQueues();
            try
            {
                using (var publishingBus = DependencyResolver.Get<IPublishingBus>())
                {
                    publishingBus.Publish(new AMessage(), StorageType.NonPersistent);

                    RestartMessageBrokerToForceConnectionToBeLost();

                    try
                    {
                        publishingBus.Publish(new AMessage(), StorageType.NonPersistent);
                    }
                    catch
                    {
                        Assert.Fail("Publish should be successful even if the connection was previously closed!");
                    }
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        [Test]
        [Category("unstable")]
        [Category("fast")]
        public void TestAutoConnectBetweenSends()
        {
            DeleteQueues();
            try
            {
                using (var sendingBus = DependencyResolver.Get<ISendingBus>())
                {
                    sendingBus.Should().NotBeNull();

                    var subscriptionId = SubscriptionId.FromString("TestAutoConnectBetweenSends");

                    sendingBus.Send(new AMessage(), subscriptionId, StorageType.NonPersistent);

                    RestartMessageBrokerToForceConnectionToBeLost();

                    try
                    {
                        sendingBus.Send(new AMessage(), subscriptionId, StorageType.NonPersistent);
                    }
                    catch
                    {
                        Assert.Fail("Send should be successful even if the connection was previously closed!");
                    }
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        [Test]
        [Category("unstable")]
        [Category("fast")]
        public void TestAutoConnectAfterSubscribeTo()
        {
            DeleteQueues();
            try
            {
                var publishedMessage = new AMessage();
                MessageId receivedMessageId = null;
                var expectedMessageId = publishedMessage.MessageId;

                using (var publishingBus = DependencyResolver.Get<IPublishingBus>())
                {
                    using (var subscriptionBus = DependencyResolver.Get<ISubscriptionBus>())
                    {
                        var subscriptionId = SubscriptionId.FromString("TestAutoConnectAfterSubscribeTo");

                        subscriptionBus.SubscribeTo<AMessage>(subscriptionId, m => receivedMessageId = ((AMessage)m).MessageId);

                        RestartMessageBrokerToForceConnectionToBeLost();

                        publishingBus.Publish(publishedMessage, StorageType.NonPersistent);

                        Thread.Sleep(1000);

                        receivedMessageId.Should().Be(expectedMessageId, "The published message should be received even if the connection was closed after the subscription!");
                    }
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        [Test]
        [Category("unstable")]
        [Category("fast")]
        public void TestAutoConnectAfterReceive()
        {
            DeleteQueues();
            try
            {
                var publishedMessage = new AMessage();
                MessageId receivedMessageId = null;
                var expectedMessageId = publishedMessage.MessageId;

                using (var sendingBus = DependencyResolver.Get<ISendingBus>())
                {
                    using (var receivingBus = DependencyResolver.Get<IReceivingBus>())
                    {
                        var subscriptionId = SubscriptionId.FromString("TestAutoConnectAfterReceive");

                        receivingBus.Receive<AMessage>(subscriptionId, m => receivedMessageId = ((AMessage)m).MessageId);

                        RestartMessageBrokerToForceConnectionToBeLost();

                        sendingBus.Send(publishedMessage, subscriptionId, StorageType.NonPersistent);

                        Thread.Sleep(1000);

                        receivedMessageId.Should().Be(expectedMessageId, "The sent message should be received even if the connection was closed after the receiving!");
                    }
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        [Test]
        [Category("unstable")]
        [Category("slow")]
        public void TestAutoConnectBetweenRequests()
        {
            DeleteQueues();
            try
            {
                using (var requestBus = DependencyResolver.Get<IRequestBus>())
                {
                    try
                    {
                        requestBus.Request<RequestX, ResponseX>(new RequestX(RequesterId.New(), RequestId.New()));
                    }
                    catch (TimeoutException)
                    {
                    }
                    catch (Exception)
                    {
                        Assert.Fail("Request should be published and timeout on the first try!");
                    }
                    RestartMessageBrokerToForceConnectionToBeLost();
                    try
                    {
                        requestBus.Request<RequestX, ResponseX>(new RequestX(RequesterId.New(), RequestId.New()));
                    }
                    catch (TimeoutException)
                    {
                    }
                    catch (Exception)
                    {
                        Assert.Fail("Request should be published and timeout even if the connection was previously closed!");
                    }
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        [Test]
        [Category("unstable")]
        [Category("fast")]
        public void TestAutoConnectAfterStartRespondingTo()
        {
            DeleteQueues();
            try
            {
                var requestX = new RequestX(RequesterId.New(), RequestId.New());

                using (var requestBus = DependencyResolver.Get<IRequestBus>())
                {
                    using (var responseBus = DependencyResolver.Get<IResponseBus>())
                    {
                        responseBus.StartRespondingTo<RequestX, ResponseX>(r => new ResponseX(ResponderId.New(), ResponseId.New(), r));

                        RestartMessageBrokerToForceConnectionToBeLost();

                        var receivedResponse = requestBus.Request<RequestX, ResponseX>(requestX);

                        receivedResponse.Should().NotBeNull("A response must be received for the sent request!");
                        receivedResponse.Request.RequestId.Should().Be(requestX.RequestId, "The request associated with the received response must be the sent request!");
                    }
                }
            }
            finally
            {
                DeleteQueues();
            }
        }

        internal static void RestartMessageBrokerToForceConnectionToBeLost()
        {
            try
            {
                if (IsRabbitMQOnLocalHost())
                    WindowsService.Restart("RabbitMQ", TimeSpan.FromSeconds(15));
                else
                {
                    var host = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME");
                    const string rabbitmqcontainer = "rabbitmq-rstests";
                    var client = new SshClient(host, "docker", "tcuser");
                    client.Connect();
                    client.RunCommand("docker restart " + rabbitmqcontainer);
                    client.Disconnect();
                }

                Thread.Sleep(TimeSpan.FromSeconds(20)); // Wait some time to allow RabbitMQ to be ready again
            }
            catch (Exception)
            {
                Assert.Fail("Could not restart the Message Broker! Check if you have administrator privileges.");
            }
        }

        private static bool IsRabbitMQOnLocalHost()
        {
            return Settings.ConnectionStrings.MessageBus.ConnectionString.Contains("localhost") ||
                   Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME") == "localhost";
        }

        private static void DeleteQueues()
        {
            using (var bus = DependencyResolver.Get<ISubscriptionBus>())
            {
                bus.DeleteSubscriptionQueue(typeof(EventOccurred), SubscriptionId.FromString("TestMessageTransferRate"));
                bus.DeleteSubscriptionQueue(typeof(AMessage), SubscriptionId.FromString("TestAutoConnectAfterReceive"));
                bus.DeleteSubscriptionQueue(typeof(AMessage), SubscriptionId.FromString("TestAutoConnectAfterSubscribeTo"));
                bus.DeleteSubscriptionQueue(typeof(AMessage), SubscriptionId.FromString("TestDisposeAfterReceive"));
                bus.DeleteSubscriptionQueue(typeof(AMessage), SubscriptionId.FromString("TestDisposeWhileHandlingMessageOnReceive"));
                bus.DeleteSubscriptionQueue(typeof(AMessage), SubscriptionId.FromString("TestDisposeWhileHandlingMessageOnSubscribeTo"));
                bus.DeleteSubscriptionQueue(typeof(AMessage), SubscriptionId.FromString("TestMessageBusInitialization"));
                bus.DeleteSubscriptionQueue(typeof(AMessage), SubscriptionId.FromString("TestDisposeWhileHandlingMessageOnStartRespondingTo"));
            }
        }
    }
}