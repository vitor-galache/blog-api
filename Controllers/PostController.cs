using BlogApi.Data;
using BlogApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers;

[ApiController] 
public class PostController : ControllerBase
{
    [HttpGet("v1/posts")]
    public async Task<IActionResult> GetAsync(
        [FromServices] BlogDataContext context,
        [FromQuery] int page = 0,int pageSize = 25)
    {
        var count = await context.Posts.AsNoTracking().CountAsync();
        var posts = await context.Posts.Skip(page * pageSize).Take(pageSize).ToListAsync();
        return Ok(new ResultViewModel<dynamic>(new
        {
            Posts = posts
        }));
    }
    
    
    
    
}
