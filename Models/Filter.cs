namespace Examensarbete.Models
{
    public class Filter
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<FilterCategory> FilterCategories { get; set; }
    }
}
