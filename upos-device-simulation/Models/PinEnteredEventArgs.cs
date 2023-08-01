namespace upos_device_simulation.Models
{
    public class PinEnteredEventArgs
    {
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string ExpirationDate { get; set; }
        public string ServiceCode { get; set; }
        public string PinData { get; set; }
        public decimal Amount { get; set; }
        public string DeviceId { get; set; }
        public bool PaymentStatus { get; set; }
    }
}
