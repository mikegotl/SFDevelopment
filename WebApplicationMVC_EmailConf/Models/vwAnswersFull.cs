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
    
    public partial class vwAnswersFull
    {
        public Nullable<int> speechID { get; set; }
        public string Step { get; set; }
        public string Sub_Step { get; set; }
        public int StepOrder { get; set; }
        public int SubStep_ID { get; set; }
        public Nullable<int> Sub_StepOrder { get; set; }
        public string Question { get; set; }
        public string Choice_Type { get; set; }
        public string Choice { get; set; }
        public string Answer { get; set; }
        public int questionChoiceID { get; set; }
    }
}