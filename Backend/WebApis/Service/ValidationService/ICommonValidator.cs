namespace WebApis.Service.ValidationService
{
    public class ValidationResult
    {
        public bool IsValid => !Errors.Any();
        public List<string> Errors { get; set; } = new();
    }
    public interface ICommonValidator<T>
    {
        Task<ValidationResult> ValidateAsync(T dto);
    }
}
