
using System.Net;

namespace Inventory.Domain.Common.Results.Errors
{
    public class Error
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }

        private Error() { }

        public static Error CreateInstance(string code, string description, HttpStatusCode httpStatusCode)
        {
            return new Error
            {
                Code = code,
                Description = description,
                HttpStatusCode = httpStatusCode
            };
        }
    }
}
