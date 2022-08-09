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
    public class SexRepository : ISex
    {
        private readonly PeopleContext _context;
        public SexRepository(PeopleContext context)
        {
            _context = context;
        }
        public Sex Create(Sex sex)
        {
            _context.Sexes.Add(sex);
            _context.SaveChanges();
            return sex;
        }

        public Sex Delete(Sex sex)
        {
            _context.Sexes.Attach(sex);
            _context.Entry(sex).State = EntityState.Deleted;
            _context.SaveChanges();
            return sex;
        }

        public Sex Edit(Sex sex)
        {
            _context.Sexes.Attach(sex);
            _context.Entry(sex).State = EntityState.Modified;
            _context.SaveChanges();
            return sex;
        }


        private List<Sex> DoSort(List<Sex> sexes, string SortProperty, SortOrder sortOrder)
        {           
                if (sortOrder == SortOrder.Ascending)
                    sexes = sexes.OrderBy(n => n.Name).ToList();
                else
                    sexes = sexes.OrderByDescending(n => n.Name).ToList();
                return sexes;
        }

        public PaginatedList<Sex> GetItems(string SortProperty, SortOrder sortOrder,string SearchText="",int pageIndex=1,int pageSize=5)
        {
            List<Sex> units;

            if (SearchText != "" && SearchText!=null)
            {
                units = _context.Sexes.Where(n => n.Name.Contains(SearchText))
                    .ToList();            
            }
            else
                units= _context.Sexes.ToList();

            units = DoSort(units,SortProperty,sortOrder);
            
            PaginatedList<Sex> retUnits = new PaginatedList<Sex>(units, pageIndex, pageSize);

            return retUnits;
        }

        public Sex GetSex(int id)
        {
            Sex unit = _context.Sexes.Where(u => u.Id == id).FirstOrDefault();
            return unit;
        }
        public bool IsSexNameExists(string name)
        {
            int ct = _context.Sexes.Where(n => n.Name.ToLower() == name.ToLower()).Count();
            if (ct > 0)
                return true;
            else
                return false;      
        }

        public bool IsSexNameExists(string name,int Id)
        {
            int ct = _context.Sexes.Where(n => n.Name.ToLower() == name.ToLower() && n.Id!=Id).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }

    }
}
