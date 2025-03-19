namespace OnlineExam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting(RateLimiterType.Concurrency)]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        //[Authorize(Roles = $"{Role.Admin},{Role.User}")]
        public async Task<ActionResult<PaginatedResponse<Subject>>> GetSubjectsAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var subjects = await _subjectService.GetSubjectsAsync(pageNumber, pageSize);

            return Ok(subjects);
        }
    }
}
