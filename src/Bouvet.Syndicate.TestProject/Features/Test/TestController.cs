using Microsoft.AspNetCore.Mvc;

namespace Bouvet.Syndicate.TestProject.Features.Test
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public TestController()
        {
        }

        [HttpGet("upload")]
        public ActionResult<string> SimpleFileUploadPage()
        {
            // Test code for testing file upload since nswag did not work
            var s = $@"
<html>
<head>
</head>
<body>
<form method='post' enctype='multipart/form-data' action='/api/attachment/1/1'>
<input type='file' name='file'>
<br>
<input type='submit' value='Submit Form'>
</form>
</body>
</html>
";
            return new ContentResult() { Content = s, ContentType = "text/html", StatusCode = 200 };
        }

        [HttpGet("auth-test")]

        public ActionResult<string> CheckAuth()
        {
            var user = HttpContext.User;
            return "";
        }
    }
}
