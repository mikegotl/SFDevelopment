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
    
    public partial class Main_Point_Transition
    {
        public int Transition_ID { get; set; }
        public int MainPoint_ID { get; set; }
    
        public virtual Main_Point Main_Point { get; set; }
    }
}