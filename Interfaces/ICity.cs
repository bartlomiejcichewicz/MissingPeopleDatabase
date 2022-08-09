using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MissingPeopleDatabase.Models;
using CodesByAniz.Tools;

namespace MissingPeopleDatabase.Interfaces
{
    public interface ICity
    {
        PaginatedList<City> GetItems(string SortProperty, SortOrder sortOrder, string SearchText = "", int pageIndex = 1, int pageSize = 5);
        City GetItem(int id);
        City Create(City unit);
        City Edit(City unit);
        City Delete(City unit);
        public bool IsItemExists(string name);
        public bool IsItemExists(string name, int Id);
    }
}


