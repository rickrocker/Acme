//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyData
{
    using System;
    using System.Collections.Generic;
    
    public partial class vw_Tasks
    {
        public int ID { get; set; }
        public int TaskID { get; set; }
        public string UserID { get; set; }
        public Nullable<int> TaskTypeID { get; set; }
        public string TaskTypeName { get; set; }
        public Nullable<int> TaskStatusID { get; set; }
        public string TaskStatusName { get; set; }
        public Nullable<int> ContractID { get; set; }
        public Nullable<System.DateTime> ExpireDate { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public Nullable<int> MinutesUntilTaskExpires { get; set; }
    }
}