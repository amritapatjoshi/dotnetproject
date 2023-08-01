namespace upos_device_simulation.Models
{
    public class CardSwipeEventArgs
    {
        public string Title { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string ExpirationDate { get; set; }
        public string ServiceCode { get; set; }
        public string Suffix { get; set; }
    }
}
