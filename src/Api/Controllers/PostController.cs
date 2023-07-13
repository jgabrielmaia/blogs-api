using System;
using System.Collections.Generic;
using System.Linq;
using Api.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model;
using Repository.Interfaces;

namespace Api.Controllers
{
    [ApiController]
    [Route("posts")]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;

        public PostController(
            ILogger<PostController> logger,
            ICommentRepository commentRepository,
            IPostRepository postRepository)
        {
            _logger = logger;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
        }

        /// <summary>
        /// Gets all posts.
        /// </summary>
        /// <returns>List of posts.</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Post>> GetAll()
        {
            return Ok(_postRepository.GetAll());
        }

        /// <summary>
        /// Gets a post by its ID.
        /// </summary>
        /// <param name="id">The ID of the post.</param>
        /// <returns>The post with the specified ID.</returns>
        [HttpGet("{id:guid}")]
        [PostExistsActionFilter]
        public ActionResult<Post> Get(Guid id)
        {
            var post = _postRepository.Get(id);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        /// <summary>
        /// Creates a new post.
        /// </summary>
        /// <param name="post">The post to create.</param>
        /// <returns>The newly created post.</returns>
        [HttpPost]
        public ActionResult<Post> Post([FromBody] Post post)
        {
            var createdPost = _postRepository.Create(post);

            return CreatedAtAction(nameof(Get), new { id = createdPost.Id }, createdPost);
        }

        /// <summary>
        /// Updates an existing post.
        /// </summary>
        /// <param name="id">The ID of the post to update.</param>
        /// <param name="post">The updated post.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id:guid}")]
        [PostExistsActionFilter]
        public IActionResult Put(Guid id, [FromBody] Post post)
        {
            if (post.Id != id)
            {
                return BadRequest();
            }

            _postRepository.Update(post);

            return NoContent();
        }

        /// <summary>
        /// Deletes a post by its ID.
        /// </summary>
        /// <param name="id">The ID of the post to delete.</param>
        /// <returns>No content if the post was deleted; otherwise, returns not found.</returns>
        [HttpDelete("{id:guid}")]
        [PostExistsActionFilter]
        public IActionResult Delete(Guid id)
        {
            var wasDeleted = _postRepository.Delete(id);

            if (wasDeleted)
            {
                return NoContent();
            }

            return NotFound();
        }

        /// <summary>
        /// Gets all comments for a specific post.
        /// </summary>
        /// <param name="id">The ID of the post.</param>
        /// <returns>List of comments for the post.</returns>
        [HttpGet("{id:guid}/comments")]
        [PostExistsActionFilter]
        public ActionResult<IEnumerable<Comment>> GetComments(Guid id)
        {
            return Ok(_commentRepository.GetByPostId(id));
        }
    }
}
