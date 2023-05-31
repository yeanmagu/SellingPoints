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
using System.Web.Caching;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace Arkix.Modules.SellingPoints.Components
{
    [TableName("Ax_SellingPoints_SellingPoints")]
    //setup the primary key for table
    [PrimaryKey("Id", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Ax_SellingPoints_SellingPoints", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    [Scope("ModuleId")]
    public class SellingPoints
    {
        ///<summary>
        /// The ID of your object with the name of the ItemName
        ///</summary>
        public int Id { get; set; }

        public string Nombre { get; set; }

        ///<summary>
        /// A string with the description of the object
        ///</summary>
        public string Description { get; set; }

        ///<summary>
        /// A string with the description of the object
        ///</summary>
        public string Telefono { get; set; }

        ///<summary>
        /// A string with the description of the object
        ///</summary>
        public decimal Lat { get; set; }

        ///<summary>
        /// An integer with the user id of the assigned user for the object
        ///</summary>
        public decimal Long { get; set; }

        ///<summary>
        /// A string with the description of the object
        ///</summary>
        public string Departamento { get; set; }

        ///<summary>
        /// A string with the description of the object
        ///</summary>
        public string Ciudad { get; set; }

        ///<summary>
        /// A string with the description of the object
        ///</summary>
        public string Status { get; set; }

        public int PortalId { get; set; }
        public int Exclusive { get; set; }

        ///<summary>
        /// The date the object was created
        ///</summary>
        public DateTime CreatedOnDate { get; set; }

        ///<summary>
        /// The date the object was updated
        ///</summary>
        public DateTime LastModifiedOnDate { get; set; }
    }
}
