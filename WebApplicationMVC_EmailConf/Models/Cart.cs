//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplicationMVC_EmailConf.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cart
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cart()
        {
            this.Cart_Items = new HashSet<Cart_Items>();
        }
    
        public int CartID { get; set; }
        public Nullable<int> MemberID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> StatusID { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> Total { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart_Items> Cart_Items { get; set; }
        public virtual Member Member { get; set; }
        public virtual Status Status { get; set; }
    }
}
