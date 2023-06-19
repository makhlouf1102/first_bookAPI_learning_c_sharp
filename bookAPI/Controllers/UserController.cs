using BCrypt.Net;
using bookAPI.Data;
using bookAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace bookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BookDbContext _context;

        public UserController(BookDbContext context)
        {
            this._context = context;
        }

        [HttpGet("all")]
        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpPost("/register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(CreateUser user)
        {
            // validate User information validator
            CreateUserValidator validator = new();
            var validation = validator.Validate(user);

            if(!validation.IsValid)
            {
                var errors = validation.Errors.Select(error => error.ErrorMessage).ToList();
                return BadRequest(errors);
            }


            // check if username exists
            var UserNameExists = await _context.Users.SingleOrDefaultAsync(u => u.UserName == user.UserName);
            if (UserNameExists != null) return Conflict();
            
            // check if email exists
            var EmailExists = await _context.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (EmailExists != null) return Conflict();


            User RegisterUser = new();

            RegisterUser.UserName = user.UserName;
            RegisterUser.Email = user.Email;

            RegisterUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            await _context.Users.AddAsync(RegisterUser);
            await _context.SaveChangesAsync();

            // has to change to 201
            return Ok();
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public async Task<IActionResult> Login(LoginUser model)
        {
            // validate User information validator
            LiginUserValidator validator = new();
            var validation = validator.Validate(model);

            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(error => error.ErrorMessage).ToList();
                return BadRequest(errors);
            }

            // check if user exists
            var User = await _context.Users.SingleOrDefaultAsync(u => u.UserName == model.UserName);
            if (User == null) return Unauthorized();

            bool goodPassword = BCrypt.Net.BCrypt.Verify(model.Password, User.Password);
            if(!goodPassword)
            {
                return Unauthorized();
            }

            return Ok();
        }


    }
}
