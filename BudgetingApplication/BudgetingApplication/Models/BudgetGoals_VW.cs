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
    
    public partial class BudgetGoals_VW
    {
        public Nullable<int> BudgetGoalID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public Nullable<int> GoalCategory { get; set; }
        public Nullable<int> BudgetPointValue { get; set; }
        public Nullable<decimal> BudgetGoalAmount { get; set; }
        public decimal TransactionAmount { get; set; }
        public string ParentCategory { get; set; }
        public string CategoryType { get; set; }
        public string Status { get; set; }
    }
}
