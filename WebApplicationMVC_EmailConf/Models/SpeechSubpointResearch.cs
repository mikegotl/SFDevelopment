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
    
    public partial class SpeechSubpointResearch
    {
        public int SpeechSubpointResearchID { get; set; }
        public Nullable<int> SpeechSubpointID { get; set; }
        public Nullable<int> SourceID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual Source Source { get; set; }
        public virtual SpeechSubPoint SpeechSubPoint { get; set; }
    }
}
