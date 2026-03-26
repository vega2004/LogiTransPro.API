namespace LogiTransPro.API.Models.ViewModels
{
    public class ValidationErrorResponse
    {
        public string Property { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}