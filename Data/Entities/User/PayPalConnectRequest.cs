namespace Data.Entities.User
{
    public class PayPalConnectRequest
    {
        public string UserId { get; set; }
        public string PayPalEmail { get; set; }
        public string PayPalFirstName { get; set; }
        public string PayPalLastName { get; set; }
    }
}