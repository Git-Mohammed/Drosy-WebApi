using System.Collections.Concurrent;
using Drosy.Api.Commons.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Drosy.Api.Filters
{
    public class FluentValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
    {
        // Cache: Stores type -> validator resolution
        private static readonly ConcurrentDictionary<Type, Type> ValidatorTypeCache = new();

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is null) continue;

                var modelType = argument.GetType();

                // Get or add the cached validator type for this model type
                var validatorType = ValidatorTypeCache.GetOrAdd(modelType, type =>
                    typeof(IValidator<>).MakeGenericType(type));

                var validatorObj = serviceProvider.GetService(validatorType);

                if (validatorObj is IValidator validator)
                {
                    var validationContext = new ValidationContext<object>(argument);
                    var result = await validator.ValidateAsync(validationContext);

                    if (!result.IsValid)
                    {
                        var errors = result.Errors
                            .Select(error => new ApiError(error.PropertyName, error.ErrorMessage))
                            .ToList();

                        context.Result = ApiResponseFactory.BadRequestResponse(errors, "Validation failed");
                        return;
                    }

                }
            }

            await next();
        }
    }
}