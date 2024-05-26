namespace Examensarbete.Models
{
    public class FilterCategory
    {
        public int FilterId { get; set; }
        public Filter Filter { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
