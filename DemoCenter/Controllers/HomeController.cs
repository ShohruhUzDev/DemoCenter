using Microsoft.AspNetCore.Mvc;

namespace DemoCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Get() => "Hellooo ...";
    }
}
