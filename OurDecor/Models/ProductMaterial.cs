using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OurDecor.Models
{
    [Table("product_materials")]
    public class ProductMaterial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("quantity_required")]
        public decimal QuantityRequired { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [ValidateNever]
        public virtual Product Product { get; set; } = null!;

        [Required]
        [Column("material_id")]
        public int MaterialId { get; set; }

        [ForeignKey("MaterialId")]
        [ValidateNever]
        public virtual Material Material { get; set; } = null!;
    }
}
