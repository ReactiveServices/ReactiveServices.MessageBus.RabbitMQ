using System;
using FluentAssertions;
using ReactiveServices.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.IO;
using ReactiveServices.Authorization;
using ReactiveServices.Extensions;
using ReactiveServices.MessageBus.Tests.UnitTests;

namespace ReactiveServices.MessageBus.Tests.Specifications
{
    public sealed class StepDefinitionsExecutor : IDisposable
    {
        #region Publish Subscribe

        private ISubscriptionBus SubscriptionBus;
        private IPublishingBus PublishingBus;

        private readonly List<Tuple<Type, SubscriptionId>> ActiveSubscriptionIds = new List<Tuple<Type, SubscriptionId>>();

        private TimeSpan? MessagesValidity;

        internal string RelevantField1Value = null;

        internal string RelevantField2Value = null;

        internal void InitializeMessageBus()
        {
            SubscriptionBus = DependencyResolver.Get<ISubscriptionBus>();
            PublishingBus = DependencyResolver.Get<IPublishingBus>();
        }

        internal void ResetInMemoryTestData()
        {
            ActiveSubscriptionIds.Clear();
            MessagesValidity = null;
            RelevantField1Value = null;
            RelevantField2Value = null;
            MessagesReceivedBySubscriber1.Clear();
            MessagesReceivedBySubscriber2.Clear();
        }

        internal void RemoveAllSubscriptions()
        {
            foreach (var subscription in ActiveSubscriptionIds)
            {
                SubscriptionBus.RemoveSubscription(subscription.Item1, subscription.Item2);
            }
            ActiveSubscriptionIds.Clear();
            DeleteExchange<AMessage>();
            DeleteExchange<EventOccurred>();
            DeleteExchange<OtherEventOccurred>();
            DeleteSubscriptionQueue<AMessage>("Test");
            DeleteSubscriptionQueue<EventOccurred>("DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurred");
            DeleteSubscriptionQueue<EventOccurred>("DadoQueUmAssinanteSeInscrevaParaReceberAMensagemOtherEventOccurred");
            DeleteSubscriptionQueue<EventOccurred>("DadoQueOutroAssinanteSeInscrevaParaReceberAMensagemEventOccurred");
            DeleteSubscriptionQueue<EventOccurred>("DadoQueOutroAssinanteSeInscrevaParaReceberAMensagemOtherEventOccurred");
            DeleteSubscriptionQueue<EventOccurred>("DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurredNoTopicoConfirmed");
            DeleteSubscriptionQueue<OtherEventOccurred>("DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurred");
            DeleteSubscriptionQueue<OtherEventOccurred>("DadoQueUmAssinanteSeInscrevaParaReceberAMensagemOtherEventOccurred");
            DeleteSubscriptionQueue<OtherEventOccurred>("DadoQueOutroAssinanteSeInscrevaParaReceberAMensagemEventOccurred");
            DeleteSubscriptionQueue<OtherEventOccurred>("DadoQueOutroAssinanteSeInscrevaParaReceberAMensagemOtherEventOccurred");
        }

        public void Dispose()
        {
            PublishingBus.Dispose();
            SubscriptionBus.Dispose();
            foreach (var requester in Requesters.Values)
            {
                requester.Item1.Dispose();
            }
            foreach (var responder in Responders.Values)
            {
                responder.Dispose();
            }
        }

        internal List<object> MessagesReceivedBySubscriber1 = new List<object>();

        internal List<object> MessagesReceivedBySubscriber2 = new List<object>();

        internal void MessageHandlerForSubscriber1(object receivedMessage)
        {
            MessagesReceivedBySubscriber1.Add(receivedMessage);
        }

        internal void MessageHandlerForSubscriber2(object receivedMessage)
        {
            MessagesReceivedBySubscriber2.Add(receivedMessage);
        }

        internal void PrepareSubscription<TMessage>(string subscriptionIdValue)
            where TMessage : class, new()
        {
            var subscriptionId = SubscriptionId.FromString(subscriptionIdValue);
            SubscriptionBus.PrepareSubscriptionTo<TMessage>(subscriptionId);
            ActiveSubscriptionIds.Add(new Tuple<Type, SubscriptionId>(typeof(TMessage), subscriptionId));
        }

        internal void PerformSubscription<TMessage>(List<object> messagesReceivedBuffer, Action<object> onMessageReceived, string subscriptionIdValue, string topicIdValue)
            where TMessage : class, new()
        {
            var subscriptionId = SubscriptionId.FromString(subscriptionIdValue);

            if (topicIdValue == null)
                SubscriptionBus.SubscribeTo<TMessage>(subscriptionId, onMessageReceived);
            else
                SubscriptionBus.SubscribeTo<TMessage>(TopicId.FromString(topicIdValue), subscriptionId, onMessageReceived);

            ActiveSubscriptionIds.Add(new Tuple<Type, SubscriptionId>(typeof(TMessage), subscriptionId));
        }

        internal void SetMessageValidity<TMessage>(int elapsedTimeInSeconds)
            where TMessage : class, new()
        {
            MessagesValidity = TimeSpan.FromSeconds(elapsedTimeInSeconds);
        }

        internal void PerformPublication<TMessage>(string topicId = null, int messageCount = 1, Action<TMessage> configureMessage = null, StorageType storageType = StorageType.Persistent)
            where TMessage : class, new()
        {
            var range = Enumerable.Range(0, messageCount);
            foreach (var i in range.AsParallel())
            {
                var message = new TMessage();

                if (configureMessage != null)
                {
                    configureMessage(message);
                }

                using (var publishingBus = DependencyResolver.Get<IPublishingBus>())
                {
                    if (topicId == null)
                        publishingBus.Publish(message, storageType, expiration: MessagesValidity.GetValueOrDefault());
                    else
                        publishingBus.Publish(TopicId.FromString(topicId), message, storageType, expiration: MessagesValidity.GetValueOrDefault());
                }
            }
        }

        internal void Wait(int timeToWaitInSeconds)
        {
            Thread.Sleep(timeToWaitInSeconds * 1000);
        }

        internal void DeleteSubscriptionQueue<TMessage>(string subscriptionIdValue)
        {
            var subscriptionId = SubscriptionId.FromString(subscriptionIdValue);
            SubscriptionBus.RemoveSubscription(typeof(TMessage), subscriptionId);
            SubscriptionBus.DeleteSubscriptionQueue(typeof(TMessage), subscriptionId);
        }

        internal void DeleteExchange<TMessage>()
        {
            PublishingBus.DeletePublishingExchange(typeof(TMessage));
        }

        internal bool HasSubscription<TMessage>(string subscriptionIdValue)
        {
            return SubscriptionBus.IsListenningTo(typeof(TMessage), SubscriptionId.FromString(subscriptionIdValue));
        }

        private readonly Dictionary<RequesterId, Tuple<IRequestBus, SubscriptionId>> Requesters = new Dictionary<RequesterId, Tuple<IRequestBus, SubscriptionId>>();
        private readonly Dictionary<ResponderId, IResponseBus> Responders = new Dictionary<ResponderId, IResponseBus>();

        internal void RestartMessageBroker()
        {
            MessageBusTests.RestartMessageBrokerToForceConnectionToBeLost();
        }

        #endregion

        #region Request Response

        public readonly string SpecificRequestXSubscription = "SubscriptionX";

        internal void RemoveRequestQueues()
        {
            using (var requester = DependencyResolver.Get<IRequestBus>())
            {
                //   ReactiveServices.MessageBus.Tests.RequestX:ReactiveServices.MessageBus.Tests

                requester.DeleteRequestQueue(typeof(RequestX));
                requester.DeleteRequestQueue(typeof(RequestY));
                requester.DeleteRequestQueue(typeof(RequestX), SubscriptionId.FromString(SpecificRequestXSubscription));
                requester.DeleteRequestQueue(typeof(AuthorizationRequest), SubscriptionId.FromString(AuthorizationRequestSubscriptionId));
            }
        }

        internal void PrepareRequestResponder(string responderIdValue, string requestTypeName, string subscriptionIdValue = null)
        {
            var responder = DependencyResolver.Get<IResponseBus>();
            var responderId = ResponderId.FromString(responderIdValue);
            var requestType = TypeOfName(requestTypeName);
            Debug.Assert(requestType != null, "requestType != null");
            var responseType = ResponseTypeFor(requestTypeName);
            var subscriptionId = subscriptionIdValue != null ? SubscriptionId.FromString(subscriptionIdValue) : null;
            responder.StartRespondingTo(requestType, responseType, request =>
            {
                var response = NewResponse(responseType, responderId, ResponseId.New(), request);
                Thread.Sleep(100);
                return response;
            }, subscriptionId);
            Responders[responderId] = responder;
        }

        private static IResponse NewResponse(Type responseType, ResponderId responderId, ResponseId responseId, IRequest request)
        {
            var response = (IResponse)Activator.CreateInstance(responseType, responderId, responseId, request);
            return response;
        }

        private static Type ResponseTypeFor(string requestTypeName)
        {
            string responseTypeName = null;
            switch (requestTypeName)
            {
                case "RequestX":
                    responseTypeName = "ResponseX";
                    break;
                case "RequestY":
                    responseTypeName = "ResponseY";
                    break;
            }

            if (responseTypeName == null)
                throw new ArgumentException("Invalid requestTypeName!");

            return TypeOfName(responseTypeName);
        }

        private readonly Dictionary<RequestId, IResponse> ResponseOf = new Dictionary<RequestId, IResponse>();

        internal void PrepareRequestRequester(string requesterIdValue, string requestTypeName, string subscriptionIdValue = null)
        {
            var requester = DependencyResolver.Get<IRequestBus>();
            var requesterId = RequesterId.FromString(requesterIdValue);
            var subscriptionId = subscriptionIdValue != null ? SubscriptionId.FromString(subscriptionIdValue) : null;
            Requesters[requesterId] = new Tuple<IRequestBus, SubscriptionId>(requester, subscriptionId);
        }

        internal void InitializeDependencyResolver()
        {
            DependencyResolver.Reset();
            DependencyResolver.Initialize();
        }

        private static Type TypeOfName(string typeName)
        {
            switch (typeName)
            {
                case "RequestX":
                    return typeof(RequestX);
                case "RequestY":
                    return typeof(RequestY);
                case "ResponseX":
                    return typeof(ResponseX);
                case "ResponseY":
                    return typeof(ResponseY);
                default:
                    throw new ArgumentException(String.Format("Invalid typeName: {0}!", typeName));
            }
        }

        private static IRequest NewRequest(Type requestType, RequesterId requesterId, RequestId requestId)
        {
            var request = (IRequest)Activator.CreateInstance(requestType, requesterId, requestId);
            return request;
        }

        internal void SendRequest(string requesterIdValue, string requestIdValue, string requestTypeName, bool requestAsync)
        {
            var requesterAndSubscriptionId = Requesters[RequesterId.FromString(requesterIdValue)];
            var requester = requesterAndSubscriptionId.Item1;
            var subscriptionId = requesterAndSubscriptionId.Item2;
            var requestType = TypeOfName(requestTypeName);
            Debug.Assert(requestType != null, "requestType != null");
            var responseType = ResponseTypeFor(requestTypeName);
            var requesterId = RequesterId.FromString(requesterIdValue);
            var requestId = RequestId.FromString(requestIdValue);
            var request = NewRequest(requestType, requesterId, requestId);
            try
            {
                if (requestAsync)
                    ResponseOf[requestId] = requester.RequestAsync(requestType, responseType, request, subscriptionId).Result;
                else
                    ResponseOf[requestId] = requester.Request(requestType, responseType, request, subscriptionId);
            }
            catch (TimeoutException)
            {
                // If the request times out, no response for the corresponding requestId should be found latter
            }
            catch (AggregateException ae)
            {
                // If the request times out, no response for the corresponding requestId should be found latter
                if (!(ae.InnerException is TimeoutException))
                    throw;
            }
        }

        internal void SendRequests(string requesterIdValue, int requestCount, string requestTypeName, bool requestAsync)
        {
            var requestIdValues =
                Enumerable.Range(0, requestCount)
                    .Select(requestIndex => String.Format("{0}_{1}", requestTypeName, requestIndex));

            foreach (var requestIdValue in requestIdValues)
            {
                SendRequest(requesterIdValue, requestIdValue, requestTypeName, requestAsync);
            }
        }

        internal void AssertThatRequestResponseWasReceived(string requesterIdValue, string requestIdValue, string responderIdValue)
        {
            AssertThatRequestResponseWasReceived(requesterIdValue, requestIdValue, responderIdValue, ResponseOf);
        }

        private static void AssertThatRequestResponseWasReceived(string requesterIdValue, string requestIdValue,
            string responderIdValue, IReadOnlyDictionary<RequestId, IResponse> responseOf)
        {
            var requestId = RequestId.FromString(requestIdValue);
            var requesterId = RequesterId.FromString(requesterIdValue);
            responseOf.ContainsKey(requestId).Should().BeTrue();
            responseOf[requestId].ShouldNotBeNull();
            responseOf[requestId].Request.ShouldNotBeNull();
            responseOf[requestId].Request.RequesterId.Should().Be(requesterId);
            responseOf[requestId].Request.RequestId.Should().Be(requestId);
            if (responderIdValue == null)
                return;
            var responderId = ResponderId.FromString(responderIdValue);
            responseOf[requestId].ResponderId.Should().Be(responderId);
        }

        public void AssertThatOneOfTwoPossibleResponsesWasReceived(string requesterIdValue, string requestId1Value, string requestId2Value)
        {
            var requestId1 = RequestId.FromString(requestId1Value);
            var requestId2 = RequestId.FromString(requestId2Value);
            var requesterId = RequesterId.FromString(requesterIdValue);

            (ResponseOf.ContainsKey(requestId1) || ResponseOf.ContainsKey(requestId2)).Should().BeTrue();

            if (ResponseOf.ContainsKey(requestId1))
            {
                ResponseOf[requestId1].ShouldNotBeNull();
                ResponseOf[requestId1].Request.ShouldNotBeNull();
            }

            if (ResponseOf.ContainsKey(requestId2))
            {
                ResponseOf[requestId2].ShouldNotBeNull();
                ResponseOf[requestId2].Request.ShouldNotBeNull();
            }

            (ResponseOf[requestId1].Request.RequesterId == requesterId || ResponseOf[requestId2].Request.RequesterId == requesterId).Should().BeTrue();
        }

        internal void AssertThatRequestResponseWasNotReceived(string requesterIdValue, string requestIdValue, string responderIdValue)
        {
            var requestId = RequestId.FromString(requestIdValue);
            ResponseOf.ContainsKey(requestId).Should().BeFalse();
        }

        internal void AssertThatRequestResponsesWereReceived(string requesterIdValue, int requestCount, string requestTypeName)
        {
            var requestIdValues =
                Enumerable.Range(0, requestCount)
                    .Select(requestIndex => String.Format("{0}_{1}", requestTypeName, requestIndex));

            foreach (var requestIdValue in requestIdValues)
            {
                AssertThatRequestResponseWasReceived(requesterIdValue, requestIdValue, null);
            }
        }

        internal void AssertThatAtLeastOneRequestResponseWasReceived(string requestTypeName, string responderIdValue)
        {
            AssertThatAtLeastOneRequestResponseWasReceived(requestTypeName, responderIdValue, ResponseOf);
        }

        private static void AssertThatAtLeastOneRequestResponseWasReceived(string requestTypeName, string responderIdValue,
            Dictionary<RequestId, IResponse> responseOf)
        {
            var responses = responseOf.Values.Where(response => response.Request != null);
            responses = responses.Where(response => response.Request.GetType().Name.EndsWith(requestTypeName));
            responses = responses.Where(response => response.ResponderId == ResponderId.FromString(responderIdValue));
            responses.Count().Should().BeGreaterThan(0);
        }

        #endregion

        #region Authorized Messages

        private Func<AuthorizationRequest, AuthorizationResponse> AuthorizationAction = req => new AuthorizationResponse(req, AuthorizationError.None);
        private bool AuthorizedMessageReceivedOnSubscriptionBus;
        private bool AuthorizedMessageReceivedOnResponseBus;
        private bool AuthorizedMessageReceivedOnReceivingBus;
        private Exception AuthorizationException;
        private const string AllowAll_Authorization = "AllowAll_Authorization.config";
        private const string DenyAll_Authorization = "DenyAll_Authorization.config";
        private const string DenyAllExceptSpecifiled_Authorization = "DenyAllExceptSpecifiled_Authorization.config";
        private const string ResourceFolderName = "Resources";
        private const string AuthorizationRequestSubscriptionId = "AllowAll";

        internal void EnsureAuthorizationFileDoesNotExist()
        {
            if (File.Exists(Authorizer.AuthorizationFileName))
                File.Delete(Authorizer.AuthorizationFileName);
        }

        internal void EnsureAuthorizationFileExists()
        {
            File.Copy(Path.Combine(ResourceFolderName, AllowAll_Authorization), Authorizer.AuthorizationFileName, true);
        }

        internal void ChangeAuthorizationFileToAllowAllByDefault()
        {
            File.Copy(Path.Combine(ResourceFolderName, AllowAll_Authorization), Authorizer.AuthorizationFileName, true);
        }

        internal void ChangeAuthorizationFileToDenyAllByDefault()
        {
            File.Copy(Path.Combine(ResourceFolderName, DenyAll_Authorization), Authorizer.AuthorizationFileName, true);
        }

        internal void PrepareAMessageNotDiscriminatedInTheAuthorizationFile()
        {
        }

        internal void PrepareAMessageDiscriminatedInTheAuthorizationFile()
        {
            File.Copy(Path.Combine(ResourceFolderName, DenyAllExceptSpecifiled_Authorization), Authorizer.AuthorizationFileName, true);
        }

        internal void PrepareAuthorizationServiceThatAllowAllMessages()
        {
            AuthorizationAction = req => new AuthorizationResponse(req, AuthorizationError.None);
        }

        internal void PrepareAuthorizationServiceThatDenyAllMessages()
        {
            AuthorizationAction = req => new AuthorizationResponse(req, AuthorizationError.UnauthorizedToken);
        }

        internal void TryToSendAndReceiveAuthorizedMessages()
        {
            try
            {
                Authorizer.LoadConfiguration();

                using (var authorizationService = DependencyResolver.Get<IResponseBus>())
                {
                    authorizationService.StartRespondingTo(
                        AuthorizationAction, SubscriptionId.FromString(AuthorizationRequestSubscriptionId), AuthorizationHeaders());

                    ExecuteAPublishSubscribeCase();

                    ExecuteARequestResponseCase();

                    ExecuteASendReceiveCase();
                }
            }
            catch (TimeoutException)
            {
            }
            catch (Exception e)
            {
                AuthorizationException = e;
            }
        }

        private void ExecuteASendReceiveCase()
        {
            AuthorizedMessageReceivedOnReceivingBus = false;

            // Receive message that requires authorization
            using (var receivingBus = DependencyResolver.Get<IAuthorizedReceivingBus>())
            {
                receivingBus.Receive<AMessage>(
                    SubscriptionId.FromString("Test"),
                    m => { AuthorizedMessageReceivedOnReceivingBus = true; }
                    );

                // Send message that requires authorization
                using (var sendingBus = DependencyResolver.Get<IAuthorizedSendingBus>())
                {
                    var message = NewMessage<AMessage>();
                    sendingBus.Send(message, SubscriptionId.FromString("Test"), headers: AuthorizationHeaders());
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        private void ExecuteARequestResponseCase()
        {
            AuthorizedMessageReceivedOnResponseBus = false;

            // Respond to message that requires authorization
            using (var responseBus = DependencyResolver.Get<IAuthorizedResponseBus>())
            {
                responseBus.StartRespondingTo<RequestX, ResponseX>(
                    req =>
                    {
                        AuthorizedMessageReceivedOnResponseBus = true;
                        return new ResponseX(ResponderId.FromString("Test"), ResponseId.FromString("Test"), req);
                    }, headers: AuthorizationHeaders());

                // Request message that requires authorization
                using (var requestBus = DependencyResolver.Get<IAuthorizedRequestBus>())
                {
                    var message = NewMessage<RequestX>(RequesterId.FromString("Test"), RequestId.FromString("Test"));
                    requestBus.Request<RequestX, ResponseX>(message, headers: AuthorizationHeaders());
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        private void ExecuteAPublishSubscribeCase()
        {
            AuthorizedMessageReceivedOnSubscriptionBus = false;

            // Subscribe to message that requires authorization
            using (var subscriptionBus = DependencyResolver.Get<IAuthorizedSubscriptionBus>())
            {
                subscriptionBus.SubscribeTo<AMessage>(SubscriptionId.FromString("Test"),
                    m => { AuthorizedMessageReceivedOnSubscriptionBus = true; }
                    );

                // Publish message that requires authorization
                using (var publishingBus = DependencyResolver.Get<IAuthorizedPublishingBus>())
                {
                    var message = NewMessage<AMessage>();
                    publishingBus.Publish(message, headers: AuthorizationHeaders());
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        private TMessage NewMessage<TMessage>(params object[] parameters)
        {
            var message = (TMessage)Activator.CreateInstance(typeof(TMessage), parameters);
            return message;
        }

        private static Dictionary<string, string> AuthorizationHeaders()
        {
            const string authenticationToken = "TestAuthenticationToken";
            var headers = new Dictionary<string, string> { { Authorizer.AuthenticationTokenKey, authenticationToken } };
            return headers;
        }

        internal void AssertThatSendAndReceivingOfAuthorizedMessagesHasFailedDueToMissingAuthorizationFile()
        {
            AuthorizationException.Should().BeOfType<FileNotFoundException>();
        }

        internal void AssertThatTheAuthorizationFileWasSuccessfullyLoaded()
        {
            Authorizer.Loaded.Should().BeTrue("Configuration should be loaded");
        }

        internal void AssertThatSendAndReceivingOfAuthorizedMessagesHasSucceeded()
        {
            AuthorizationException.Should().BeNull("No exception expected");
            AuthorizedMessageReceivedOnSubscriptionBus.Should().BeTrue("Message should be authorized for publish/subscribe operation");
            AuthorizedMessageReceivedOnResponseBus.Should().BeTrue("Message should be authorized for request/response operation");
            AuthorizedMessageReceivedOnReceivingBus.Should().BeTrue("Message should be authorized for send/receive operation");
        }

        internal void AssertThatSendAndReceivingOfAuthorizedMessagesHasFailedDueToAuthorizationDenial()
        {
            AuthorizationException.Should().NotBeNull("Exception expected");
            AuthorizedMessageReceivedOnSubscriptionBus.Should().BeFalse("Message should not be authorized for publish/subscribe operation");
            AuthorizedMessageReceivedOnResponseBus.Should().BeFalse("Message should not be authorized for request/response operation");
            AuthorizedMessageReceivedOnReceivingBus.Should().BeFalse("Message should not be authorized for send/receive operation");
        }

        #endregion

        public void WaitMessageToBeProcessed()
        {
            //Give some time to message to be processed
            Wait(1);
        }
    }
}