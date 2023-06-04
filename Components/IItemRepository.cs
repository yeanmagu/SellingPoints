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

using System.Linq;
using Arkix.Modules.SellingPoints.Services.Request;
using Arkix.Modules.SellingPoints.Services.ViewModels;
using DotNetNuke.Collections;

namespace Arkix.Modules.SellingPoints.Components
{
    public interface IItemRepository
    {

        int AddItem(SellingPoints t);

        void DeleteItem(int itemId, int moduleId);

        void DeleteItem(SellingPoints t);

        SellingPoints GetItem(int itemId);

        IQueryable<SellingPoints> GetItems(int moduleId);

        IQueryable<SellingPoints> GetItems(GetMapRequest getMapRequest);
        IPagedList<PointsByCity> GetPublicItems(GetMapRequest getMapRequest);
        IPagedList<SellingPoints> GetPublicByCity(GetMapRequest getMapRequest);
        PagedList<SellingPoints> GetAdminItems(SellingPointRequest getMapRequest);

        void UpdateItem(SellingPoints t);
    }
}