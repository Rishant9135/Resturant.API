using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceAPI.Common.Models
{
    public class TokenValidationRequest
    {
        public long UserId { get; set; }
        public string Token { get; set; }
    }
}
