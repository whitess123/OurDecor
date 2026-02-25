using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurDecor.Models
{
    [Table("material_types")]
    public class MaterialType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("defect_percentage")]
        public decimal DefectPercentage { get; set; }

        public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
    }
}
