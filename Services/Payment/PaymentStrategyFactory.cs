using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services.Payment
{
    public class PixPaymentStrategy : IPaymentStrategy
    {
        public Task ProcessPayment(decimal amount, int customerId)
        {
            // Implementar lógica de pagamento via Pix
            return Task.CompletedTask;
        }
    }

    public class CreditCardPaymentStrategy : IPaymentStrategy
    {
        public Task ProcessPayment(decimal amount, int customerId)
        {
            // Implementar lógica de pagamento via cartão
            return Task.CompletedTask;
        }
    }

    public class PaypalPaymentStrategy : IPaymentStrategy
    {
        public Task ProcessPayment(decimal amount, int customerId)
        {
            // Implementar lógica de pagamento via PayPal
            return Task.CompletedTask;
        }
    }

    public class PaymentStrategyFactory
    {
        private readonly Dictionary<string, IPaymentStrategy> _strategies;

        public PaymentStrategyFactory()
        {
            _strategies = new Dictionary<string, IPaymentStrategy>(StringComparer.OrdinalIgnoreCase)
            {
            { "pix", new PixPaymentStrategy() },
            { "creditcard", new CreditCardPaymentStrategy() },
            { "paypal", new PaypalPaymentStrategy() }
        };
        }

        public IPaymentStrategy GetStrategy(string method)
        {
            if (!_strategies.TryGetValue(method, out var strategy))
                throw new ArgumentException($"Método de pagamento '{method}' não suportado.");

            return strategy;
        }
    }

}
