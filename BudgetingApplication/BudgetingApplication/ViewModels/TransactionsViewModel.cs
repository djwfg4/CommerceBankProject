using BudgetingApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BudgetingApplication.ViewModels
{
    public class TransactionsViewModel
    {
        public List<Transaction> Transactions { get; set; }
        public List<Account> Accounts { get; set; }
        public Client Client { get; set; }
        public List<Category> Categories { get; set; }
    }
}