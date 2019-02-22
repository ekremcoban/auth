using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EsahaPlusApi.Services
{
    public interface IJsonService
    {
        string SerializeObject(object obj);

        string GetToken(string secret, IEnumerable<Claim> claims);
    }
}
