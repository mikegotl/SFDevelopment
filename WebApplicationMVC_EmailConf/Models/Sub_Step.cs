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
    
    public partial class Sub_Step
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sub_Step()
        {
            this.qSub_Step_Question = new HashSet<qSub_Step_Question>();
            this.SubStepStatus = new HashSet<SubStepStatu>();
        }
    
        public int SubStep_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Step_ID { get; set; }
        public Nullable<int> Order_By { get; set; }
        public Nullable<bool> Active { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<qSub_Step_Question> qSub_Step_Question { get; set; }
        public virtual Step Step { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubStepStatu> SubStepStatus { get; set; }
    }
}