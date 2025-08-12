using System.ComponentModel.DataAnnotations;

namespace StockNotificationWarning.Entities
{
    public class ProductDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(120)]
        public string ShopifyProductGid { get; set; } = default!;
        [MaxLength(120)]
        public string CustomDisplayName { get; set; } = default!;
        public string? CustomDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
