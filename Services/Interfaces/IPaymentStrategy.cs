namespace ProvaPub.Services.Interfaces
{
    public interface IPaymentStrategy
    {
        Task ProcessPayment(decimal amount, int customerId);
    }

}
