using BudgetingApplication.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BudgetingApplication.Controllers
{
    public class HomeController : Controller
    {
        DataContext dbContext = new DataContext();
        public static int CLIENT_ID = 1;

        // GET: Home
        public ActionResult Index()
        {
            indexModelView mv = new indexModelView();

            mv.budgetGoals = getBudgetInfo();
            mv.transactions = getTransactionInfo();


            return View(mv);
        }

        public ActionResult TechnicalPrototype()
        {



            return View();
        }

        private IEnumerable<Transaction> getTransactionInfo()
        {
            List<Transaction> transactions = new List<Transaction>();

            var query = from trans in dbContext.Transactions
                        from account in dbContext.Accounts
                        where trans.TransactionAccountNo == account.AccountNo && account.ClientID == CLIENT_ID
                        group new { trans, account } by trans.TransactionAccountNo into f
                        select new
                        {
                            accountDescr = f.Select(x => x.account.AccountType),
                            accountNo = f.Select(x=>x.account.AccountNo),
                            totalMoney = f.Sum(x => x.trans.TransactionAmount)
                        };

            foreach(var item in query)
            {
                Transaction trans = new Transaction();
                trans.Description = item.accountDescr.First();
                trans.TransactionAccountNo = item.accountNo.First();
                trans.TransactionAmount = item.totalMoney;
                transactions.Add(trans);
            }
            return transactions;
        }
        private BudgetGoalModelView getBudgetInfo()
        {
            List<BudgetGoals_VW> BudgetGoalList = new List<BudgetGoals_VW>();


            BudgetGoalList = dbContext.BudgetGoals_VW.Where(x => x.ClientID == CLIENT_ID).ToList();

            BudgetGoalModelView budgetGoal = new BudgetGoalModelView();
            budgetGoal.budgetView = BudgetGoalList;
            budgetGoal.totalBudgeted = budgetGoal.budgetView.Select(x => x).Where(x => x.GoalCategory != 1).Sum(x => Convert.ToDouble(x.BudgetGoalAmount));
            budgetGoal.totalSpent = budgetGoal.budgetView.Select(x => x).Where(x => x.GoalCategory != 1).Sum(x => Convert.ToDouble(x.TransactionAmount)) * -1;
            return budgetGoal;
        }

        public JsonResult GetCalendarData()
        {
            Dictionary<string, List<string>> transactionDictionary
                = new Dictionary<string, List<string>>();
            /*var transactions = dbContext.Transactions.Where(
                trans => trans.TransactionAccountNo == 2 ||
                trans.TransactionAccountNo == 3).ToArray();
            var query = from cat in dbContext.Categories
                        join trans in dbContext.Transactions on cat.CategoryID equals trans.TransactionCategory
                        select new { CategoryType = cat.CategoryType, TransactionDate = trans.TransactionDate };*/
            bool found;
            /*foreach (var trans in query)
            {
                found = false;
                if (transactionDictionary.Count == 0)
                {
                    transactionDictionary.Add(trans.CategoryType, new List<string>());
                    string date1 = Convert.ToDateTime(trans.TransactionDate).ToString("yyyy-MM-dd");
                    transactionDictionary[trans.CategoryType].Add(date1);
                }
                else
                {
                    foreach (KeyValuePair<string, List<string>> entry in transactionDictionary)
                    {
                        if (entry.Key.Equals(trans.CategoryType))
                        {
                            string date1 = Convert.ToDateTime(trans.TransactionDate).ToString("yyyy-MM-dd");
                            entry.Value.Add(date1);
                            found = true;
                        }
                    }
                    if (found == false)
                    {

                        string date1 = Convert.ToDateTime(trans.TransactionDate).ToString("yyyy-MM-dd");
                        transactionDictionary.Add(trans.CategoryType, new List<string>());
                        transactionDictionary[trans.CategoryType].Add(date1);
                    }
                }
            }*/
            Debug.WriteLine("Categories: " + transactionDictionary.Keys.ToString());
            Debug.WriteLine("Categories: " + transactionDictionary.Values.ToString());
            return new JsonResult { Data = transactionDictionary, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public JsonResult GetDonutAndBarGraphData()
        {
            List<Transaction> tr = new List<Transaction>();
            Dictionary<string, List<object>> dict = new Dictionary<string, List<object>>();
            Dictionary<string, List<object>> dict2 = new Dictionary<string, List<object>>();

            var monthlyTransacations = from trans in dbContext.Transactions
                                       from account in dbContext.Accounts
                                       where trans.TransactionDate.Month == DateTime.Now.Month &&
                                            trans.TransactionDate.Year == DateTime.Now.Year && 
                                            trans.TransactionAccountNo == account.AccountNo && 
                                            account.ClientID == CLIENT_ID
                                       select new { trans, account };

            var sumQuery = from querys in monthlyTransacations
                           group querys by new { querys.account.ClientID, querys.trans.CategoryID } into f
                           select new
                           {
                               ClientID = f.Key.ClientID,
                               TransactionAmount = f.Sum(x => x.trans.TransactionAmount),
                               CategoryID = f.Max(x => x.trans.CategoryID),
                               CategoryType = f.Select(x => x.trans.Category.CategoryType)

                           };
            List<object> labels = new List<object>();
            List<object> nums = new List<object>();
            string[] colors = { "rgb(255, 99, 132)",
                "rgb(255, 159, 64)",
                "rgb(255, 205, 86)",
                "rgb(75, 192, 192)",
                "rgb(54, 162, 235)",
                "rgb(153, 102, 255)",
                "rgb(231,233,237)" };

            foreach (var trans in sumQuery)
            {
                if (trans.CategoryID != 1)
                {
                    labels.Add(trans.CategoryType.First());
                    nums.Add(Math.Abs(trans.TransactionAmount));
                }
            }

            dict["datasets"] = new List<object>();

            dict2["backgroundColor"] = colors.ToList<object>();
            dict2["data"] = nums;

            dict["datasets"].Add(dict2);
            dict["labels"] = labels;

            return new JsonResult { Data = dict, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}