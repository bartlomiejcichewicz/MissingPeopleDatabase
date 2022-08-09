using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MissingPeopleDatabase.Models;
using CodesByAniz.Tools;

namespace MissingPeopleDatabase.Interfaces
{
    public interface ISex
    {
        PaginatedList<Sex> GetItems(string SortProperty,SortOrder sortOrder, string SearchText="", int pageIndex = 1, int pageSize = 5);
        Sex GetSex(int id);
        Sex Create(Sex unit);
        Sex Edit(Sex unit);
        Sex Delete(Sex unit);
        public bool IsSexNameExists(string name);
        public bool IsSexNameExists(string name, int Id);
    }
}
