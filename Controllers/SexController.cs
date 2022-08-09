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
    public class SexController : Controller
    {       
        
        private readonly ISex _sexRepo;
        public SexController(ISex sexrepo)
        {            
            _sexRepo = sexrepo;
        }
        public IActionResult Index(string sortExpression="", string SearchText = "",int pg=1,int pageSize=5) 
        {
            SortModel sortModel = new SortModel();
            sortModel.AddColumn("name");
            sortModel.ApplySort(sortExpression);
            ViewData["sortModel"] = sortModel;
            ViewBag.SearchText = SearchText;
            PaginatedList<Sex> sexes = _sexRepo.GetItems(sortModel.SortedProperty, sortModel.SortedOrder,SearchText,pg,pageSize);            
            var pager = new PagerModel(sexes.TotalRecords, pg, pageSize);
            pager.SortExpression = sortExpression;
            this.ViewBag.Pager = pager;
            TempData["CurrentPage"] = pg;
            return View(sexes);
        }
        public IActionResult Create()
        {
            Sex sex = new Sex();
            return View(sex);
        }
        [HttpPost]  
        public IActionResult Create(Sex sex)
        {
            bool bolret = false;
            string errMessage = "";
            try
            {
                if (_sexRepo.IsSexNameExists(sex.Name) == true)
                    errMessage = errMessage + " " + sex.Name +" already exists!";
                if (errMessage == "")
                {
                    sex = _sexRepo.Create(sex);
                    bolret = true;
                }                
            }
            catch(Exception ex) 
            {
                errMessage = errMessage + " " + ex.Message;
            }
            if (bolret == false)
            {
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);
                return View(sex);
            }
            else
            {
                TempData["SuccessMessage"] = sex.Name + " added succesfully!";
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult Details(int id) //Read
        {
            Sex sex =_sexRepo.GetSex(id);              
            return View(sex);        
        }
        public IActionResult Edit(int id)
        {
            Sex sex = _sexRepo.GetSex(id);
            TempData.Keep();
            return View(sex);
        }  
        [HttpPost]
        public IActionResult Edit(Sex sex)
        {
            bool bolret = false;
            string errMessage = "";
            try
            {
                if (_sexRepo.IsSexNameExists(sex.Name) == true)
                    errMessage = errMessage + sex.Name + " already exists!";
                if (errMessage == "")
                {
                    sex = _sexRepo.Edit(sex);
                    TempData["SuccessMessage"] = sex.Name + ", saved succesfully!";
                    bolret = true;
                }
            }
            catch(Exception ex)
            {
                errMessage = errMessage + " " + ex.Message;                
            }
            int currentPage = 1;
            if (TempData["CurrentPage"] != null)
                currentPage = (int)TempData["CurrentPage"];
            if(bolret==false)
            {
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);
                return View(sex);
            }
            else
            return RedirectToAction(nameof(Index),new {pg=currentPage});
        }
        public IActionResult Delete(int id)
        {
            Sex sex = _sexRepo.GetSex(id);
            TempData.Keep();
            return View(sex);
        }
        [HttpPost]
        public IActionResult Delete(Sex sex)
        {
            try
            {
                sex = _sexRepo.Delete(sex);
            }
            catch(Exception ex)
            {
                string errMessage = ex.Message;
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);
                return View(sex);
            }          
            int currentPage = 1;
            if (TempData["CurrentPage"] != null)
                currentPage = (int)TempData["CurrentPage"];
            TempData["SuccessMessage"] = sex.Name + " deleted successfully";
            return RedirectToAction(nameof(Index), new { pg = currentPage });
        }
    }
}
