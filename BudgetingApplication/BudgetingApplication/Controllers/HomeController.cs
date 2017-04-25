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

        /// GET: Home - OVerview Page
        /// Loads all necessary data for it to be shown in one place
        /// 
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

        /// <summary>
        /// Technical Prototype for the project assignment. Just a proof of concept.
        /// </summary>
        /// <returns>TechnicalProtoype.cshtml view</returns>
        public ActionResult TechnicalPrototype()
        {
            return View();
        }

        /// <summary>
        /// This function will query the database for all the income for this current month and all the 
        /// transactions spent in the current month. 
        /// </summary>
        /// <returns>A 2 element array, first element is income, second is spent amount</returns>
        private decimal[] getTotalIncomeAndSpent()
        {
            decimal spent = 0, income = 0;

            //query for current client's transaction for this month
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

            //loop through all and add to either total spent or total income.
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
        /// <summary>
        /// Returns a list of all badges this app has to offer.
        /// </summary>
        /// <returns>list of badges</returns>
        private IEnumerable<Badge> getAllBadges()
        {
            return dbContext.Badges;
        }

        /// <summary>
        /// Get a list of current client's list of earned badges
        /// </summary>
        /// <returns>list of earned badges</returns>
        private IEnumerable<ClientBadge> getClientBadges()
        {
            return dbContext.ClientBadges.Where(x => x.ClientID == CLIENT_ID);
        }

        /// <summary>
        /// Get all transactions from database for each account the client owns.
        /// </summary>
        /// <returns>List of transactions for each account from current client</returns>
        private IEnumerable<Transaction> getTransactionInfo()
        {
            List<Transaction> transactions = new List<Transaction>();

            //query database and group by the account type and its sum
            var query = from trans in dbContext.Transactions
                    from account in dbContext.Accounts
                    where trans.TransactionAccountNo == account.AccountNo && account.ClientID == CLIENT_ID
                    group new { trans, account } by trans.TransactionAccountNo into f
                    select new
                    {
                        accountDescr = f.Select(x => x.account.AccountType),
                        accountNo = f.Select(x => x.account.AccountNo),
                        totalMoney = f.Sum(x => x.trans.TransactionAmount)
                    };

            //add each grouped amount into the list
            foreach (var item in query)
            {
                Transaction trans = new Transaction();
                trans.Description = item.accountDescr.First();
                trans.TransactionAccountNo = item.accountNo.First();
                trans.TransactionAmount = item.totalMoney;
                transactions.Add(trans);
            }
            return transactions;
        }

        /// <summary>
        /// Return a BudgetGoalModelView full of the information needed for it to be displayed 
        /// on the Overview Page. It will calculate the total budgeted and total spent. These will
        /// be individually displayed on the page.
        /// </summary>
        /// <returns>BudgetGoalModelView</returns>
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

        /// <summary>
        /// This method will be called through Ajax in the javascript files. This will 
        /// calculate all the information needed for the graphs to display information
        /// from each category. It generates the JSON that chart.js will use.
        /// </summary>
        /// <returns>JSON used by chart.js</returns>
        public JsonResult GetDonutAndBarGraphData()
        {
            List<Transaction> tr = new List<Transaction>();
            Dictionary<string, List<object>> dict = new Dictionary<string, List<object>>();
            Dictionary<string, List<object>> dict2 = new Dictionary<string, List<object>>();

            //get all transaction for all accounts the client has
            var monthlyTransacations = from trans in dbContext.Transactions
                                       from account in dbContext.Accounts
                                       where trans.TransactionDate.Month == DateTime.Now.Month && trans.TransactionAccountNo == account.AccountNo && account.ClientID == CLIENT_ID
                                       select new { trans, account };

            //from the above query, get the sum of each category within all accounts.
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

            //the colors of the graph sections/bars
            string[] colors = { "rgb(255, 99, 132)",
                "rgb(255, 159, 64)",
                "rgb(255, 205, 86)",
                "rgb(75, 192, 192)",
                "rgb(54, 162, 235)",
                "rgb(153, 102, 255)",
                "rgb(231,233,237)" };


            //add each category and it's amount to separate lists, skipping category 1, which is the income category
            foreach (var trans in sumQuery)
            {
                if (trans.CategoryID != 1)
                {
                    labels.Add(trans.CategoryType.First());
                    nums.Add(Math.Abs(trans.TransactionAmount));
                }
            }


            //create dictionaries to easily have it converted into JSON.
            dict["datasets"] = new List<object>();

            dict2["backgroundColor"] = colors.ToList<object>();
            dict2["data"] = nums;

            dict["datasets"].Add(dict2);
            dict["labels"] = labels;

            return new JsonResult { Data = dict, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        /// <summary>
        /// Gets all the badges the client has earned and returns the information to JSON. 
        /// This will also update the status of the clientbadge from "new" to "earned"
        /// </summary>
        /// <returns>JSON of badges earned</returns>
        public JsonResult GetNewlyEarnedBadges()
        {
            //Only return badges that are the client's and have a status of "new"
            List<ClientBadge> newlyEarnedBadges = dbContext.ClientBadges.Where(x => x.ClientID == CLIENT_ID && x.Status == "new").ToList();
            Dictionary<String, object> dict = new Dictionary<String, object>();
            
            //get the badge info to be displayed, then update the status.
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