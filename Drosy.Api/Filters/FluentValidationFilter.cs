using System.Collections.Concurrent;
using Drosy.Api.Commons.Models;
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
                        // foreach (var error in result.Errors)
                        //     context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        var errors = result.Errors.Select(x => new ApiError(x.PropertyName, x.ErrorMessage)).ToList();
                        context.Result = new BadRequestObjectResult(ApiResponse<string>.Failure(errors, "Validation failed"));
                        return;
                    }
                }
            }

            await next();
        }
    }
}