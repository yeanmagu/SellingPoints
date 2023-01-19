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

using Arkix.Modules.SellingPoints.Components;
using Newtonsoft.Json;
using System;

namespace Arkix.Modules.SellingPoints.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PointViewModel
    {
        public PointViewModel(Components.SellingPoints t)
        {
            Id = t.Id;
            Description = t.Description;
            Status = t.Status;
        }

        public PointViewModel(Components.SellingPoints t, string editUrl)
        {
            Id = t.Id;
            Description = t.Description;
            Status = t.Status;
        }


        public PointViewModel()
        {

        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("ciudad")]
        public string Ciudad { get; set; }

        [JsonProperty("departamento")]
        public string Departamento { get; set; }

        [JsonProperty("telefono")]
        public string Telefono { get; set; }

        [JsonProperty("lat")]
        public decimal Lat { get; set; }

        [JsonProperty("long")]
        public decimal Long { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("Exclusive")]
        public int Exclusive { get; set; }

        [JsonProperty("createdOnDate")]
        public DateTime CreatedOnDate { get; set; }

        [JsonProperty("lastModifiedOnDate")]
        public DateTime LastModifiedOnDate { get; }
    }
}