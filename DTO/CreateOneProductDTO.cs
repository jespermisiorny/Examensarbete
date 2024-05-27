namespace Examensarbete.DTO
{
    public class CreateOneProductDTO
    {
        public int ProductsCreated { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success => string.IsNullOrEmpty(ErrorMessage);
    }
}