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
    
    public partial class SpeechStrategy
    {
        public int SpeechStrategyID { get; set; }
        public int SpeechID { get; set; }
        public int StrategyID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<bool> Best { get; set; }
    
        public virtual Speech Speech { get; set; }
        public virtual Strategy Strategy { get; set; }
    }
}
