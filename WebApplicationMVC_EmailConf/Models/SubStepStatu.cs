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
    
    public partial class SubStepStatu
    {
        public int SubStepStatusID { get; set; }
        public Nullable<int> SubStepID { get; set; }
        public Nullable<int> SpeechID { get; set; }
        public Nullable<int> StatusID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual Speech Speech { get; set; }
        public virtual Status Status { get; set; }
        public virtual Sub_Step Sub_Step { get; set; }
    }
}