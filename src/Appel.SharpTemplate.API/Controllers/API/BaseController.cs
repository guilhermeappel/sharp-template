using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Appel.SharpTemplate.API.Controllers.API;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase { }
