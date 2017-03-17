using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BudgetingApplication.Models;

namespace BudgetingApplication.Controllers
{
    public class BudgetGoalsController : Controller
    {
        private DataContext dbContext = new DataContext();
        private static int CLIENT_ID = 1;

        // GET: BudgetGoals
        public ActionResult Index()
        {

            List<BudgetGoals_VW> BudgetGoalList = new List<BudgetGoals_VW>();


            BudgetGoalList = dbContext.BudgetGoals_VW.Where(x => x.ClientID == CLIENT_ID).ToList();
           
            return View(BudgetGoalList.ToList());
        }

        public ActionResult InsertBudgetGoal()
        {
            ViewBag.GoalCategory = dbContext.Categories.Select(x => new SelectListItem()
            {
                Text = x.CategoryType,
                Value = x.CategoryID.ToString()
            }).OrderBy(v => v.Text);

            return View();
        }
        [HttpPost]
        public ActionResult InsertBudgetGoal(BudgetGoal newBudgetGoal)
        {
            //insert for next month
            newBudgetGoal.Month = new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1);
            newBudgetGoal.Status = "A";
            newBudgetGoal.BudgetPointValue = 25;
            newBudgetGoal.ClientID = CLIENT_ID;
            dbContext.BudgetGoals.Add(newBudgetGoal);
            dbContext.SaveChanges();
            return RedirectToAction("index");
        }
    }
}