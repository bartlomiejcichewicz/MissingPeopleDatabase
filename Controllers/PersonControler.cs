using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MissingPeopleDatabase.Data;
using MissingPeopleDatabase.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using MissingPeopleDatabase.Interfaces;
using CodesByAniz.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace MissingPeopleDatabase.Controllers
{
    [Authorize]
    public class PersonControler : Controller
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly ICity _cityRepo;
        private readonly IStatus _statusRepo;        
        private readonly ISex _sexRepo;
        private readonly IPerson _personRepo;
        public PersonControler(IPerson personRepo,ISex sexRepo, ICity cityRepo, IStatus statusRepo, IWebHostEnvironment webHost)
        {
            _webHost = webHost;
            _personRepo = personRepo;
            _sexRepo = sexRepo;
            _cityRepo = cityRepo;
            _statusRepo = statusRepo;
        }
        public IActionResult Index(string sortExpression = "", string SearchText = "", int pg = 1, int pageSize = 5)
        {
            SortModel sortModel = new SortModel();
            sortModel.AddColumn("Id");
            sortModel.AddColumn("First Name");
            sortModel.AddColumn("Last Name");
            sortModel.AddColumn("Sex");
            sortModel.AddColumn("City");
            sortModel.AddColumn("Status");
            sortModel.ApplySort(sortExpression);
            ViewData["sortModel"] = sortModel;
            ViewBag.SearchText = SearchText;
            PaginatedList<Person> persons = _personRepo.GetItems(sortModel.SortedProperty, sortModel.SortedOrder, SearchText, pg, pageSize);
            var pager = new PagerModel(persons.TotalRecords, pg, pageSize);
            pager.SortExpression = sortExpression;
            this.ViewBag.Pager = pager;
            TempData["CurrentPage"] = pg;
            return View(persons);
        }
        private void PopulateViewbags()
        {
            ViewBag.Sexes = GetSexes();
            ViewBag.Cities = GetCities();
            ViewBag.Statuses = GetStatuses();
        }
        public IActionResult Create()
        {
            Person person = new Person();
            PopulateViewbags();
            return View(person);
        }
        [HttpPost]
        public IActionResult Create(Person person)
        {
            bool bolret = false;
            string errMessage = "";
            try
            {
                if (person.LastName.Length < 5 || person.LastName == null)
                    errMessage = "Last name must be atleast 5 characters";
                if (_personRepo.IsItemCodeExists(person.Id) == true)
                    errMessage = errMessage + " " + " Person Id " + person.Id + " already exists!";
                if (errMessage == "")
                {
                    string uniqueFileName = GetUploadedFileName(person);
                    person.PhotoUrl = uniqueFileName;
                    person = _personRepo.Create(person);
                    bolret = true;
                }
            }
            catch (Exception ex)
            {
                errMessage = errMessage + " " + ex.Message;
            }
            if (bolret == false)
            {
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);
                PopulateViewbags();       
                return View(person);
            }
            else
            {
                TempData["SuccessMessage"] = person.FirstName + " added succesfully!";
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult Details(string id)
        {
            Person person = _personRepo.GetItem(id);
            return View(person);
        }
        public IActionResult Edit(string id)
        {
            Person person = _personRepo.GetItem(id);
            ViewBag.Sexes = GetSexes();
            ViewBag.Cities = GetCities();
            ViewBag.Statuses = GetStatuses();
            TempData.Keep();
            return View(person);
        }
        [HttpPost]
        public IActionResult Edit(Person person)
        {
            bool bolret = false;
            string errMessage = "";
            try
            {
                if (person.LastName.Length < 5 || person.LastName == null)
                    errMessage = "Last name must be at least 5 characters";
                if (person.PersonPhoto != null)
                {
                    string uniqueFileName = GetUploadedFileName(person);
                    person.PhotoUrl = uniqueFileName;
                }
                if (errMessage == "")
                {
                    person = _personRepo.Edit(person);
                    TempData["SuccessMessage"] = person.FirstName + person.LastName + ", added successfully!";
                    bolret = true;
                }
            }
            catch (Exception ex)
            {
                errMessage = errMessage + " " + ex.Message;
            }
            int currentPage = 1;
            if (TempData["CurrentPage"] != null)
                currentPage = (int)TempData["CurrentPage"];
            if (bolret == false)
            {
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);
                return View(person);
            }
            else
                return RedirectToAction(nameof(Index), new { pg = currentPage });
        }
        public IActionResult Delete(string id)
        {
            Person person = _personRepo.GetItem(id);
            TempData.Keep();
            return View(person);
        }
        [HttpPost]
        public IActionResult Delete(Person person)
        {
            try
            {
                person = _personRepo.Delete(person);
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message;
                if (ex.InnerException != null)
                    errMessage = ex.InnerException.Message;
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);
                return View(person);
            }

            int currentPage = 1;
            if (TempData["CurrentPage"] != null)
                currentPage = (int)TempData["CurrentPage"];
            TempData["SuccessMessage"] = person.FirstName + person.LastName + " deleted successfully";
            return RedirectToAction(nameof(Index), new { pg = currentPage });
        }
        private List<SelectListItem> GetSexes()
        {
            var lstSexes = new List<SelectListItem>();

            PaginatedList<Sex> sexes = _sexRepo.GetItems("Name", SortOrder.Ascending,"",1,1000);
            lstSexes = sexes.Select(ut => new SelectListItem()
            {
                Value = ut.Id.ToString(),
                Text = ut.Name
            }).ToList();
            var defItem = new SelectListItem()
            {
                Value="",
                Text="----Select Sex----"
            };
            lstSexes.Insert(0, defItem);
            return lstSexes;        
        }
        private List<SelectListItem> GetCities()
        {
            var lstItems = new List<SelectListItem>();

            PaginatedList<City> items = _cityRepo.GetItems("Name", SortOrder.Ascending, "", 1, 1000);
            lstItems = items.Select(ut => new SelectListItem()
            {
                Value = ut.Id.ToString(),
                Text = ut.Name
            }).ToList();
            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "----Select City----"
            };
            lstItems.Insert(0, defItem);
            return lstItems;
        }
        private List<SelectListItem> GetStatuses()
        {
            var lstItems = new List<SelectListItem>();
            PaginatedList<Status> items = _statusRepo.GetItems("Name", SortOrder.Ascending, "", 1, 1000);
            lstItems = items.Select(ut => new SelectListItem()
            {
                Value = ut.Id.ToString(),
                Text = ut.Name
            }).ToList();
            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "----Select Status----"
            };
            lstItems.Insert(0, defItem);
            return lstItems;
        }
        private string GetUploadedFileName(Person person)
        {
            string uniqueFileName = null;
            if (person.PersonPhoto != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + person.PersonPhoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    person.PersonPhoto.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        [AcceptVerbs("Get","Post")]
        public JsonResult IsPersonCodeValid(string Code,string Name="")
        {
            bool isExists = _personRepo.IsItemIdExists(Code,Name);

            if (isExists)
                return Json(data: false);
            else
                return Json(data: true);
        }
        [AcceptVerbs("Get", "Post")]
        public JsonResult IsPersonNameValid(string Name,string Code="")
        {
            bool isExists = _personRepo.IsItemExists(Name,Code);
            if (isExists)
                return Json(data: false);
            else
                return Json(data: true);
        }
    }
}
