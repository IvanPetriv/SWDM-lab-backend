using Api.Dtos;
using Api.Extensions;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PostController(
    PostService postService,
    IMapper mapper
) : ControllerBase
{
    /// <summary>
    /// Gets all posts for a specific course.
    /// </summary>
    /// <param name="courseId">Course ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of posts for the course.</returns>
    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<IEnumerable<PostGetDto>>> GetByCourseId(Guid courseId, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        // Check if user has access to the course
        var hasAccess = await postService.UserHasAccessToCourse(courseId, userId.Value, ct);
        if (!hasAccess)
            return Forbid();

        var posts = await postService.GetByCourseId(courseId, ct);
        var postsDto = mapper.Map<IEnumerable<PostGetDto>>(posts);

        return Ok(postsDto);
    }

    /// <summary>
    /// Gets a specific post by ID.
    /// </summary>
    /// <param name="id">Post ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Post details.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PostGetDto>> GetById(Guid id, CancellationToken ct)
    {
        var post = await postService.GetById(id, ct);
        if (post is null)
            return NotFound();

        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        // Check if user has access to the course
        var hasAccess = await postService.UserHasAccessToCourse(post.CourseId, userId.Value, ct);
        if (!hasAccess)
            return Forbid();

        var postDto = mapper.Map<PostGetDto>(post);
        return Ok(postDto);
    }

    /// <summary>
    /// Gets the image for a specific post.
    /// </summary>
    /// <param name="id">Post ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Image file.</returns>
    [HttpGet("{id}/image")]
    public async Task<ActionResult> GetImage(Guid id, CancellationToken ct)
    {
        var post = await postService.GetById(id, ct);
        if (post is null)
            return NotFound();

        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        // Check if user has access to the course
        var hasAccess = await postService.UserHasAccessToCourse(post.CourseId, userId.Value, ct);
        if (!hasAccess)
            return Forbid();

        var imageData = await postService.GetPostImage(id, ct);
        if (imageData is null)
            return NotFound("Post has no image");

        return File(imageData.Value.imageData, imageData.Value.contentType);
    }

    /// <summary>
    /// Creates a new post for a course. Only teachers can create posts.
    /// </summary>
    /// <param name="postDto">Post creation data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created post.</returns>
    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<PostGetDto>> Create([FromForm] PostCreateDto postDto, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        // Check if course exists
        var courseExists = await postService.CourseExists(postDto.CourseId, ct);
        if (!courseExists)
            return NotFound("Course not found");

        // Check if user has access to the course (must be the teacher)
        var hasAccess = await postService.UserHasAccessToCourse(postDto.CourseId, userId.Value, ct);
        if (!hasAccess)
            return Forbid();

        var post = new Post
        {
            CourseId = postDto.CourseId,
            AuthorId = userId.Value,
            Title = postDto.Title,
            TextContent = postDto.TextContent
        };

        // Handle image upload
        if (postDto.Image is not null)
        {
            using var memoryStream = new MemoryStream();
            await postDto.Image.CopyToAsync(memoryStream, ct);
            post.ImageData = memoryStream.ToArray();
            post.ImageContentType = postDto.Image.ContentType;
        }

        var createdPost = await postService.Create(post, ct);
        var resultDto = mapper.Map<PostGetDto>(createdPost);

        return CreatedAtAction(nameof(GetById), new { id = createdPost.Id }, resultDto);
    }

    /// <summary>
    /// Updates an existing post. Only the author can update their post.
    /// </summary>
    /// <param name="id">Post ID.</param>
    /// <param name="postDto">Post update data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult> Update(Guid id, [FromForm] PostUpdateDto postDto, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var existingPost = await postService.GetById(id, ct);
        if (existingPost is null)
            return NotFound();

        // Only the author can update the post
        if (existingPost.AuthorId != userId.Value)
            return Forbid();

        byte[]? imageData = null;
        string? imageContentType = null;

        // Handle image upload
        if (postDto.Image is not null)
        {
            using var memoryStream = new MemoryStream();
            await postDto.Image.CopyToAsync(memoryStream, ct);
            imageData = memoryStream.ToArray();
            imageContentType = postDto.Image.ContentType;
        }

        var updated = await postService.Update(
            id,
            postDto.Title,
            postDto.TextContent,
            imageData,
            imageContentType,
            postDto.RemoveImage ?? false,
            ct
        );

        if (updated is null)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Deletes a post. Only the author can delete their post.
    /// </summary>
    /// <param name="id">Post ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var existingPost = await postService.GetById(id, ct);
        if (existingPost is null)
            return NotFound();

        // Only the author can delete the post
        if (existingPost.AuthorId != userId.Value)
            return Forbid();

        var deleted = await postService.Delete(id, ct);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Gets all posts created by the current user.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of posts by the current user.</returns>
    [HttpGet("my-posts")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<IEnumerable<PostGetDto>>> GetMyPosts(CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var posts = await postService.GetByAuthorId(userId.Value, ct);
        var postsDto = mapper.Map<IEnumerable<PostGetDto>>(posts);

        return Ok(postsDto);
    }
}
