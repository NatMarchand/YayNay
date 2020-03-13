using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

    public class DeniedCommandResult : FailureCommandResult
    {
        public DeniedCommandResult(string reason) 
            : base(reason)
        {
        }
    }

    public class NotFoundCommandResult : FailureCommandResult
    {
        public NotFoundCommandResult(string reason) 
            : base(reason)
        {
        }
    }

    public class ValidationFailureCommandResult : FailureCommandResult
    {
        public ValidationErrors ValidationErrors { get; }

        public ValidationFailureCommandResult(string reason)
            : base(reason)
        {
            ValidationErrors = new ValidationErrors();
        }

        public void AddValidationError(string key, string error)
        {
            ValidationErrors.AddError(key, error);
        }
    }

    [ExcludeFromCodeCoverage]
    public class ValidationErrors : IReadOnlyDictionary<string, IReadOnlyList<string>>
    {
        private readonly Dictionary<string, List<string>> _errors;

        public IEnumerable<string> Keys => _errors.Keys;
        public IEnumerable<IReadOnlyList<string>> Values => _errors.Values;
        public int Count => _errors.Sum(e => e.Value.Count);
        public IReadOnlyList<string> this[string key] => _errors[key];

        public ValidationErrors()
        {
            _errors = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void AddError(string key, string error)
        {
            if (!_errors.ContainsKey(key))
            {
                _errors[key] = new List<string>();
            }

            _errors[key].Add(error);
        }

        public IEnumerator<KeyValuePair<string, IReadOnlyList<string>>> GetEnumerator()
        {
            return _errors.Select(e => KeyValuePair.Create<string, IReadOnlyList<string>>(e.Key, e.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            return _errors.ContainsKey(key);
        }

        public bool TryGetValue(string key, out IReadOnlyList<string> value)
        {
            var found = _errors.TryGetValue(key, out var list);
            value = list;
            return found;
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