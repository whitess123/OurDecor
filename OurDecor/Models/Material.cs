using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OurDecor.Models
{
    [Table("materials")]
    public class Material
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column("stock_quantity")]
        public decimal StockQuantity { get; set; }

        [Required]
        [Column("min_quantity")]
        public decimal MinQuantity { get; set; }

        [Required]
        [Column("package_quantity")]
        public decimal PackageQuantity { get; set; }

        [Required]
        [Column("unit_of_measure")]
        public string UnitOfMeasure { get; set; } = string.Empty;

        [Required]
        [Column("material_type_id")]
        public int MaterialTypeId { get; set; }

        [ForeignKey("MaterialTypeId")]
        [ValidateNever]
        public virtual MaterialType MaterialType { get; set; } = null!;

        public virtual ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
    }
}
