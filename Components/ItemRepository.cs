/*
' Copyright (c) 2023 Julius2Grow.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Arkix.Modules.SellingPoints.Services;
using Arkix.Modules.SellingPoints.Services.Request;
using Arkix.Modules.SellingPoints.Services.ViewModels;
using DotNetNuke.Collections;
using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Framework;

namespace Arkix.Modules.SellingPoints.Components
{
    public class ItemRepository : ServiceLocator<IItemRepository, ItemRepository>, IItemRepository
    {

        protected override Func<IItemRepository> GetFactory()
        {
            return () => new ItemRepository();
        }

        public int AddItem(SellingPoints t)
        {
            try
            {
                Requires.NotNull(t);

                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<SellingPoints>();
                    rep.Insert(t);
                }
                return t.Id;
            }
            catch (Exception ex)
            {
                if (t.Id > 0)
                {
                    return t.Id;
                }
                throw ex;
            }
        }

        public void DeleteItem(SellingPoints t)
        {
            Requires.NotNull(t);
            Requires.PropertyNotNegative(t, "Id");

            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SellingPoints>();
                rep.Delete(t);
            }
        }

        public void DeleteItem(int itemId, int moduleId)
        {
            Requires.NotNegative("Id", itemId);

            var t = GetItem(itemId);
            DeleteItem(t);
        }

        public SellingPoints GetItem(int itemId)
        {
            Requires.NotNegative("Id", itemId);

            SellingPoints t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SellingPoints>();
                t = rep.GetById(itemId);
            }
            return t;
        }

        public IQueryable<SellingPoints> GetItems(int moduleId)
        {
            Requires.NotNegative("moduleId", moduleId);

            IQueryable<SellingPoints> t = null;

            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SellingPoints>();

                t = rep.Get(moduleId).AsQueryable();
            }

            return t;
        }

        public IQueryable<SellingPoints> GetItems(GetMapRequest getMapRequest)
        {
            try
            {
                IQueryable<SellingPoints> t = null;

                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<SellingPoints>();
                    var condition = $"WHERE Status = '{getMapRequest.Status}'";
                    condition += $" and PortalId = {getMapRequest.PortalId}";

                    if (!string.IsNullOrEmpty(getMapRequest.Department))
                    {
                        condition += $" and Departamento = '{getMapRequest.Department}'";
                    }

                    if (!string.IsNullOrEmpty(getMapRequest.City))
                    {
                        condition += $" and Ciudad = '{getMapRequest.City}'";
                    }

                    if (getMapRequest.Exclusive > 0)
                    {
                        condition += $" and Exclusive = {getMapRequest.Exclusive}";
                    }

                    LogHelper.LOG($"parametros: {condition}");
                    t = rep.Find(condition).AsQueryable();
                }

                return t;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<PointsByCity> GetPublishItemsGroupByCity(GetMapRequest getMapRequest)
        {
            try
            {
                IQueryable<PointsByCity> filteredData = null;

                using (IDataContext ctx = DataContext.Instance())
                {
                    //var rep = ctx.GetRepository<SellingPoints>();

                    var condition = $"WHERE Status = '{getMapRequest.Status}'";
                    condition += $" and PortalId = {getMapRequest.PortalId}";

                    if (!string.IsNullOrEmpty(getMapRequest.Department))
                    {
                        condition += $" and Departamento = '{getMapRequest.Department}'";
                    }

                    if (!string.IsNullOrEmpty(getMapRequest.City))
                    {
                        condition += $" and Ciudad = '{getMapRequest.City}'";
                    }

                    if (getMapRequest.Exclusive > 0)
                    {
                        condition += $" and Exclusive = {getMapRequest.Exclusive}";
                    }
                    filteredData = ctx.ExecuteQuery<PointsByCity>(System.Data.CommandType.Text,
                        @"SELECT Ciudad, Departamento, COUNT(*) AS Total
                          FROM Ax_SellingPoints_SellingPoints "
                          + condition +
                          "GROUP BY Ciudad, Departamento").AsQueryable();
                }

                return filteredData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IQueryable<SellingPoints> GetItems(string searchTerm, int PortalId)
        {
            try
            {
                var t = Enumerable.Empty<SellingPoints>().AsQueryable();
                var condition = "WHERE  ";
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    condition += $"Nombre LIKE '%{searchTerm}%' AND ";
                }

                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<SellingPoints>();
                    condition += $"PortalId = {PortalId}";
                    t = rep.Find(condition).AsQueryable();
                }

                return t;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IQueryable<SellingPoints> GetItems()
        {

            IQueryable<SellingPoints> t = null;

            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SellingPoints>();

                t = rep.Get().AsQueryable();
            }

            return t;
        }


        public PagedList<SellingPoints> GetAdminItems(SellingPointRequest getMapRequest)
        {
            Requires.NotNegative("pageIndex", getMapRequest.PageNumber);

            var t = GetItems(getMapRequest.Search, getMapRequest.PortalId);
            return new PagedList<SellingPoints>(t, getMapRequest.PageNumber, getMapRequest.PageSize);
        }

        public IPagedList<PointsByCity> GetPublicItems(GetMapRequest getMapRequest)
        {
            Requires.NotNegative("pageIndex", getMapRequest.PageNumber);

            var filteredData = GetPublishItemsGroupByCity(getMapRequest);

            return new PagedList<PointsByCity>(filteredData, getMapRequest.PageNumber, getMapRequest.PageSize);
        }

        public void UpdateItem(SellingPoints t)
        {
            try
            {
                Requires.NotNull(t);
                Requires.PropertyNotNegative(t, "Id");

                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<SellingPoints>();
                    rep.Update(t);
                }
            }
            catch (Exception ex)
            {
                if (t.Id > 0)
                {
                    return;
                }
                throw ex;
            }
        }

        public IPagedList<SellingPoints> GetPublicByCity(GetMapRequest getMapRequest)
        {
            try
            {
                IQueryable<SellingPoints> filteredData = null;

                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<SellingPoints>();
                    var condition = $"WHERE Status = '{getMapRequest.Status}'";
                    condition += $" and PortalId = {getMapRequest.PortalId}";

                    if (!string.IsNullOrEmpty(getMapRequest.Department))
                    {
                        condition += $" and Departamento = '{getMapRequest.Department}'";
                    }

                    if (!string.IsNullOrEmpty(getMapRequest.City))
                    {
                        condition += $" and Ciudad = '{getMapRequest.City}'";
                    }
                    filteredData = rep.Find($"{condition}").AsQueryable();

                    return new PagedList<SellingPoints>(filteredData, getMapRequest.PageNumber, getMapRequest.PageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}