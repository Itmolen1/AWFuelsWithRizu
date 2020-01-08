﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using IT.Core.ViewModels;
using IT.Repository.WebServices;

namespace IT.Web.Controllers
{
    public class StorageController : Controller
    {
        WebServices webServices = new WebServices();
        List<StorageViewModel>storageViewModels = new List<StorageViewModel>();
        StorageViewModel StorageViewModel = new StorageViewModel();
       

        public ActionResult Index()
        {
            try
            {
                
                PagingParameterModel pagingParameterModel = new PagingParameterModel();
                pagingParameterModel.pageNumber = 1;
                pagingParameterModel._pageSize = 1;
                pagingParameterModel.Id = 0;
                pagingParameterModel.pageSize = 100;

                var StorageList = webServices.Post(pagingParameterModel, "Storage/All");

                if (StorageList.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    storageViewModels = (new JavaScriptSerializer().Deserialize<List<StorageViewModel>>(StorageList.Data.ToString()));
                    return View(storageViewModels);
                }
                return View(storageViewModels);

            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(StorageViewListModel storageViewListModel)
        {

            try
            {
                var value = DateTime.Now.ToFileTime().ToString();


                List<StorageViewModel> storageViewModels1 = new List<StorageViewModel>();
                storageViewModels1 = storageViewListModel.storageViewModels;

                storageViewModels1[0].CreatedBy = Convert.ToInt32(Session["UserId"]);
                storageViewModels1[1].CreatedBy = Convert.ToInt32(Session["UserId"]);
                storageViewModels1[0].uniques = value;
                storageViewModels1[1].uniques = value;
                var result = webServices.Post(storageViewModels1, "Storage/StorageAdd");
                if (result.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                   
                    int k = (new JavaScriptSerializer()).Deserialize<int>(result.Data);
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception)
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }

           
        }

        public ActionResult Edit(int Id)
        {
           
            try
            {

                StorageViewModel.Id = Id;
                

                var result = webServices.Post(StorageViewModel, "Storage/Edit");

                if (result.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    if (result.Data != "[]")
                    {
                        storageViewModels = (new JavaScriptSerializer().Deserialize<List<StorageViewModel>>(result.Data.ToString()));
                    }
                }
                ProductController productController = new ProductController();
                SiteController siteController = new SiteController();
                VehicleController vehicleController = new VehicleController();
                AwfVehicleController awfVehicleController = new AwfVehicleController();
                int CompanyId = Convert.ToInt32(Session["CompanyId"]);


                ViewBag.AdminVehicles = awfVehicleController.AdminVehicles();
                ViewBag.Vehicles = vehicleController.Vehicles();
                ViewBag.Sites = siteController.SitesList(CompanyId);
                ViewBag.Products = productController.Products();
                return View(storageViewModels);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
