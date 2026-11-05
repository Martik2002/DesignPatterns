namespace SagaPattern.Saga;

/// <summary>A compensation that itself failed, leaving the system in an inconsistent state.</summary>
/// <param name="StepName">The step whose compensation failed.</param>
/// <param name="Error">The exception thrown by the compensation.</param>
public record CompensationFailure(string StepName, Exception Error);

/// <summary>
/// The outcome of a saga run: either every step committed, or the saga unwound.
/// A failed saga is still a defined end state — unless a compensation also failed,
/// in which case <see cref="RequiresManualIntervention"/> is true.
/// </summary>
public class SagaResult
{
    private SagaResult(
        bool isSuccess,
        IReadOnlyList<string> completedSteps,
        IReadOnlyList<string> compensatedSteps,
        IReadOnlyList<CompensationFailure> compensationFailures,
        string? failedStep,
        Exception? error)
    {
        IsSuccess = isSuccess;
        CompletedSteps = completedSteps;
        CompensatedSteps = compensatedSteps;
        CompensationFailures = compensationFailures;
        FailedStep = failedStep;
        Error = error;
    }

    public bool IsSuccess { get; }

    /// <summary>Steps that executed successfully, in execution order.</summary>
    public IReadOnlyList<string> CompletedSteps { get; }

    /// <summary>Steps that were rolled back, in compensation order (reverse of execution).</summary>
    public IReadOnlyList<string> CompensatedSteps { get; }

    public IReadOnlyList<CompensationFailure> CompensationFailures { get; }

    /// <summary>The step that broke the saga, or <c>null</c> when it succeeded.</summary>
    public string? FailedStep { get; }

    public Exception? Error { get; }

    /// <summary>
    /// True when the saga could not fully undo itself. These cases need an alert and a human:
    /// retrying the compensation later, or fixing the state by hand.
    /// </summary>
    public bool RequiresManualIntervention => CompensationFailures.Count > 0;

    public static SagaResult Success(IReadOnlyList<string> completedSteps) =>
        new(true, completedSteps, Array.Empty<string>(), Array.Empty<CompensationFailure>(), null, null);

    public static SagaResult Failed(
        string failedStep,
        Exception error,
        IReadOnlyList<string> completedSteps,
        IReadOnlyList<string> compensatedSteps,
        IReadOnlyList<CompensationFailure> compensationFailures) =>
        new(false, completedSteps, compensatedSteps, compensationFailures, failedStep, error);
}
