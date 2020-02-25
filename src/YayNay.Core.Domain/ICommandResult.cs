using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Events;

namespace NatMarchand.YayNay.Core.Domain
{
    public interface ICommandResult
    {

    }

    public class FailureCommandResult : ICommandResult
    {
        public string Reason { get; }

        public FailureCommandResult(string reason)
        {
            Reason = reason;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"Failure : {Reason}";
        }
    }

    public class SuccessCommandResult : ICommandResult
    {

    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task<(ICommandResult result, IReadOnlyList<IDomainEvent> events)> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);
    }

    public interface ICommand
    {
    }
}