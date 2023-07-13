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

        [HttpGet]
        public ActionResult<IEnumerable<Comment>> GetAll()
        {
            return Ok(_commentRepository.GetAll());
        }

        [HttpGet("{id:guid}")]
        public ActionResult<Comment> Get([FromRoute] Guid id)
        {
            var comment = _commentRepository.Get(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        [HttpPost]
        public ActionResult<Comment> Post([FromBody] Comment comment)
        {
            var createdComment = _commentRepository.Create(comment);

            return CreatedAtAction(nameof(Get), new { id = createdComment.Id }, createdComment);
        }

        [HttpPut("{id:guid}")]
        public IActionResult Put([FromRoute] Guid id, [FromBody] Comment comment)
        {
            if (comment.Id != id)
            {
                return BadRequest();
            }

            _commentRepository.Update(comment);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete([FromRoute] Guid id)
        {
            var was_deleted = _commentRepository.Delete(id);

            if (was_deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}