using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OurDecor.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("article")]
        public string Article { get; set; } = string.Empty;

        [Required]
        [Column("min_partner_price")]
        public decimal MinPartnerPrice { get; set; }

        [Required]
        [Column("roll_width")]
        public decimal RollWidth { get; set; }

        [Required]
        [Column("product_type_id")]
        public int ProductTypeId { get; set; }

        [ForeignKey("ProductTypeId")]
        public virtual ProductType ProductType { get; set; } = null!;

        public virtual ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
    }
}
