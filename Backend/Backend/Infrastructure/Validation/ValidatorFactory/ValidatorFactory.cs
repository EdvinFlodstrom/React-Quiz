using FluentValidation;

namespace Backend.Infrastructure.Validation.ValidatorFactory;

public class ValidatorFactory(IServiceProvider serviceProvider) : IValidatorFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public IValidator<T> GetValidator<T>()
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(typeof(T));
        var validatorInstance = _serviceProvider.GetService(validatorType) ?? throw new InvalidOperationException($"No validator registered for type {typeof(T)}.");
        
        return (IValidator<T>)validatorInstance;
    }
}