namespace Shop_HTH.Models
{
    public class ShippingModel
    {
        public int Id { get; set; }

        public decimal Price { get; set; }

        public string Ward { get; set; } // Phường, xã

        public string District { get; set; } // Quận, huyện

        public string City { get; set; } // Tỉnh, thành phố
    }
}
