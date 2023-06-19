using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
// takes off CS8618 warning
#pragma warning disable CS8618

namespace bookAPI.Models
{
    public class Book
    {
        [Key]
        public Guid Id { get; set; }

/*        [Required]
        [ForeignKey(nameof(User))] // required
        public Guid UserId { get; set; }
*/
        public User User { get; set; }

        [Required]
        public string Name { get; set; } 
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; } 

        public string? ImageURL { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; } 
        
        public Book()
        {
            this.Id = Guid.NewGuid();
            this.CreatedAt = DateTime.Now;
            this.UpdatedAt = this.CreatedAt;

        }
    }

}

namespace bookAPI.Models
{
    public class CreateBook
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

    }

    public class CreateBookValidator: AbstractValidator<CreateBook>
    {
        public CreateBookValidator()
        {
            RuleFor(b => b.Name).NotEmpty().WithMessage("Name is required.");

            RuleFor(b => b.Title).NotEmpty().WithMessage("Title is required.");

            RuleFor(u => u.Description).NotEmpty().WithMessage("Description is required.");
        }
    }
}

namespace bookAPI.Models
{
    public class BookImageModel
    {
        public IFormFile BookImage { get; set; }
    }

}
