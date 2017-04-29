using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BudgetingApplication.Models;
using System.Web.Mvc;

namespace BudgetingApplication.ViewModels
{
    public class SavingsGoalsViewModel
    {
        public List<SavingsGoal> savingsView { get; set; }
        public SavingsGoal savingsGoal { get; set; }
        public List<SelectListItem> active { get; set; }
        public double totalBudgeted { get; set; }
        public double totalSaved { get; set; }
        string progressType { get; set; }

        public int getPercentage()
        {
            double percentage = totalSaved / totalBudgeted;
            return Math.Abs((int)(percentage * 100));
        }
    }
}