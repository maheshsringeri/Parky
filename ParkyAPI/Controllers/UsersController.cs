using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ParkyAPI.Repository.IRepository;
using ParkyAPI.Models;

namespace ParkyAPI.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            this._userRepo = userRepo;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticationModel model)
        {
            var user = _userRepo.Authenticate(model.Username, model.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            user.Password = "";

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthenticationModel model)
        {
            var IsUserExists = _userRepo.IsUniqueUser(model.Username);

            if (!IsUserExists)
            {
                return BadRequest(new { messege = "Username already exists" });
            }

            var user = _userRepo.Register(model.Username, model.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Error while registering." });
            }

            return Ok();
        }
    }
}
