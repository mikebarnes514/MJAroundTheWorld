//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MJAroundTheWorld
{
    using System;
    using System.Collections.Generic;
    
    public partial class Photo
    {
        public int PhotoId { get; set; }
        public int LocationId { get; set; }
        public string Description { get; set; }
        public System.DateTime RecordCreatedDateTime { get; set; }
        public string ImageProvidedBy { get; set; }
        public Nullable<System.DateTime> PhotoTakenDate { get; set; }
        public string FileName { get; set; }
    
        public virtual Location Location { get; set; }
        public virtual PhotoImage PhotoImage { get; set; }
    }
}
