using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bouvet.Syndicate.TestProject.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static ActionResult FormConflict(this ControllerBase controllerBase)
        {
            return controllerBase.ConflictProblem(
                "Form already exists",
                "The uploaded form do already exists in the cosmos collection, try update the form instead"
            );
        }

        public static ActionResult FormNotFound(this ControllerBase controllerBase)
        {
            return controllerBase.NotFoundProblem(
                "Form not found",
                "The requested form was not found"
            );
        }

        public static ActionResult FormAlreadySubmittedBadRequest(this ControllerBase controllerBase)
        {
            return controllerBase.GenericBadRequestProblem(
                "Form already submitted",
                "The requested form is already submitted and cannot be changed or resubmitted afterwards"
            );
        }

        public static ActionResult FormSubmitValidationProblem(this ControllerBase controllerBase, ValidationResult result)
        {
            return controllerBase.ValidationProblem(
                detail: "The form that is tried to be submitted are missing the following fields",
                modelStateDictionary: result.ToModelStateDictionary());
        }

        public static ActionResult FileUploadValidationProblem(this ControllerBase controllerBase, ValidationResult result)
        {
            return controllerBase.ValidationProblem(
                detail: "The file upload does not match the requirements",
                modelStateDictionary: result.ToModelStateDictionary());
        }


        public static ActionResult FormMissingFormDetails(this ControllerBase controllerBase)
        {
            return controllerBase.GenericBadRequestProblem(
                "Form missing form details",
                "The requested form is missing all the values, please try update the form with some values"
            );
        }

        public static ActionResult AttachmentNotFound(this ControllerBase controllerBase, string uploadName)
        {
            return controllerBase.NotFoundProblem(
                "Attachment not found",
                $"The requested attachment with name '{uploadName}' was not found in the form"
            );
        }


        public static ActionResult InvalidFileFormatBadRequest(this ControllerBase controllerBase, string contentType)
        {
            var details = new ValidationProblemDetails();
            details.Errors.Add("File", new string[] { $"File format with content type '{contentType}' is not supported" });
            return controllerBase.ValidationProblem(details);
        }

        public static ActionResult InvalidIdBadRequest(this ControllerBase controllerBase, string orgNr)
        {
            return controllerBase.GenericBadRequestProblem(
                "Organaization id not valid",
                $"The organization id {orgNr} is not valid."
            );
        }

        public static ActionResult UnknownStatusBadRequest(this ControllerBase controllerBase, string? errorName = null)
        {
            return controllerBase.GenericBadRequestProblem(
                "Unknown status from other service",
                errorName == null
                    ? "Got a unknown status code from an other service"
                    : $"Got the status {errorName} while processing the request"
            );
        }

        public static ActionResult GenericBadRequestProblem(this ControllerBase controllerBase, string title, string details)
        {
            return controllerBase.ProblemWithStatus(title, details, StatusCodes.Status400BadRequest);
        }

        public static ActionResult NotFoundProblem(this ControllerBase controllerBase, string title, string details)
        {
            return controllerBase.ProblemWithStatus(title, details, StatusCodes.Status404NotFound);
        }

        public static ActionResult ConflictProblem(this ControllerBase controllerBase, string title, string details)
        {
            return controllerBase.ProblemWithStatus(title, details, StatusCodes.Status409Conflict);
        }

        public static ActionResult InternalServerProblem(this ControllerBase controllerBase, string title, string details)
        {
            return controllerBase.ProblemWithStatus(title, details, StatusCodes.Status500InternalServerError);
        }

        private static ActionResult ProblemWithStatus(this ControllerBase controllerBase, string title, string details, int statusCode)
        {
            var problem = controllerBase.Problem(details, null, statusCode, title);
            problem.StatusCode = statusCode;
            return problem;
        }

        public static ModelStateDictionary ToModelStateDictionary(this ValidationResult result)
        {
            var dict = new ModelStateDictionary();
            foreach (var v in result.Errors)
            {
                dict.AddModelError(v.PropertyName, v.ErrorMessage);
            }
            return dict;
        }
    }
}
