using Core.EsahaPlus.Dtos;

namespace EsahaPlusApi.Services
{
    public interface IAuthService
    {
        UserDto Authenticate(UserDto user); 
    }
}
