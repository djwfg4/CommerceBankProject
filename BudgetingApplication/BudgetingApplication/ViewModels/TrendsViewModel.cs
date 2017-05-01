using BudgetingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BudgetingApplication.ViewModels
{
    public class TrendsViewModel
    {
        public DateTime StartDate { get; set; } //filtered start date
        public DateTime EndDate { get; set; } //filtered end date
        public decimal AverageSpending { get; set; }
        public decimal MostSpent { get; set; }
        public string MostSpentCategory { get; set; }
        public decimal LeastSpent { get; set; }
        public List<string> Colors { get; set; }
        public string LeastSpentCategory { get; set; }
        public decimal TotalSpent { get; set; }
        public List<Category> Categories { get; set; } //list of all budget/transaction categories
        public List<string> ChartLabels { get; set; }
        public string Category { get; set; } //used when user filters by category
        public List<Transaction> Transactions { get; set; }
        public List<BudgetGoal> BudgetGoals { get; set; }
        public List<decimal> TransactionTotals { get; set; }
        public Dictionary<string, decimal> TransactionAmounts { get; set; }
        public List<decimal> BudgetAmounts { get; set; } 
        public Dictionary<string, decimal> BudgetTotals { get; set; }
        public List<string> Months = new List<string> //used for filtering by month
        {
            "January", "February", "March", "April",
            "May", "June", "July", "August",
            "September", "October", "November", "December"
        };
        public List<string> Years = new List<string> //used for filtering by year
        {
            "2017", "2016", "2015", "2014"
        };
        public List<string> ColorsList = new List<string>
        {
            "#0000FF", //blue
            "#FF0000", //red
            "#008000", //green
            "#EEEE00", //yellow 
            "#800080", //purple
            "#FFA500", //orange
            "#FFC0CB", //pink
            "#ADD8E6", //lightblue
            "#90EE90", //lightgreen
            "#EE82EE", //violet
            "#40E0D0", //turquoise
            "#A52A2A", //brown
            "#006400", //darkgreen
            "#FFD700"  //gold
        };
        public bool ValidDates { get; set; } //used for displaying popup when dates selected are invalid
    }
}