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
    
    public partial class Cart_Items
    {
        public int Cart_ItemID { get; set; }
        public Nullable<int> Catalog_ItemID { get; set; }
        public Nullable<int> CartID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual Cart Cart { get; set; }
        public virtual Catalog_Items Catalog_Items { get; set; }
    }
}