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
    
    public partial class Delivery_Schedule
    {
        public int Schedule_ID { get; set; }
        public Nullable<System.DateTime> Org_Research_Comp_Date { get; set; }
        public Nullable<System.DateTime> Construction_Comp_Date { get; set; }
        public Nullable<System.DateTime> Delivery_Prep_Comp_Date { get; set; }
        public Nullable<System.DateTime> Delivery_Date { get; set; }
        public string Time_Restraint { get; set; }
        public Nullable<int> Speech_ID { get; set; }
        public Nullable<int> ScheduleAssistance { get; set; }
    
        public virtual Speech Speech { get; set; }
    }
}