using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Core.Dtos;
using Core.EsahaPlus.Dtos;
using EsahaPlusApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EsahaPlusApi.Controllers
{
    [Route("esaha/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPut("[action]")]
        public IActionResult Authenticate([FromBody]UserDto user)
        {
            var _user = _authService.Authenticate(user);

            if (_user == null)
                return BadRequest(new ErrorDto() { Message = "Kullanıcı adı veya şifre yanlış!" });

            return Ok(_user);

            //if (user.Username == "q" && user.Password == "a")
            //{
            //    user.Password = null;
            //    user.Token = "bad token";
            //    user.FirsName = "Osman";
            //    user.LastName = "Çelebi";
            //    return Ok(user);
            //}
            //else
            //    return NotFound();
        }

        [HttpGet("[action]")]
        public IActionResult Getir()
        {
            var name = User.Identity.Name;
            var user = new UserDto() { FirsName = "Elif", LastName = "Hayal", Username = "ehayal", Token = "secret token - ehayal" };
            var user2 = new UserDto() { FirsName = "Ahmet", LastName = "Polat", Username = "apolat", Token = "secret token - apolat" };

            List<UserDto> lst = new List<UserDto>();
            lst.Add(user);
            lst.Add(user2);

            return Ok(lst);
        }
    }
}