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

        [HttpGet]
        public ActionResult<IEnumerable<Post>> GetAll()
        {
            return Ok(_postRepository.GetAll());
        }

        [HttpGet("{id:guid}")]
        [PostExistsActionFilterAttribute]
        public ActionResult<Post> Get([FromRoute] Guid id)
        {
            var post = _postRepository.Get(id);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpPost]
        public ActionResult<Post> Post([FromBody] Post post)
        {
            var createdPost = _postRepository.Create(post);

            return CreatedAtAction(nameof(Get), new { id = createdPost.Id }, createdPost);
        }

        [HttpPut("{id:guid}")]
        [PostExistsActionFilterAttribute]
        public IActionResult Put([FromRoute] Guid id, [FromBody] Post post)
        {
            if (post.Id != id)
            {
                return BadRequest();
            }

            _postRepository.Update(post);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [PostExistsActionFilterAttribute]
        public IActionResult Delete([FromRoute] Guid id)
        {
            var was_deleted = _postRepository.Delete(id);

            if (was_deleted)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpGet("{id:guid}/comments")]
        [PostExistsActionFilterAttribute]
        public ActionResult<IEnumerable<Comment>> GetComments([FromRoute] Guid id)
        {
            var comments = _commentRepository.GetByPostId(id);

            if (comments.Any())
            {
                return Ok(comments);
            }

            return NotFound();
        }
    }
}