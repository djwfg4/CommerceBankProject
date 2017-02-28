using BudgetingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace BudgetingApplication.Controllers
{
    public class DBTransactionsController : Controller
    {
        DatabaseContext dbContext = new DatabaseContext();

        // GET: Transactions
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult getTransactions()
        {
            var transactions = dbContext.Transactions.Where(
                trans => trans.TransactionAccountNo == 2 ||
                trans.TransactionAccountNo == 3).ToArray();
            
            return Json(transactions, JsonRequestBehavior.AllowGet);
        }
    }
}