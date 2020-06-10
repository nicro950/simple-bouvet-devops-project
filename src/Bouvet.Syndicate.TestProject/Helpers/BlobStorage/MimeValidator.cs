using System.Collections.Generic;
using FluentValidation;

namespace Bouvet.Syndicate.TestProject.Helpers.BlobStorage
{
    public static class MimeValidator
    {
        private static readonly Dictionary<string, string> extensionToMimeTypeMap;
        private static readonly Dictionary<string, string> mimeTypeToExtensionMap;
        static MimeValidator()
        {
            mimeTypeToExtensionMap = new Dictionary<string, string>();
            extensionToMimeTypeMap = new Dictionary<string, string>()
            {
                [".pdf"] = "application/pdf",

                [".doc"] = "application/msword",
                [".xls"] = "application/vnd.ms-excel",

                [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                [".odt"] = "application/vnd.oasis.opendocument.text",
                [".ods"] = "application/vnd.oasis.opendocument.spreadsheet",
            };

            foreach (var (key, value) in extensionToMimeTypeMap)
            {
                mimeTypeToExtensionMap[value] = key;
            }
        }

        public static bool IsValidFileExtension(string fileExtension)
        {
            return extensionToMimeTypeMap.ContainsKey(fileExtension);
        }

        public static bool IsValidContentType(string contentType)
        {
            return mimeTypeToExtensionMap.ContainsKey(contentType);
        }

        public static void ValidMimeType<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            ruleBuilder.Custom((contentType, context) =>
            {
                if (contentType == null)
                {
                    context.AddFailure($"Null is not a valid content type");
                }
                else if (!IsValidContentType(contentType))
                {
                    context.AddFailure($"The content type: {contentType} is not supported");
                }
            });
        }
    }
}
