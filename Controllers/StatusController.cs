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

namespace MissingPeopleDatabase.Controllers
{
    [Authorize]
    public class StatusController : Controller
    {

        private readonly IStatus _Repo;
        public StatusController(IStatus repo)
        {
            _Repo = repo;
        }
        public IActionResult Index(string sortExpression = "", string SearchText = "", int pg = 1, int pageSize = 5)
        {
            SortModel sortModel = new SortModel();
            sortModel.AddColumn("name");
            sortModel.AddColumn("description");
            sortModel.ApplySort(sortExpression);
            ViewData["sortModel"] = sortModel;
            ViewBag.SearchText = SearchText;
            PaginatedList<Status> items = _Repo.GetItems(sortModel.SortedProperty, sortModel.SortedOrder, SearchText, pg, pageSize);
            var pager = new PagerModel(items.TotalRecords, pg, pageSize);
            pager.SortExpression = sortExpression;
            this.ViewBag.Pager = pager;
            TempData["CurrentPage"] = pg;
            return View(items);
        }
        public IActionResult Create()
        {
            Status item = new Status();
            return View(item);
        }
        [HttpPost]
        public IActionResult Create(Status item)
        {
            bool bolret = false;
            string errMessage = "";
            try
            {
                if (item.Description.Length < 10 || item.Description == null)
                    errMessage = "Description Must be atleast 10 Characters";
                if (_Repo.IsItemExists(item.Name) == true)
                    errMessage = errMessage + " " + item.Name + " already exists!";
                if (errMessage == "")
                {
                    item = _Repo.Create(item);
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
                return View(item);
            }
            else
            {
                TempData["SuccessMessage"] = "" + item.Name + " Created Successfully";
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult Details(int id)
        {
            Status item = _Repo.GetItem(id);
            return View(item);
        }
        public IActionResult Edit(int id)
        {
            Status item = _Repo.GetItem(id);
            TempData.Keep();
            return View(item);
        }
        [HttpPost]
        public IActionResult Edit(Status item)
        {
            bool bolret = false;
            string errMessage = "";
            try
            {
                if (item.Description.Length < 10 || item.Description == null)
                    errMessage = "Description Must be atleast 10 Characters";
                if (_Repo.IsItemExists(item.Name, item.Id) == true)
                    errMessage = errMessage +  item.Name + " Already Exists";
                if (errMessage == "")
                {
                    item = _Repo.Edit(item);
                    TempData["SuccessMessage"] = item.Name + ", Saved Successfully";
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
                return View(item);
            }
            else
                return RedirectToAction(nameof(Index), new { pg = currentPage });
        }
        public IActionResult Delete(int id)
        {
            Status item = _Repo.GetItem(id);
            TempData.Keep();
            return View(item);
        }
        [HttpPost]
        public IActionResult Delete(Status item)
        {
            try
            {
                item = _Repo.Delete(item);
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message;
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);
                return View(item);

            }
            int currentPage = 1;
            if (TempData["CurrentPage"] != null)
                currentPage = (int)TempData["CurrentPage"];

            TempData["SuccessMessage"] = item.Name + " Deleted Successfully";
            return RedirectToAction(nameof(Index), new { pg = currentPage });
        }
    }
}
