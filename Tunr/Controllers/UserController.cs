using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tunr.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Tunr.Models.BindingModels;
using Tunr.Models.ViewModels;

namespace Tunr.Controllers
{
    [Route("User")]
    public class UserController : Controller
    {
        private readonly UserManager<TunrUser> userManager;
        private readonly SignInManager<TunrUser> signInManager;

        public UserController(UserManager<TunrUser> userManager, SignInManager<TunrUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [Route("Login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestBindingModel request)
        {
            var result = await signInManager.PasswordSignInAsync(request.Email, request.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await userManager.FindByEmailAsync(request.Email);
                return Ok(new LoginResultViewModel{
                    UserId = user.Id,
                    Email = user.Email
                });
            }
            else
            {
                // TODO: More granular auth behavior
                return Unauthorized();
            }
        }

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            else
            {
                return Ok(new TunrUserViewModel()
                {
                    UserId = user.Id,
                    Email = user.Email,
                });
            }
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestBindingModel request)
        {
            if (ModelState.IsValid)
            {
                // Check for existing email
                var existingUser = await userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest("A user with this email address already exists");
                }
                var newUser = new TunrUser()
                {
                    Id = Guid.NewGuid(),
                    UserName = request.Email,
                    Email = request.Email
                };
                var createResult = await userManager.CreateAsync(newUser, request.Password);
                if (createResult.Succeeded)
                {
                    return Ok();
                }
                return BadRequest(String.Join("\n", createResult.Errors.Select(e => e.Description)));
            }
            return BadRequest(ModelState);
        }
    }
}