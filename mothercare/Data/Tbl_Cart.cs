//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace mothercare.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tbl_Cart
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tbl_Cart()
        {
            this.Tbl_CartItems = new HashSet<Tbl_CartItems>();
        }
    
        public int CartId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> MemberId { get; set; }
        public Nullable<int> CartStatusId { get; set; }
        public string PaymentType { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> Total { get; set; }
    
        public virtual Tbl_Product Tbl_Product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_CartItems> Tbl_CartItems { get; set; }
    }
}
