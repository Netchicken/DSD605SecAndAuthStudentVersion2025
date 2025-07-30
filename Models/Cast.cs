namespace DSD605SecAndAuthStudentVersion2025.Models
{
    public class Cast
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ScreenName { get; set; }

        public Guid? MovieId { get; set; }


        //Navigation

        public Movie? Movie { get; set; }
    }
}
