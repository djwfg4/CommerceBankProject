﻿using BudgetingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetingApplication.ViewModels
{
    public class BadgesModelView
    {
        //an iterator of the badges needs to be here
        public int badgeCount;
        public int totalBadgeCount;

        private DataContext dbContext = new DataContext();

        public IEnumerable<Badge> badges { get; set; }

        //Add a badge if the user has not earned it yet.
        public void addNewBadge(int badgeID, int clientID)
        {
            ClientBadge cb = dbContext.ClientBadges.Where(x => x.ClientID == clientID && x.BadgeID == badgeID).FirstOrDefault();
            if (cb != null)
            {
                return;
            }
            else
            {
                cb = new ClientBadge();
            }
            cb.BadgeID = badgeID;
            cb.ClientID = clientID;
            cb.DateEarned = DateTime.Now;
            cb.Status = "new";
            cb.DateEarned = DateTime.Now;
            dbContext.ClientBadges.Add(cb);
            dbContext.SaveChanges();
        }
    }
}