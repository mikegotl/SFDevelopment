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
    
    public partial class SpeechSubpointConnection
    {
        public int SpeechSubpointConnectionID { get; set; }
        public string Text { get; set; }
        public Nullable<int> SpeechSubpointID { get; set; }
        public Nullable<int> ConnectionTypeID { get; set; }
    
        public virtual ConnectionType ConnectionType { get; set; }
        public virtual SpeechSubPoint SpeechSubPoint { get; set; }
    }
}
