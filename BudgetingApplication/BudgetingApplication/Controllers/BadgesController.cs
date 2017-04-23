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

            BadgesModelView badgeModel = new BadgesModelView();
            badgeModel.badgeNames = getUserBadges();
            badgeModel.badgeCount = badgeModel.badgeNames.Count();
            return View(badgeModel);
        }

        private List<String> getUserBadges()
        {
            List<ClientBadge> ClientBadgeList = new List<ClientBadge>();
            ClientBadgeList = dbContext.ClientBadges.Where(x => x.ClientID == CLIENT_ID).ToList();
            List<Badge> TotalBadges = dbContext.Badges.ToList();

            List<String> returnList = new List<String>();

            for(int i = 0; i < ClientBadgeList.Count(); i++)
            {
                for(int j = 0; j < TotalBadges.Count(); j++)
                {
                    if(ClientBadgeList[i].BadgeID == TotalBadges[j].BadgeID)
                    {
                        String temp = TotalBadges[j].BadgeName;
                        temp += ".png";
                        returnList.Add(TotalBadges[j].BadgeName );
                    }
                }
            }

            return returnList;
        }

        
    }
}