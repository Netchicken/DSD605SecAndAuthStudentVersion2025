using System.ComponentModel.DataAnnotations;

namespace RolesForAssessment.Models
{
    public class Movie
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string Overview { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public decimal Price { get; set; }

    }
}
