using SagaPattern.Interfaces;
using SagaPattern.Models;

namespace SagaPattern.Steps;

public class ChargePaymentStep : ISagaStep<OrderContext>
{
    private readonly IPaymentService _paymentService;

    public ChargePaymentStep(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public string Name => "Charge payment";

    public async Task ExecuteAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        context.PaymentId = await _paymentService.ChargeAsync(context.OrderId, context.Amount, cancellationToken);
    }

    public async Task CompensateAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        if (context.PaymentId is null)
        {
            return;
        }

        await _paymentService.RefundAsync(context.PaymentId, cancellationToken);
        context.PaymentId = null;
    }
}
