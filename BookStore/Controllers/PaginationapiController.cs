using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Web.UI.WebControls;
using BookStore.Models.Pagination;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using Swashbuckle.Swagger.Annotations;

namespace BookStore.Controllers
{

    /// <summary>
    /// Get values
    /// </summary>
    public class PaginationApiController : ApiController
    {
        List<Phone> phones;
        public PaginationApiController()
        {
            phones = new List<Phone>
            {
                new Phone {Id = 1, Model = "Samsung Galaxy III", Producer = "Samsung"},
                new Phone {Id = 2, Model = "Samsung Ace II", Producer = "Samsung"},
                new Phone {Id = 3, Model = "HTC Hero", Producer = "HTC"},
                new Phone {Id = 4, Model = "HTC One S", Producer = "HTC"},
                new Phone {Id = 5, Model = "HTC One X", Producer = "HTC"},
                new Phone {Id = 6, Model = "LG Optimus 3D", Producer = "LG"},
                new Phone {Id = 7, Model = "Nokia N9", Producer = "Nokia"},
                new Phone {Id = 8, Model = "Samsung Galaxy Nexus", Producer = "Samsung"},
                new Phone {Id = 9, Model = "Sony Xperia X10", Producer = "SONY"},
                new Phone {Id = 10, Model = "Samsung Galaxy II", Producer = "Samsung"}
            };
        }


        /// <summary>
        /// Get phones
        /// </summary>
        /// <param name="page">page number</param>
        /// <returns>returns items for page</returns>
        /// <operation-name>get values</operation-name>
        [ResponseType(typeof(IndexViewModel))]
        [ActionName("Get values")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(InternalServerErrorResult),
            Description = "bad data")]
        [SwaggerOperation(Tags = new[] { "Get values" })]
        public IHttpActionResult Get(int page = 1)
        {
            int pageSize = 3;
            IEnumerable<Phone> phonesPerPages = phones.Skip((page - 1) * pageSize).Take(pageSize);
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = phones.Count };
            IndexViewModel ivm = new IndexViewModel { PageInfo = pageInfo, Phones = phonesPerPages };
            return Ok(ivm);
        }
    }
}
