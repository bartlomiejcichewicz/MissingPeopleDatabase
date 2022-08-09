using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MissingPeopleDatabase.Models;
using CodesByAniz.Tools;

namespace MissingPeopleDatabase.Interfaces
{
    public interface IPerson
    {
        PaginatedList<Person> GetItems(string SortProperty, SortOrder sortOrder, string SearchText = "", int pageIndex = 1, int pageSize = 5);
        Person GetItem(string Id);
        Person Create(Person person);
        Person Edit(Person person);
        Person Delete(Person person);
        public bool IsItemExists(string name);
        public bool IsItemExists(string name, string Id);
        public bool IsItemCodeExists(string itemId);
        public bool IsItemIdExists(string itemId, string name);
    }
}

