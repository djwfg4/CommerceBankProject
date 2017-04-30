using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BudgetingApplication.Models;
using System.Data.Entity;
using BudgetingApplication.ViewModels;

namespace BudgetingApplication.Controllers
{
    public class SavingsGoalController : Controller
    {
        private DataContext dbContext = new DataContext();
        private static int CLIENT_ID = 1;

        // GET: SavingsGoal
        public ActionResult Index()
        {

            List<SavingsGoal> SavingsGoalList = new List<SavingsGoal>();

           
            SavingsGoalList = dbContext.SavingsGoals.Where(x => x.ClientID == CLIENT_ID).Where(x => x.Status == "Active").ToList();

            SavingsGoalsViewModel savingsGoal = new SavingsGoalsViewModel();
            savingsGoal.savingsView = SavingsGoalList;
            savingsGoal.totalBudgeted = savingsGoal.savingsView.Select(x => x).Where(x => x.SavingGoalID != 0).Sum(x => Convert.ToDouble(x.SavingsGoalAmount));
            savingsGoal.totalSaved = savingsGoal.savingsView.Select(x => x).Sum(x => Convert.ToDouble(x.CurrentGoalAmount));
            return View(savingsGoal);
        }

        public ActionResult InsertSavingsGoal(int? id)
        {
            SavingsGoalsViewModel goal = new SavingsGoalsViewModel();

            if (id == null)
            {
                return View(goal);
            }
            goal.savingsGoal = dbContext.SavingsGoals.Find(id);
            return View(goal);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertSavingsGoal(SavingsGoalsViewModel model)
        {
            SavingsGoal newSavingsGoal = new SavingsGoal();
            newSavingsGoal = model.savingsGoal;
            newSavingsGoal.ClientID = CLIENT_ID;
            newSavingsGoal.SavingsPointValue = 0;
            if (newSavingsGoal.SavingGoalID == 0)
            {
                newSavingsGoal.StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                newSavingsGoal.Status = "Active";
                newSavingsGoal.Recurring = "No";
                dbContext.SavingsGoals.Add(newSavingsGoal);
            }
            else
            {
                dbContext.Entry(newSavingsGoal).State = EntityState.Modified;
            }
            if (ModelState.IsValid)
            {
                dbContext.SaveChanges();
                return RedirectToAction("index");
            }
            return View(model);

        }

        public ActionResult AddFunds(SavingsGoalsViewModel model)
        {
            SavingsGoal newSavingsGoal = new SavingsGoal();
            newSavingsGoal = model.savingsGoal;
            //newSavingsGoal.ClientID = CLIENT_ID;
            //newSavingsGoal.SavingsPointValue = 0;

            return View(model);
        }

    }
}