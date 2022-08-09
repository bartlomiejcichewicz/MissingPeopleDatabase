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
    public class CityRepo : ICity
    {
        private readonly PeopleContext _context;
        public CityRepo(PeopleContext context)
        {
            _context = context;
        }
        public City Create(City City)
        {
            _context.Cities.Add(City);
            _context.SaveChanges();
            return City;
        }

        public City Delete(City City)
        {
            _context.Cities.Attach(City);
            _context.Entry(City).State = EntityState.Deleted;
            _context.SaveChanges();
            return City;
        }

        public City Edit(City City)
        {
            _context.Cities.Attach(City);
            _context.Entry(City).State = EntityState.Modified;
            _context.SaveChanges();
            return City;
        }


        private List<City> DoSort(List<City> items, string SortProperty, SortOrder sortOrder)
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
                    items = items.OrderBy(d => d.Province).ToList();
                else
                    items = items.OrderByDescending(d => d.Province).ToList();
            }

            return items;
        }

        public PaginatedList<City> GetItems(string SortProperty, SortOrder sortOrder, string SearchText = "", int pageIndex = 1, int pageSize = 5)
        {
            List<City> items;

            if (SearchText != "" && SearchText != null)
            {
                items = _context.Cities.Where(n => n.Name.Contains(SearchText) || n.Province.Contains(SearchText))
                    .ToList();
            }
            else
                items = _context.Cities.ToList();

            items = DoSort(items, SortProperty, sortOrder);

            PaginatedList<City> retItems = new PaginatedList<City>(items, pageIndex, pageSize);

            return retItems;
        }

        public City GetItem(int id)
        {
            City item = _context.Cities.Where(u => u.Id == id).FirstOrDefault();
            return item;
        }
        public bool IsItemExists(string name)
        {
            int ct = _context.Cities.Where(n => n.Name.ToLower() == name.ToLower()).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }

        public bool IsItemExists(string name, int Id)
        {
            int ct = _context.Cities.Where(n => n.Name.ToLower() == name.ToLower() && n.Id != Id).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }

    }
}
