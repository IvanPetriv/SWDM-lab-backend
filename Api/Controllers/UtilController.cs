using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
public class UtilController : Controller {
    [HttpGet("guid")]
    public ActionResult<string> GetGuid() {
        return Ok(Guid.NewGuid().ToString());
    }
}
