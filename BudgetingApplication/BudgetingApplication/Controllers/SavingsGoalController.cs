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

           
            SavingsGoalList = dbContext.SavingsGoals.Where(x => x.ClientID == CLIENT_ID && x.Status == "Active").OrderByDescending(x=>x.CurrentGoalAmount/x.SavingsGoalAmount).ToList();

            SavingsGoalsViewModel savingsGoal = new SavingsGoalsViewModel();
            savingsGoal.savingsView = SavingsGoalList;
            savingsGoal.savingsViewFails = dbContext.SavingsGoals.Where(x => x.ClientID == CLIENT_ID && x.Status == "Fail").ToList();
            savingsGoal.savingsViewSuccesses = dbContext.SavingsGoals.Where(x => x.ClientID == CLIENT_ID && x.Status == "Success").ToList();
            savingsGoal.totalBudgeted = savingsGoal.savingsView.Select(x => x).Where(x => x.SavingGoalID != 0).Sum(x => Convert.ToDouble(x.SavingsGoalAmount));
            savingsGoal.totalSaved = savingsGoal.savingsView.Select(x => x).Sum(x => Convert.ToDouble(x.CurrentGoalAmount));
            addBadge();
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
            newSavingsGoal.Status = "Active";
            if (newSavingsGoal.SavingGoalID == 0)
            {
                newSavingsGoal.StartDate = model.savingsGoal.StartDate;
                newSavingsGoal.CurrentGoalAmount = 0;
                newSavingsGoal.SavingsGoalAmount = model.savingsGoal.SavingsGoalAmount;
                newSavingsGoal.EndDate = model.savingsGoal.EndDate;
                newSavingsGoal.GoalDescription = model.savingsGoal.GoalDescription;
                newSavingsGoal.Recurring = model.savingsGoal.Recurring;
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

        public void addBadge()
        {
            //count number of badges
            List<SavingsGoal> goalList = new List<SavingsGoal>();
            goalList = dbContext.SavingsGoals.Where(x => x.ClientID == CLIENT_ID).ToList();
            BadgesModelView bmv = new BadgesModelView();

            //add badges based on goal count
            switch (goalList.Count())
            {
                case (1):
                    bmv.addNewBadge(75, CLIENT_ID);
                    break;
                case (3):
                    //add previous badges for testing
                    bmv.addNewBadge(75, CLIENT_ID);

                    bmv.addNewBadge(76, CLIENT_ID);
                    break;
                case (5):
                    bmv.addNewBadge(75, CLIENT_ID);

                    bmv.addNewBadge(76, CLIENT_ID);

                    bmv.addNewBadge(77, CLIENT_ID);
                    break;
            }

            
            //determine if user is completed with goals
            List<SavingsGoal> activeGoalList = dbContext.SavingsGoals.Where(x => x.ClientID == CLIENT_ID).Where(x => x.Status == "Active").ToList();
            List<SavingsGoal> successGoalList = dbContext.SavingsGoals.Where(x => x.ClientID == CLIENT_ID).Where(x => x.Status == "Success").ToList();
            if (activeGoalList.Count() == 0 && successGoalList.Count() >= 1)
            {
                bmv.addNewBadge(96, CLIENT_ID);
            }
            //determine is user is halfway through goals
            else
            {
            double totalGoalAmount = activeGoalList.Sum(x => Convert.ToDouble(x.SavingsGoalAmount));
            double totalSavingsAlloted = activeGoalList.Sum(x => Convert.ToDouble(x.CurrentGoalAmount));

            double ratio = totalSavingsAlloted / totalGoalAmount;

            if(ratio >= .5)
                {
                 bmv.addNewBadge(95, CLIENT_ID);
                }
            }
            
        }
    }
}