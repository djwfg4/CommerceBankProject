using BudgetingApplication.Models;
using BudgetingApplication.ViewModels;
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

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Login(Client objClient)
        {
            if (ModelState.IsValid)
            {
                using (dbContext)
                {
                    var obj = dbContext.Clients.Where(a => a.Username.Equals(objClient.Username) && a.Password.Equals(objClient.Password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.ClientID.ToString();
                        Session["UserName"] = obj.Username.ToString();
                        return RedirectToAction("UserDashBoard");
                    }
                }
            }
            return View(objClient);
        }

        public ActionResult UserDashBoard()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public static int CLIENT_ID = 1;

        // GET: Home
        public ActionResult Index()
        {
            BadgesModelView bmv = new BadgesModelView();
            bmv.addNewBadge(74, CLIENT_ID); //Give user inital load of app badge if they havent earned it.

            indexModelView mv = new indexModelView();

            mv.budgetGoals = getBudgetInfo();
            mv.transactions = getTransactionInfo();
            mv.badges = getAllBadges();
            mv.clientBadges = getClientBadges();
            decimal[] totals = getTotalIncomeAndSpent();
            mv.totalIncome = totals[0];
            mv.totalSpent = totals[1];
            return View(mv);
        }

        public ActionResult TechnicalPrototype()
        {
            return View();
        }

        private decimal[] getTotalIncomeAndSpent()
        {
            decimal spent = 0, income = 0;
            var query = from trans in dbContext.Transactions
                        from account in dbContext.Accounts
                        where trans.TransactionAccountNo == account.AccountNo && account.ClientID == CLIENT_ID
                        && trans.TransactionDate.Month == System.DateTime.Now.Month && trans.TransactionDate.Year == System.DateTime.Now.Year
                        group new { trans, account } by trans.TransactionAccountNo into f
                        select new
                        {
                            accountDescr = f.Select(x => x.account.AccountType),
                            accountNo = f.Select(x => x.account.AccountNo),
                            totalMoney = f.Sum(x => x.trans.TransactionAmount),
                            categoryID = f.Select(x => x.trans.CategoryID)
                        };
            foreach (var item in query)
            {
                if (item.categoryID.First() == 1)
                {
                    income += item.totalMoney;
                }
                else
                {
                    spent += item.totalMoney;
                }
            }
            return new decimal[] { income, spent };
        }
        private IEnumerable<Badge> getAllBadges()
        {
            return dbContext.Badges;
        }

        private IEnumerable<ClientBadge> getClientBadges()
        {
            return dbContext.ClientBadges.Where(x => x.ClientID == CLIENT_ID);
        }
        private IEnumerable<Transaction> getTransactionInfo()
        {
            List<Transaction> transactions = new List<Transaction>();

            var query2 = from trans in dbContext.Transactions
                    from account in dbContext.Accounts
                    where trans.TransactionAccountNo == account.AccountNo && account.ClientID == CLIENT_ID
                    group new { trans, account } by trans.TransactionAccountNo into f
                    select new
                    {
                        accountDescr = f.Select(x => x.account.AccountType),
                        accountNo = f.Select(x => x.account.AccountNo),
                        totalMoney = f.Sum(x => x.trans.TransactionAmount)
                    };


            foreach (var item in query2)
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


        public JsonResult GetDonutAndBarGraphData()
        {
            List<Transaction> tr = new List<Transaction>();
            Dictionary<string, List<object>> dict = new Dictionary<string, List<object>>();
            Dictionary<string, List<object>> dict2 = new Dictionary<string, List<object>>();

            var monthlyTransacations = from trans in dbContext.Transactions
                                       from account in dbContext.Accounts
                                       where trans.TransactionDate.Month == DateTime.Now.Month && trans.TransactionAccountNo == account.AccountNo && account.ClientID == CLIENT_ID
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

        public JsonResult GetNewlyEarnedBadges()
        {
            List<ClientBadge> newlyEarnedBadges = dbContext.ClientBadges.Where(x => x.ClientID == CLIENT_ID && x.Status == "new").ToList();
            Dictionary<String, object> dict = new Dictionary<String, object>();
            
            
            foreach ( ClientBadge cb in newlyEarnedBadges)
            {
                Badge badge = dbContext.Badges.Where(x => x.BadgeID == cb.BadgeID).FirstOrDefault();
                var badgeInfo = new { url = badge.BadgeName, date = cb.DateEarned.Date.ToLongDateString(), descr = badge.Description };
                dict[cb.BadgeID.ToString()] = badgeInfo;
                cb.Status = "earned";
            }

            //updated the badges as notified. 
            dbContext.SaveChangesAsync();

            return new JsonResult {Data = dict, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}