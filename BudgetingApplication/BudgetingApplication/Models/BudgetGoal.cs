//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BudgetingApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BudgetGoal
    {
        public int BudgetGoalID { get; set; }
        public int ClientID { get; set; }
        public string GoalCategory { get; set; }
        public int BudgetPointValue { get; set; }
        public decimal BudgetGoalAmount { get; set; }
        public System.DateTime Month { get; set; }
        public string Status { get; set; }
    
        public virtual Client Client { get; set; }
    }
}
