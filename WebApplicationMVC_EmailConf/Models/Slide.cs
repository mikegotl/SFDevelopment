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
    
    public partial class Slide
    {
        public int Slide_ID { get; set; }
        public string Title { get; set; }
        public string Content_Text { get; set; }
        public int Speech_ID { get; set; }
        public int Order_By { get; set; }
        public string File_Name { get; set; }
        public System.DateTime Entered_Date { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
    
        public virtual Speech Speech { get; set; }
    }
}
