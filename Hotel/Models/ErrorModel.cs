namespace Hotel.Models
{
    public class ErrorModel
    {

        public int Code { get; set; }
        public string? Message { get; set; }

        public List <ErrorModel>? Errors { get; set; }
    }
}
