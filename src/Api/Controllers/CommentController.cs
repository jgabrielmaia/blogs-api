using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model;
using Repository.Interfaces;

namespace Api.Controllers
{
    [ApiController]
    [Route("comments")]
    public class CommentController : ControllerBase
    {
        private readonly ILogger<CommentController> _logger;
        private readonly ICommentRepository _commentRepository;

        public CommentController(ILogger<CommentController> logger, ICommentRepository commentRepository)
        {
            _logger = logger;
            _commentRepository = commentRepository;
        }

        /// <summary>
        /// Gets all comments.
        /// </summary>
        /// <returns>List of comments.</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Comment>> GetAll()
        {
            return Ok(_commentRepository.GetAll());
        }

        /// <summary>
        /// Gets a comment by its ID.
        /// </summary>
        /// <param name="id">The ID of the comment.</param>
        /// <returns>The comment with the specified ID.</returns>
        [HttpGet("{id:guid}")]
        public ActionResult<Comment> Get(Guid id)
        {
            var comment = _commentRepository.Get(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        /// <summary>
        /// Creates a new comment.
        /// </summary>
        /// <param name="comment">The comment to create.</param>
        /// <returns>The newly created comment.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /comments
        ///     {
        ///        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///        "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///        "content": "Sample comment",
        ///        "author": "John Doe",
        ///        "creationDate": "2022-01-01T00:00:00Z"
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created comment.</response>
        /// <response code="400">If the comment is null.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Comment), 201)]
        [ProducesResponseType(400)]
        public ActionResult<Comment> Post([FromBody] Comment comment)
        {
            var createdComment = _commentRepository.Create(comment);

            return CreatedAtAction(nameof(Get), new { id = createdComment.Id }, createdComment);
        }

        /// <summary>
        /// Updates an existing comment.
        /// </summary>
        /// <param name="id">The ID of the comment to update.</param>
        /// <param name="comment">The updated comment.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id:guid}")]
        public IActionResult Put(Guid id, [FromBody] Comment comment)
        {
            if (comment.Id != id)
            {
                return BadRequest();
            }

            _commentRepository.Update(comment);

            return NoContent();
        }

        /// <summary>
        /// Deletes a comment by its ID.
        /// </summary>
        /// <param name="id">The ID of the comment to delete.</param>
        /// <returns>No content if the comment was deleted; otherwise, returns not found.</returns>
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var wasDeleted = _commentRepository.Delete(id);

            if (wasDeleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
