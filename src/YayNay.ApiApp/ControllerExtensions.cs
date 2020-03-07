using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NatMarchand.YayNay.Core.Domain;

namespace NatMarchand.YayNay.ApiApp
{
    [ExcludeFromCodeCoverage]
    internal static class ControllerExtensions
    {
        public static ActionResult ValidationProblem(this ControllerBase controller, ValidationFailureCommandResult commandResult)
        {
            if (commandResult.ValidationErrors.Count > 0)
            {
                var dictionary = new ModelStateDictionary();
                foreach (var (key, errors) in commandResult.ValidationErrors)
                {
                    foreach (var error in errors)
                    {
                        dictionary.AddModelError(key, error);
                    }
                }

                return controller.ValidationProblem(commandResult.Reason, modelStateDictionary: dictionary);
            }

            return controller.Problem(commandResult.Reason, statusCode: StatusCodes.Status400BadRequest);
        }

        public static ActionResult Problem(this ControllerBase controller, FailureCommandResult commandResult)
        {
            return controller.Problem(commandResult.Reason);
        }
    }
}