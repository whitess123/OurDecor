using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurDecor.Models
{
    [Table("product_types")]
    public class ProductType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("coefficient")]
        public decimal Coefficient { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
