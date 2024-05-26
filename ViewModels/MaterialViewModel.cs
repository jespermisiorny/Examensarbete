namespace Examensarbete.ViewModels
{
    public class MaterialViewModel
    {
        public int MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public int Percentage { get; set; } // Procentandel av materialet i produkten
        public bool IsSelected { get; set; }

    }
}
