using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetingApplication.Models
{
    public class indexModelView
    {
        public BudgetGoalModelView budgetGoals { get; set; }
        public IEnumerable<Transaction> transactions { get; set; }
    }
}