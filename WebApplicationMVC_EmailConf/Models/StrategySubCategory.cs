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
    
    public partial class StrategySubCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StrategySubCategory()
        {
            this.Strategies = new HashSet<Strategy>();
        }
    
        public int StrategySubCategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ToolTip { get; set; }
        public Nullable<int> StrategyCategoryID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Strategy> Strategies { get; set; }
        public virtual StrategyCategory StrategyCategory { get; set; }
    }
}
