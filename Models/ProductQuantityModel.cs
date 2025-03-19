using System.ComponentModel.DataAnnotations;

namespace Shop_HTH.Models
{
    public class ProductQuantityModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu không bỏ trống số lượng")]
        public int Quantity { get; set; }

        public int ProductId { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
