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
        private static int CLIENT_ID;

        // GET: SavingsGoal
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                CLIENT_ID = int.Parse(Session["UserID"].ToString());
            }
            List<SavingsGoal> SavingsGoalList = new List<SavingsGoal>();

           
            SavingsGoalList = dbContext.SavingsGoals.Where(x => x.ClientID == CLIENT_ID).Where(x => x.Status == "Active").ToList();

            SavingsGoalsViewModel savingsGoal = new SavingsGoalsViewModel();
            savingsGoal.savingsView = SavingsGoalList;
            savingsGoal.savingsViewFails = dbContext.SavingsGoals.Where(x => x.ClientID == CLIENT_ID && x.Status == "Fail").ToList();
            savingsGoal.savingsViewSuccesses = dbContext.SavingsGoals.Where(x => x.ClientID == CLIENT_ID && x.Status == "Success").ToList();
            savingsGoal.totalBudgeted = savingsGoal.savingsView.Select(x => x).Where(x => x.SavingGoalID != 0).Sum(x => Convert.ToDouble(x.SavingsGoalAmount));
            savingsGoal.totalSaved = savingsGoal.savingsView.Select(x => x).Sum(x => Convert.ToDouble(x.CurrentGoalAmount));
            return View(savingsGoal);
        }

        public ActionResult InsertSavingsGoal(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                CLIENT_ID = int.Parse(Session["UserID"].ToString());
            }
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
                newSavingsGoal.StartDate = model.savingsGoal.StartDate;
                newSavingsGoal.CurrentGoalAmount = 0;
                newSavingsGoal.SavingsGoalAmount = model.savingsGoal.SavingsGoalAmount;
                newSavingsGoal.EndDate = model.savingsGoal.EndDate;
                newSavingsGoal.GoalDescription = model.savingsGoal.GoalDescription;
                newSavingsGoal.Status = "Active";
                newSavingsGoal.Recurring = "No";
                dbContext.SavingsGoals.Add(newSavingsGoal);
                ModelState.Remove("savingsGoal.SavingGoalId"); //remove the error saying that this id is needed, when it will be automatically generated on insert
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

        public ActionResult AddFunds(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                CLIENT_ID = int.Parse(Session["UserID"].ToString());
            }

            if (id == null) //Only allow updates to a valid goal
            {
                return RedirectToAction("index");
            }
            SavingsGoal goal = dbContext.SavingsGoals.Where(x => x.ClientID == CLIENT_ID && x.SavingGoalID == id).FirstOrDefault();
            if(goal == null) //allow only valid updates
            {
                return RedirectToAction("index");
            }
            
            SavingsGoalsViewModel model = new SavingsGoalsViewModel();
            model.clientTransaction = new HomeController().getTransactionInfo(); //get total amount available on each client's accounts
            model.savingsGoal = goal;

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFunds(SavingsGoalsViewModel model)
        {
            if(model.addToGoal + model.savingsGoal.CurrentGoalAmount > model.savingsGoal.SavingsGoalAmount)
            {
                //Trying to add more than specified goal amount error
                ModelState.AddModelError("addToGoal", "The amount you want to enter is too large.");
            }
            if(model.addToGoal <= 0)
            {
                //less than 0 transaction error
                ModelState.AddModelError("addToGoal", "The amount you want to enter is too small.");
            }
            SavingsGoal goal = dbContext.SavingsGoals.Where(x => x.SavingGoalID == model.savingsGoal.SavingGoalID).FirstOrDefault();
            if(goal == null)
            {
                //error getting goal to update
                ModelState.AddModelError("addToGoal", "Error trying to add to this goal.");
            }
            if (ModelState.IsValid)
            {
                Transaction trans = new Transaction();
                trans.TransactionAccountNo = model.transaction.TransactionAccountNo;
                trans.TransactionAmount = model.addToGoal * -1;
                trans.TransactionDate = DateTime.Now;
                trans.Description = "Added to goal: " + model.savingsGoal.GoalDescription;
                trans.CategoryID = 17;

                goal.CurrentGoalAmount = goal.CurrentGoalAmount + model.addToGoal;

                dbContext.Transactions.Add(trans);

                dbContext.SaveChanges();
                return RedirectToAction("index");
            }
            //repopluate the transactions for the dropdown list
            model.clientTransaction = new HomeController().getTransactionInfo();
            return View(model);
        }

    }
}