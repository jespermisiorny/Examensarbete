namespace Examensarbete.DTO
{
    public class CreateIncompleteProductsResultDTO
    {
        public int ProductsCreated { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success => string.IsNullOrEmpty(ErrorMessage);
    }
}
