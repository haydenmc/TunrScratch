using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tunr.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Tunr.Controllers
{
    [Route("User")]
    public class UserController : Controller
    {
        private readonly UserManager<TunrUser> userManager;

        public UserController(UserManager<TunrUser> userManager)
        {
            this.userManager = userManager;
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

        [Authorize("Bearer")]
        public string Get()
        {
            return "It works!";
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