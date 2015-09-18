using NLog;
using PostSharp.Patterns.Diagnostics;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReactiveServices.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ReactiveServices.MessageBus.RabbitMQ
{
    /// <summary>
    /// Decorator over RabbitMQ.Client.IModel that handles its own exclusive Connection
    /// </summary>
    [Log(AttributeExclude = true)]
    [LogException(AttributeExclude = true)]
    sealed class RabbitMQChannel : IModel
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly IMessageSerializer Serializer = DependencyResolver.Get<IMessageSerializer>();

        public RabbitMQChannel(ConnectionFactory connectionFactory)
        {
            var connection = connectionFactory.CreateConnection();
            Model = connection.CreateModel();
            connection.AutoClose = true;

            WireUpExceptionalModelEvents();
        }

        private void WireUpExceptionalModelEvents()
        {
            Model.BasicReturn += OnMessageReturnedFromBroker;
            Model.CallbackException += OnCallbackException;
            Model.ModelShutdown += ModelOnModelShutdown;
        }

        private void ModelOnModelShutdown(object sender, ShutdownEventArgs reason)
        {
            Log.Debug("Model shutdown. Reason: {0}", reason);
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            Log.Error(e.Exception, "Callback exception on RabbitMQ.Client");
        }

        private void OnMessageReturnedFromBroker(object sender, BasicReturnEventArgs args)
        {
            Log.Error("Message returned from exchange '{1}'", args.Exchange);
        }

        private readonly IModel Model;

        public void Dispose()
        {
            if (Model.IsOpen)
                Model.Close();
        }

        public void Abort(ushort replyCode, string replyText)
        {
            Model.Abort(replyCode, replyText);
        }

        public void Abort()
        {
            Model.Abort();
        }

        public void BasicAck(ulong deliveryTag, bool multiple)
        {
            Model.BasicAck(deliveryTag, multiple);
        }

        public event EventHandler<BasicAckEventArgs> BasicAcks
        {
            add { Model.BasicAcks += value; }
            remove { Model.BasicAcks -= value; }
        }

        public void BasicCancel(string consumerTag)
        {
            Model.BasicCancel(consumerTag);
        }

        public string BasicConsume(string queue, bool noAck, string consumerTag, bool noLocal, bool exclusive, IDictionary<string, object> arguments, IBasicConsumer consumer)
        {
            return Model.BasicConsume(queue, noAck, consumerTag, noLocal, exclusive, arguments, consumer);
        }

        public string BasicConsume(string queue, bool noAck, string consumerTag, IDictionary<string, object> arguments, IBasicConsumer consumer)
        {
            return Model.BasicConsume(queue, noAck, consumerTag, arguments, consumer);
        }

        public string BasicConsume(string queue, bool noAck, string consumerTag, IBasicConsumer consumer)
        {
            return Model.BasicConsume(queue, noAck, consumerTag, consumer);
        }

        public string BasicConsume(string queue, bool noAck, IBasicConsumer consumer)
        {
            return Model.BasicConsume(queue, noAck, consumer);
        }

        public BasicGetResult BasicGet(string queue, bool noAck)
        {
            return Model.BasicGet(queue, noAck);
        }

        public void BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            Model.BasicNack(deliveryTag, multiple, requeue);
        }

        public event EventHandler<BasicNackEventArgs> BasicNacks
        {
            add { Model.BasicNacks += value; }
            remove { Model.BasicNacks -= value; }
        }

        public void BasicPublish(string exchange, string routingKey, bool mandatory, bool immediate, IBasicProperties basicProperties, byte[] body)
        {
            Model.BasicPublish(exchange, routingKey, mandatory, immediate, basicProperties, body);
        }

        public void BasicPublish(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body)
        {
            Model.BasicPublish(exchange, routingKey, mandatory, basicProperties, body);
        }

        public void BasicPublish(string exchange, string routingKey, IBasicProperties basicProperties, byte[] body)
        {
            Model.BasicPublish(exchange, routingKey, basicProperties, body);
        }

        public void BasicPublish(PublicationAddress addr, IBasicProperties basicProperties, byte[] body)
        {
            Model.BasicPublish(addr, basicProperties, body);
        }

        public void BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            Model.BasicQos(prefetchSize, prefetchCount, global);
        }

        public void BasicRecover(bool requeue)
        {
            Model.BasicRecover(requeue);
        }

        public void BasicRecoverAsync(bool requeue)
        {
            Model.BasicRecoverAsync(requeue);
        }

        public event EventHandler<EventArgs> BasicRecoverOk
        {
            add { Model.BasicRecoverOk += value; }
            remove { Model.BasicRecoverOk -= value; }
        }

        public void BasicReject(ulong deliveryTag, bool requeue)
        {
            Model.BasicReject(deliveryTag, requeue);
        }

        public event EventHandler<BasicReturnEventArgs> BasicReturn
        {
            add { Model.BasicReturn += value; }
            remove { Model.BasicReturn -= value; }
        }

        public event EventHandler<CallbackExceptionEventArgs> CallbackException
        {
            add { Model.CallbackException += value; }
            remove { Model.CallbackException -= value; }
        }

        public void Close(ushort replyCode, string replyText)
        {
            Model.Close(replyCode, replyText);
        }

        public void Close()
        {
            Model.Close();
        }

        public ShutdownEventArgs CloseReason
        {
            get { return Model.CloseReason; }
        }

        public void ConfirmSelect()
        {
            Model.ConfirmSelect();
        }

        public IBasicProperties CreateBasicProperties()
        {
            var props = Model.CreateBasicProperties();
            props.Headers = new ConcurrentDictionary<string, object>();
            return props;
        }

        public IBasicConsumer DefaultConsumer
        {
            get { return Model.DefaultConsumer; }
            set { Model.DefaultConsumer = value; }
        }

        public void ExchangeBind(string destination, string source, string routingKey)
        {
            Model.ExchangeBind(destination, source, routingKey);
        }

        public void ExchangeBindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            Model.ExchangeBindNoWait(destination, source, routingKey, arguments);
        }

        public void ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            Model.ExchangeBind(destination, source, routingKey, arguments);
        }

        public void ExchangeDeclare(string exchange, string type)
        {
            Model.ExchangeDeclare(exchange, type);
        }

        public void ExchangeDeclareNoWait(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            Model.ExchangeDeclareNoWait(exchange, type, durable, autoDelete, arguments);
        }

        public void ExchangeDeclare(string exchange, string type, bool durable)
        {
            Model.ExchangeDeclare(exchange, type, durable);
        }

        public void ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            Model.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);
        }

        public void ExchangeDeclarePassive(string exchange)
        {
            Model.ExchangeDeclarePassive(exchange);
        }

        public void ExchangeDelete(string exchange)
        {
            Model.ExchangeDelete(exchange);
        }

        public void ExchangeDeleteNoWait(string exchange, bool ifUnused)
        {
            Model.ExchangeDeleteNoWait(exchange, ifUnused);
        }

        public void ExchangeDelete(string exchange, bool ifUnused)
        {
            Model.ExchangeDelete(exchange, ifUnused);
        }

        public void ExchangeUnbind(string destination, string source, string routingKey)
        {
            Model.ExchangeUnbind(destination, source, routingKey);
        }

        public void ExchangeUnbindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            Model.ExchangeUnbindNoWait(destination, source, routingKey, arguments);
        }

        public void ExchangeUnbind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            Model.ExchangeUnbind(destination, source, routingKey, arguments);
        }

        public event EventHandler<FlowControlEventArgs> FlowControl
        {
            add { Model.FlowControl += value; }
            remove { Model.FlowControl -= value; }
        }

        public bool IsClosed
        {
            get { return Model.IsClosed; }
        }

        public bool IsOpen
        {
            get { return Model.IsOpen; }
        }

        public event EventHandler<ShutdownEventArgs> ModelShutdown
        {
            add { Model.ModelShutdown += value; }
            remove { Model.ModelShutdown -= value; }
        }

        public ulong NextPublishSeqNo
        {
            get { return Model.NextPublishSeqNo; }
        }

        public void QueueBind(string queue, string exchange, string routingKey)
        {
            Model.QueueBind(queue, exchange, routingKey);
        }

        public void QueueBindNoWait(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            Model.QueueBindNoWait(queue, exchange, routingKey, arguments);
        }

        public void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            Model.QueueBind(queue, exchange, routingKey, arguments);
        }

        public QueueDeclareOk QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            return Model.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
        }

        public void QueueDeclareNoWait(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            Model.QueueDeclareNoWait(queue, durable, exclusive, autoDelete, arguments);
        }

        public QueueDeclareOk QueueDeclare()
        {
            return Model.QueueDeclare();
        }

        public QueueDeclareOk QueueDeclarePassive(string queue)
        {
            return Model.QueueDeclarePassive(queue);
        }

        public uint QueueDelete(string queue)
        {
            return Model.QueueDelete(queue);
        }

        public void QueueDeleteNoWait(string queue, bool ifUnused, bool ifEmpty)
        {
            Model.QueueDeleteNoWait(queue, ifUnused, ifEmpty);
        }

        public uint QueueDelete(string queue, bool ifUnused, bool ifEmpty)
        {
            return Model.QueueDelete(queue, ifUnused, ifEmpty);
        }

        public uint QueuePurge(string queue)
        {
            return Model.QueuePurge(queue);
        }

        public void QueueUnbind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            Model.QueueUnbind(queue, exchange, routingKey, arguments);
        }

        public void TxCommit()
        {
            Model.TxCommit();
        }

        public void TxRollback()
        {
            Model.TxRollback();
        }

        public void TxSelect()
        {
            Model.TxSelect();
        }

        public bool WaitForConfirms(TimeSpan timeout)
        {
            return Model.WaitForConfirms(timeout);
        }

        public bool WaitForConfirms(TimeSpan timeout, out bool timedOut)
        {
            return Model.WaitForConfirms(timeout, out timedOut);
        }

        public bool WaitForConfirms()
        {
            return Model.WaitForConfirms();
        }

        public void WaitForConfirmsOrDie(TimeSpan timeout)
        {
            Model.WaitForConfirmsOrDie(timeout);
        }

        public int ChannelNumber
        {
            get { return Model.ChannelNumber; }
        }

        public void WaitForConfirmsOrDie()
        {
            Model.WaitForConfirmsOrDie();
        }
    }
}
