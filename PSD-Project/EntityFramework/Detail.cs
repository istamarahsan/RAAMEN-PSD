namespace PSD_Project.EntityFramework
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Detail")]
    public partial class Detail
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Headerid { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Ramenid { get; set; }

        public int? Quantity { get; set; }

        public virtual Header Header { get; set; }

        public virtual Raman Raman { get; set; }
    }
}
