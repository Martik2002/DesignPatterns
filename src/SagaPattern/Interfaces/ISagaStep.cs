namespace SagaPattern.Interfaces;

/// <summary>
/// One local transaction inside a saga. Every step must be able to undo itself:
/// <see cref="ExecuteAsync"/> performs the work, <see cref="CompensateAsync"/> reverses it
/// when a later step fails. Compensation is a semantic undo (refund, release, cancel),
/// not a database rollback — the original transaction is already committed.
/// </summary>
/// <typeparam name="TContext">State shared by all steps of the saga.</typeparam>
public interface ISagaStep<in TContext>
{
    /// <summary>Human readable name, used for logging and for the saga result.</summary>
    string Name { get; }

    Task ExecuteAsync(TContext context, CancellationToken cancellationToken = default);

    Task CompensateAsync(TContext context, CancellationToken cancellationToken = default);
}
