using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EsahaPlusApi.Controllers
{
    /*
     * Base controller for all other controllers. Others can get Unit of work object from this controller.
     * */
    [ApiController]
    public class BaseController<T> : ControllerBase where T : BaseController<T>
    {
        //private IUnitOfWork _unitOfWork;
        //public IUnitOfWork UnitOfWork => _unitOfWork ?? (_unitOfWork = (IUnitOfWork)HttpContext.RequestServices.GetService(typeof(IUnitOfWork)));

        //public UserAndTimeInfo UserAndTimeInfo => new UserAndTimeInfo() { Username = User.Identity.Name.Split("\\")[1], UtcTimestamp = TimeManager.GetUtcDatetimeInMilliseconds() };
    }
}