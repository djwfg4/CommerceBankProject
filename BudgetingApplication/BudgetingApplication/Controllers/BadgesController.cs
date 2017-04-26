using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BudgetingApplication.ViewModels;
using BudgetingApplication.Models;
using System.Data.Entity;

namespace BudgetingApplication.Controllers
{
    public class BadgesController : Controller
    {
        private DataContext dbContext = new DataContext();
        private static int CLIENT_ID = 1;

        // GET: Badges
        public ActionResult Index()
        {
            
            //get badges the user has earned
            BadgesModelView badgeModel = new BadgesModelView();
            badgeModel.badges = getUserBadges();

            badgeModel.badgeCount = badgeModel.badges.Count();
            badgeModel.totalBadgeCount = dbContext.Badges.Count();

            return View(badgeModel);
        }

        private List<Badge> getUserBadges()
        {
            List<ClientBadge> ClientBadgeList = new List<ClientBadge>();
            ClientBadgeList = dbContext.ClientBadges.Where(x => x.ClientID == CLIENT_ID).ToList();
            List<Badge> TotalBadges = dbContext.Badges.ToList();
            List<Badge> BadgesEarned = new List<Badge>();

            for (int i = 0; i < ClientBadgeList.Count(); i++)
            {
                for (int j = 0; j < TotalBadges.Count(); j++)
                {
                    //find every badge the client has by their ID
                    if (ClientBadgeList[i].BadgeID == TotalBadges[j].BadgeID)
                    {
                        BadgesEarned.Add(TotalBadges[j]);
                    }
                }
            }


            return BadgesEarned;
        }
    }
}