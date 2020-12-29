using DatingApplicationBackEnd.Core.Models;
using DatingApplicationBackEnd.Persistance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext dataContext;

        public BuggyController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var failedQuery = dataContext.Users.Find(-1);
            if (failedQuery == null)
            {
                return NotFound();
            }
            return Ok(failedQuery);
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var failedQuery = dataContext.Users.Find(-1);
            var failedQueryReturn = failedQuery.ToString();
            return failedQueryReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This is bad request");
        }
    }
}
