using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tunr.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
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

        public class RegistrationRequest
        {
            [JsonProperty("email")]
            [EmailAddress]
            public string Email { get; set; }

            [JsonProperty("password")]
            [MinLength(5)]
            public string Password { get; set; }
        }

        public class LoginRequest
        {
            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }
        }

        [Route("Login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await signInManager.PasswordSignInAsync(request.Email, request.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await userManager.FindByEmailAsync(request.Email);
                // TODO: Automapper or some equivalent way of mapping viewmodels
                var userViewModel = new TunrUserViewModel()
                {
                    UserId = user.Id,
                    Email = user.Email
                };
                return Ok(userViewModel);
            }
            else
            {
                // TODO: More granular auth behavior
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
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