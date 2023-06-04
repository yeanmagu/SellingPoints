using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Arkix.Modules.SellingPoints.Components;
using Arkix.Modules.SellingPoints.Services.ViewModels;
using DotNetNuke.Common;
using DotNetNuke.Web.Api;
using DotNetNuke.Security;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using DotNetNuke.Collections;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using Arkix.Modules.SellingPoints.Services.Request;
namespace Arkix.Modules.SellingPoints.Services
{
    //[SupportedModules("SellingPoints")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]

    public class PointsController : DnnApiController
    {
        private readonly IItemRepository _repository;

        public PointsController(IItemRepository repository)
        {
            Requires.NotNull(repository);

            this._repository = repository;
        }

        public PointsController() : this(ItemRepository.Instance) { }

        [HttpPost]
        [ActionName("Post")]
        public HttpResponseMessage Post(PointViewModel model)
        {
            Components.SellingPoints newItem = new Components.SellingPoints();
            newItem.Nombre = model.Nombre;  //model.FullName;
            newItem.Description = model.Description;
            newItem.Ciudad = model.Ciudad;
            newItem.Departamento = model.Departamento;
            newItem.Status = "PUBLIC";
            newItem.Telefono = model.Telefono;
            newItem.Lat = model.Lat;
            newItem.Long = model.Long;
            newItem.CreatedOnDate = DateTime.Now;
            newItem.LastModifiedOnDate = DateTime.Now;
            newItem.PortalId = PortalSettings.PortalId;
            newItem.Exclusive = model.Exclusive;
            int itemId = model.Id;
            if (itemId > 0)
            {
                newItem.Id = model.Id;
                _repository.UpdateItem(newItem);
            }
            else
            {
                itemId = _repository.AddItem(newItem);
            }

            if (itemId > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "ErrorCreatedItem");
            }

        }

        [HttpPut]
        public HttpResponseMessage Public(int itemId)
        {
            ItemRepository itemRepository = new ItemRepository();
            Components.SellingPoints item = itemRepository.GetItem(itemId);
            item.Status = (item.Status == "DRAFT") ? "PUBLIC" : "DRAFT";
            itemRepository.UpdateItem(item);

            if (itemId > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "ErrorCreatedItem");
            }

        }

        [HttpPost]
        [ActionName("GetPublic")]
        public HttpResponseMessage GetPublic(GetMapRequest getMapRequest)
        {
            try
            {
                ItemRepository itemRepository = new ItemRepository();
                getMapRequest.PortalId = PortalSettings.PortalId;
                LogHelper.LOG($"Consultando información para los siguientes parametros {JsonConvert.SerializeObject(getMapRequest)}");
                IPagedList<PointsByCity> items = itemRepository.GetPublicItems(getMapRequest);

                return Request.CreateResponse(HttpStatusCode.OK, new { data=items,
                    items.HasNextPage,
                    items.HasPreviousPage,
                    items.IsFirstPage,
                    items.IsLastPage,
                    items.PageCount,
                    items.PageIndex,
                    items.PageSize,
                    items.TotalCount
                });
            }
            catch (Exception ex)
            {
                LogHelper.LOG($"Ha ocurrido un error al consultar los registros error: {ex.Message}, {ex.ToString()}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [ActionName("GetPublicByCity")]
        public HttpResponseMessage GetPublicByCityAndDepartmemt(GetMapRequest getMapRequest)
        {
            try
            {
                ItemRepository itemRepository = new ItemRepository();
                getMapRequest.PortalId = PortalSettings.PortalId;
                LogHelper.LOG($"Consultando información para los siguientes parametros {JsonConvert.SerializeObject(getMapRequest)}");
                IPagedList<Components.SellingPoints> items = itemRepository.GetPublicByCity(getMapRequest);

                return Request.CreateResponse(HttpStatusCode.OK, new { Data=items,
                    items.HasNextPage,
                    items.HasPreviousPage,
                    items.IsFirstPage,
                    items.IsLastPage,
                    items.PageCount,
                    items.PageIndex,
                    items.PageSize,
                    items.TotalCount
                });
            }
            catch (Exception ex)
            {
                LogHelper.LOG($"Ha ocurrido un error al consultar los registros error: {ex.Message}, {ex.ToString()}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [ActionName("GetById")]
        public HttpResponseMessage GetById(int id)
        {
            ItemRepository itemRepository = new ItemRepository();

            Components.SellingPoints items = _repository.GetItem(id);

            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpPost]
        [ActionName("GetPointsAdmin")]
        public HttpResponseMessage GetSellingPoints(SellingPointRequest request)
        {
            try
            {
                ItemRepository itemRepository = new ItemRepository();
                request.PortalId = PortalSettings.PortalId;
                var items = _repository.GetAdminItems(request);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    data = items,
                    items.HasNextPage,
                    items.HasPreviousPage,
                    items.IsFirstPage,
                    items.IsLastPage,
                    items.PageCount,
                    items.PageIndex,
                    items.PageSize,
                    items.TotalCount
                });
            }
            catch (Exception ex)
            {
                LogHelper.LOG($"Ha ocurrido un error al consultar los registros error: {ex.Message}, {ex.ToString()}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [ActionName("AddRecords")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage AddFile()
        {
            var response = string.Empty;
            try
            {
                HttpResponseMessage result = null;
                var httpRequest = HttpContext.Current.Request;
                var filename = string.Empty;
                if (httpRequest.Files.Count > 0)
                {
                    var docfiles = string.Empty;
                    for (int img = 0; img < httpRequest.Files.Count; img++)
                    {
                        var postedFile = httpRequest.Files[img];

                        string root = System.Web.Hosting.HostingEnvironment.MapPath("~/DesktopModules/SellingPoints/Files"); ;

                        String fileExtension = Path.GetExtension(postedFile.FileName).ToLower();
                        filename = $"{Guid.NewGuid().ToString()}{fileExtension}";
                        var filePath = $"{root}\\{filename}";
                        var fileOK = false;

                        String[] allowedExtensions = { ".xls", ".xlsx", ".csv" };
                        for (int i = 0; i < allowedExtensions.Length; i++)
                        {
                            if (fileExtension.ToLower() == allowedExtensions[i])
                            {
                                fileOK = true;
                            }
                        }
                        if (fileOK)
                        {
                            postedFile.SaveAs(filePath);
                            var data = ParseCSV.Parse(filePath, 8, ';');
                            response = SaveDataFromFile(data);
                        }

                    }
                    result = Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string SaveDataFromFile(List<string[]> data)
        {
            var recordsSaved = 0;
            var totalRecords = data.Count - 1;
            try
            {
                string[] row;
                for (int i = 1; i < data.Count; i++)
                {
                    var recordToSave = new Components.SellingPoints();
                    row = data[i];
                    recordToSave.Nombre = row[0];
                    recordToSave.Departamento = row[1];
                    recordToSave.Ciudad = row[2];
                    recordToSave.Telefono = row[3];
                    recordToSave.Description = row[4];
                    recordToSave.Lat = decimal.Parse(row[5]);
                    recordToSave.Long = decimal.Parse(row[6]);
                    recordToSave.Status = "PUBLIC";
                    recordToSave.Exclusive = row[7] == "Exclusivo" ? 1 : 2;
                    recordToSave.CreatedOnDate = DateTime.Now;
                    recordToSave.LastModifiedOnDate = DateTime.Now;
                    recordToSave.PortalId = PortalSettings.PortalId;
                    try
                    {
                        var add = _repository.AddItem(recordToSave);
                        if (add > 0)
                        {
                            recordsSaved++;
                            LogHelper.LOG($"Guardando desde importacion: Registro guardado con radicado nro: {recordToSave.Nombre} data: {JsonConvert.SerializeObject(recordToSave)}");
                        }
                        else
                        {
                            LogHelper.LOG($"Guardando desde importacion: Ha ocurrido un error al tratar de guardar la información en la fila: {i} data: {JsonConvert.SerializeObject(row)}");
                        }
                    }
                    catch (Exception e)
                    {
                        LogHelper.LOG($"Guardando desde importacion: Ha ocurrido un error al tratar de guardar la información  data: {JsonConvert.SerializeObject(e)}");
                    }

                }
                return $"{recordsSaved} registros guardados de {totalRecords} subidos";
            }
            catch (Exception ex)
            {
                LogHelper.LOG($"Guardando desde importacion: Ha ocurrido un error al tratar de guardar la información  data: {JsonConvert.SerializeObject(ex)}");
                return $"{recordsSaved} registros guardados de {totalRecords} subidos";
            }
        }

    }
}
