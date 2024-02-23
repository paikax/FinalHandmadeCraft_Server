namespace Data.ViewModels.User
{
    public class ExecutePaymentRequest
    {
        public string UserId { get; set; }
        public string PaymentId { get; set; }
        public string PayerId { get; set; }
    }
}