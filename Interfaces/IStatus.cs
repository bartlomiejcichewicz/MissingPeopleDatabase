using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MissingPeopleDatabase.Models;
using CodesByAniz.Tools;

namespace MissingPeopleDatabase.Interfaces
{
    public interface IStatus
    {
        PaginatedList<Status> GetItems(string SortProperty, SortOrder sortOrder, string SearchText = "", int pageIndex = 1, int pageSize = 5);
        Status GetItem(int id);
        Status Create(Status unit);
        Status Edit(Status unit);
        Status Delete(Status unit);
        public bool IsItemExists(string name);
        public bool IsItemExists(string name, int Id);
    }
}

