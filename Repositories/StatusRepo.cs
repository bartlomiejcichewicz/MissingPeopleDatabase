using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MissingPeopleDatabase.Data;
using MissingPeopleDatabase.Interfaces;
using MissingPeopleDatabase.Models;
using Microsoft.EntityFrameworkCore;
using CodesByAniz.Tools;

namespace MissingPeopleDatabase.Repositories
{
    public class StatusRepo : IStatus
    {
        private readonly PeopleContext _context;
        public StatusRepo(PeopleContext context)
        {
            _context = context;
        }
        public Status Create(Status status)
        {
            _context.Statuses.Add(status);
            _context.SaveChanges();
            return status;
        }

        public Status Delete(Status status)
        {
            _context.Statuses.Attach(status);
            _context.Entry(status).State = EntityState.Deleted;
            _context.SaveChanges();
            return status;
        }

        public Status Edit(Status status)
        {
            _context.Statuses.Attach(status);
            _context.Entry(status).State = EntityState.Modified;
            _context.SaveChanges();
            return status;
        }


        private List<Status> DoSort(List<Status> items, string SortProperty, SortOrder sortOrder)
        {

            if (SortProperty.ToLower() == "name")
            {
                if (sortOrder == SortOrder.Ascending)
                    items = items.OrderBy(n => n.Name).ToList();
                else
                    items = items.OrderByDescending(n => n.Name).ToList();
            }
            else
            {
                if (sortOrder == SortOrder.Ascending)
                    items = items.OrderBy(d => d.Description).ToList();
                else
                    items = items.OrderByDescending(d => d.Description).ToList();
            }

            return items;
        }

        public PaginatedList<Status> GetItems(string SortProperty, SortOrder sortOrder, string SearchText = "", int pageIndex = 1, int pageSize = 5)
        {
            List<Status> items;

            if (SearchText != "" && SearchText != null)
            {
                items = _context.Statuses.Where(n => n.Name.Contains(SearchText) || n.Description.Contains(SearchText))
                    .ToList();
            }
            else
                items = _context.Statuses.ToList();

            items = DoSort(items, SortProperty, sortOrder);

            PaginatedList<Status> retItems = new PaginatedList<Status>(items, pageIndex, pageSize);

            return retItems;
        }

        public Status GetItem(int id)
        {
            Status item = _context.Statuses.Where(u => u.Id == id).FirstOrDefault();
            return item;
        }
        public bool IsItemExists(string name)
        {
            int ct = _context.Statuses.Where(n => n.Name.ToLower() == name.ToLower()).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }

        public bool IsItemExists(string name, int Id)
        {
            int ct = _context.Statuses.Where(n => n.Name.ToLower() == name.ToLower() && n.Id != Id).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }

    }
}
