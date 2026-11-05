using SagaPattern.Interfaces;

namespace SagaPattern.Saga;

/// <summary>
/// Orchestration-based saga coordinator. It runs each <see cref="ISagaStep{TContext}"/> in order and,
/// as soon as one of them throws, compensates the already completed steps in reverse order.
/// The steps know nothing about each other — only this coordinator knows the sequence,
/// which is what separates orchestration from choreography.
/// </summary>
/// <typeparam name="TContext">State shared by all steps of the saga.</typeparam>
public class SagaOrchestrator<TContext>
{
    private readonly List<ISagaStep<TContext>> _steps = new();
    private readonly string _sagaName;

    public SagaOrchestrator(string sagaName)
    {
        _sagaName = sagaName;
    }

    /// <summary>Appends a step to the pipeline. The order of registration is the order of execution.</summary>
    public SagaOrchestrator<TContext> AddStep(ISagaStep<TContext> step)
    {
        _steps.Add(step);
        return this;
    }

    public async Task<SagaResult> ExecuteAsync(TContext context, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"-> Starting saga '{_sagaName}' with {_steps.Count} step(s)");

        // A stack gives us the reverse order for free: last completed step compensates first.
        var completed = new Stack<ISagaStep<TContext>>();

        foreach (var step in _steps)
        {
            try
            {
                Console.WriteLine($"   Step: {step.Name}");
                await step.ExecuteAsync(context, cancellationToken);
                completed.Push(step);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   !! '{step.Name}' failed: {ex.Message}");

                // Captured before compensation, which drains the stack.
                var completedNames = completed.Reverse().Select(s => s.Name).ToList();
                var (compensatedSteps, compensationFailures) = await CompensateAsync(context, completed);

                return SagaResult.Failed(
                    step.Name,
                    ex,
                    completedNames,
                    compensatedSteps,
                    compensationFailures);
            }
        }

        Console.WriteLine($"<- Saga '{_sagaName}' committed");
        return SagaResult.Success(completed.Reverse().Select(s => s.Name).ToList());
    }

    private async Task<(IReadOnlyList<string> Compensated, IReadOnlyList<CompensationFailure> Failures)> CompensateAsync(
        TContext context,
        Stack<ISagaStep<TContext>> completed)
    {
        if (completed.Count == 0)
        {
            Console.WriteLine("   Nothing to compensate, the first step never committed");
            return (Array.Empty<string>(), Array.Empty<CompensationFailure>());
        }

        Console.WriteLine($"   Compensating {completed.Count} completed step(s) in reverse order");

        var compensated = new List<string>();
        var failures = new List<CompensationFailure>();

        while (completed.Count > 0)
        {
            var step = completed.Pop();
            try
            {
                // Deliberately not passing the caller's token: once the saga starts unwinding,
                // a cancelled request must not stop us from undoing committed work.
                await step.CompensateAsync(context, CancellationToken.None);
                compensated.Add(step.Name);
            }
            catch (Exception ex)
            {
                // Keep compensating the remaining steps: one stuck service should not
                // block the rollback of everything else. The failure is reported instead.
                Console.WriteLine($"   !! Compensation for '{step.Name}' failed: {ex.Message}");
                failures.Add(new CompensationFailure(step.Name, ex));
            }
        }

        return (compensated, failures);
    }
}
