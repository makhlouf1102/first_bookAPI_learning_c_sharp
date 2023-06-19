using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using FluentValidation;
#pragma warning disable CS8618
namespace bookAPI.Models
{
    public class User
    {
        [Key] // primary key
        public Guid Id { get; set; }
        
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Email { get; set; }
        
        public string? PhoneNumber { get; set; }
        
        [Required]
        public string Password { get; set; }

        public string? ImageURL { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public User()
        {
            this.Id = Guid.NewGuid();
            this.CreatedAt = DateTime.Now;
            this.UpdatedAt = this.CreatedAt;
        }
    }

}
namespace bookAPI.Models
{
    public class CreateUser
    {
        
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }

    public class CreateUserValidator : AbstractValidator<CreateUser>
    {
        public CreateUserValidator()
        {
            RuleFor(u => u.UserName).NotEmpty().WithMessage("Username is required.")
                                    .MinimumLength(3).WithMessage("Username must have 3 letters at least");
                                    
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email is required.")
                                .EmailAddress().WithMessage("Invalid email address.");

            RuleFor(u => u.Password).NotEmpty().WithMessage("Password is required.")
                                    .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$").WithMessage("Please ensure that your password meets the following requirements: " +
                                                                                                    "at least 8 characters long, " +
                                                                                                    "contains at least one lowercase letter, " +
                                                                                                    "contains at least one uppercase letter, " +
                                                                                                    "contains at least one digit.");
        }

    }

}

namespace bookAPI.Models
{
    public class LoginUser
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LiginUserValidator : AbstractValidator<LoginUser> 
    {
        public LiginUserValidator()
        {
            RuleFor(u => u.UserName).NotEmpty().WithMessage("Username is required.");

            RuleFor(u => u.Password).NotEmpty().WithMessage("Password is required.");

        }
    }
}
