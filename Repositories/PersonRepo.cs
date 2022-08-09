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
    public class PersonRepo : IPerson
    {
        private readonly PeopleContext _context;
        public PersonRepo(PeopleContext context)
        {
            _context = context;
        }
        public Person Create(Person Person)
        {
            _context.Persons.Add(Person);
            _context.SaveChanges();
            return Person;
        }

        public Person Delete(Person Person)
        {
            Person = pGetItem(Person.Id);
            _context.Persons.Attach(Person);
            _context.Entry(Person).State = EntityState.Deleted;
            _context.SaveChanges();
            return Person;
        }

        public Person Edit(Person Person)
        {
            _context.Persons.Attach(Person);
            _context.Entry(Person).State = EntityState.Modified;
            _context.SaveChanges();
            return Person;
        }


        private List<Person> DoSort(List<Person> items, string SortProperty, SortOrder sortOrder)
        {

            if (SortProperty.ToLower() == "name")
            {
                if (sortOrder == SortOrder.Ascending)
                    items = items.OrderBy(n => n.FirstName).ToList();
                else
                    items = items.OrderByDescending(n => n.FirstName).ToList();
            }
            else if (SortProperty.ToLower() == "id")
            {
                if (sortOrder == SortOrder.Ascending)
                    items = items.OrderBy(n => n.Id).ToList();
                else
                    items = items.OrderByDescending(n => n.Id).ToList();
            }
            else
            {
                if (sortOrder == SortOrder.Ascending)
                    items = items.OrderBy(d => d.LastName).ToList();
                else
                    items = items.OrderByDescending(d => d.LastName).ToList();
            }

            return items;
        }

        public PaginatedList<Person> GetItems(string SortProperty, SortOrder sortOrder, string SearchText = "", int pageIndex = 1, int pageSize = 5)
        {
            List<Person> items;

            if (SearchText != "" && SearchText != null)
            {
                items = _context.Persons.Where(n => n.FirstName.Contains(SearchText) || n.LastName.Contains(SearchText))
                    .Include(u=>u.Sexes)
                    .ToList();
            }
            else
                items = _context.Persons.Include(u => u.Sexes).ToList();


            items = DoSort(items, SortProperty, sortOrder);

            PaginatedList<Person> retItems = new PaginatedList<Person>(items, pageIndex, pageSize);

            return retItems;
        }

        public Person GetItem(string Code)
        {
            Person item = _context.Persons.Where(u => u.Id == Code)
                .Include(u=>u.Sexes)
                .FirstOrDefault();

            item.BreifPhotoName = GetBriefPhotoName(item.PhotoUrl);

            return item;
        }


        private string GetBriefPhotoName(string fileName)
        {
            if (fileName==null)
                return "";

            string[] words = fileName.Split('_');
            return words[words.Length - 1];        
        }

    

        public Person pGetItem(string Code)
        {
            Person item = _context.Persons.Where(u => u.Id == Code)                
                .FirstOrDefault();
            return item;
        }
        public bool IsItemExists(string name)
        {
            int ct = _context.Persons.Where(n => n.FirstName.ToLower() == name.ToLower()).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }
        public bool IsItemExists(string name, string Code)
        {
            if (Code == "")
                return IsItemExists(name);
            var strCode = _context.Persons.Where(n => n.FirstName == name).Max(cd => cd.Id);
            if (strCode == null || strCode == Code)
                return false;
            else           
                 return true;           
        }
        public bool IsItemCodeExists(string itemCode)
        {
            int ct = _context.Persons.Where(n => n.Id.ToLower() == itemCode.ToLower()).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }
        public bool IsItemIdExists(string itemCode, string name)
        {
            if (name == "")
                return IsItemCodeExists(itemCode);
            var strName = _context.Persons.Where(n => n.Id == itemCode).Max(nm => nm.FirstName);
            if (strName == null || strName == name)
                return false;
            else          
                return IsItemExists(name);                                         
        }
    }
}
