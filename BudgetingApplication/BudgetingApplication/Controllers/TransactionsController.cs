using BudgetingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BudgetingApplication.Controllers
{
    public class TransactionsController : Controller
    {
        private DatabaseContext dbContext = new DatabaseContext();

        // GET: Transactions
        public ActionResult Index()
        {

            List<Transaction> transactionList = new List<Transaction>();
            transactionList = dbContext.Transactions.ToList();

            return View(transactionList);
        }
    }
}