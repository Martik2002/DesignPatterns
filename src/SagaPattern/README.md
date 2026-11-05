## Saga Pattern Implementation

This project includes an **Order Placement Saga** built with the orchestration flavour of the Saga pattern. It keeps data consistent across several services when a single ACID transaction is not available — the classic problem in a microservice or multi-provider architecture.

### The problem

Placing an order touches inventory, a payment gateway, a carrier and a mail provider. Each one commits on its own, so there is no shared transaction to roll back. If the carrier is down *after* the card was charged, the money is gone and the stock is still reserved.

### The solution

The saga splits the flow into four local transactions, each of which knows how to undo itself:

| Step | Transaction | Compensation |
|------|-------------|--------------|
| 1 | Reserve inventory | Release the reservation |
| 2 | Charge payment | Refund the payment |
| 3 | Create shipment | Cancel the shipment |
| 4 | Send confirmation | *(none — an email cannot be un-sent)* |

`SagaOrchestrator<TContext>` runs the steps in order and pushes each successful one onto a stack. When a step throws, it pops that stack and compensates everything already committed, in reverse order.

### Features

* **Orchestration, not choreography:** the sequence lives in one place, so the flow is easy to read and debug. Steps never reference each other.
* **Generic coordinator:** `SagaOrchestrator<TContext>` and `ISagaStep<TContext>` are reusable for any saga, not just orders.
* **Honest failure reporting:** `SagaResult` returns the failed step, which steps were rolled back, and whether a compensation itself failed (`RequiresManualIntervention`).
* **Compensation never gives up:** if one compensation throws, the remaining ones still run, and the error is collected rather than swallowed.
* **Cancellation-aware:** the caller's `CancellationToken` guards execution, but compensation deliberately runs with `CancellationToken.None` — a cancelled request must not leave committed work behind.

### Technical notes

Compensation is a *semantic* undo, not a rollback: the original transaction is already committed, so a refund is a new transaction that cancels out the old one. Because of that, compensations must be **idempotent** — a retried release of an already released reservation is a no-op here, not an error.

Irreversible steps belong at the end of the pipeline. The confirmation email is step 4 for exactly that reason: by the time it runs, nothing after it can force a rollback.
