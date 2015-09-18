using System;
using System.Linq;
using PostSharp.Patterns.Diagnostics;
using Sprache;

// Copied from https://github.com/mikehadlow/EasyNetQ

namespace MessageBus.RabbitMQ.ConnectionString
{
    internal interface IConnectionStringParser
    {
        IConnectionConfiguration Parse(string connectionString);
    }

    [Log(AttributeExclude = true)]
    [LogException(AttributeExclude = true)]
    internal class ConnectionStringParser : IConnectionStringParser
    {
        public IConnectionConfiguration Parse(string connectionString)
        {
            try
            {
                var updater = ConnectionStringGrammar.ConnectionStringBuilder.Parse(connectionString);
                var connectionConfiguration = updater.Aggregate(new ConnectionConfiguration(), (current, updateFunction) => updateFunction(current));
                connectionConfiguration.Validate();
                return connectionConfiguration;
            }
            catch (ParseException parseException)
            {
                throw new Exception(String.Format("Exception Parsing Connection String: {0} - {1}", connectionString, parseException.Message));
            }
        }
    }
}