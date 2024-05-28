namespace Examensarbete.DTO
{
    public class CreateProductDTO
    {
        public int ProductsCreated { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success => string.IsNullOrEmpty(ErrorMessage);
    }
}