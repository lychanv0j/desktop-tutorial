namespace nssets.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductDetailPage")]
    public partial class ProductDetailPage
    {
        [Key]
        public int ProductDetailID { get; set; }

        public int? ProductID { get; set; }

        [StringLength(100)]
        public string Brand { get; set; }

        [StringLength(100)]
        public string Product { get; set; }

        public int? RewardPoints { get; set; }

        [StringLength(50)]
        public string Availability { get; set; }

        public string Description { get; set; }

        [StringLength(50)]
        public string Size { get; set; }

        [StringLength(50)]
        public string Colour { get; set; }

        public virtual Product Product1 { get; set; }
    }
}
