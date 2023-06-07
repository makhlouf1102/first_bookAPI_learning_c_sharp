using System.ComponentModel.DataAnnotations;

namespace bookAPI.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        
        [Required]
        public string Name { get; set; } 
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; } 

        public Type Type { get; set; }

        public string ImageURL { get; set; }

        /*[Required]
        public Guid UserId { get; set; }
        */
        public DateTime Created { get; set; }

        public DateTime Updated { get; set; } 
        
        public Book()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            Title = string.Empty;
            Description = string.Empty;
            ImageURL = string.Empty;
            Type = Type.Other;
            Created = DateTime.Now;
            Updated = Created;

        }
    }

    public enum Type
    {
        Adventure, Classics, Crime, FairyTales, fables, Fantasy, HistoricalFiction, Horror, Humour, Other
    }

}
