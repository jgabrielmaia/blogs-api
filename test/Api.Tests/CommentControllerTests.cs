using System;
using System.Collections.Generic;
using System.Linq;
using Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model;
using Repository;

namespace Api.Tests
{
    public class CommentControllerTests
    {
        private readonly CommentRepository _commentRepository;

        private readonly List<Comment> _expectedComments = new List<Comment>
        {
            new Comment
            {
                Id = Guid.NewGuid(),
                PostId = Guid.NewGuid(),
                Content = "First comment",
                Author = "John Doe",
                CreationDate = DateTime.Now
            },
            new Comment
            {
                Id = Guid.NewGuid(),
                PostId = Guid.NewGuid(),
                Content = "Second comment",
                Author = "Jane Smith",
                CreationDate = DateTime.Now
            },
        };

        public CommentControllerTests()
        {
            _commentRepository = SetupCommentRepository();
        }

        public CommentRepository SetupCommentRepository()
        {
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var blogContextMock = new Mock<BlogContext>(options);
            var commentRepositoryMock = new Mock<CommentRepository>(blogContextMock.Object);

            commentRepositoryMock.Setup(repo => repo.GetAll()).Returns(_expectedComments);
            commentRepositoryMock.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns((Guid id) => _expectedComments.FirstOrDefault(c => c.Id == id));
            commentRepositoryMock.Setup(repo => repo.Create(It.IsAny<Comment>())).Returns((Comment comment) =>
            {
                _expectedComments.Add(comment);
                return comment;
            });
            commentRepositoryMock.Setup(repo => repo.Update(It.IsAny<Comment>())).Returns((Comment comment) => comment);
            commentRepositoryMock.Setup(repo => repo.Delete(It.IsAny<Guid>())).Returns((Guid id) =>
            {
                var comment = _expectedComments.SingleOrDefault(c => c.Id == id);
                if (comment != null)
                {
                    _expectedComments.Remove(comment);
                    return true;
                }
                return false;
            });
            commentRepositoryMock.Setup(repo => repo.GetByPostId(It.IsAny<Guid>())).Returns((Guid postId) => _expectedComments.Where(c => c.PostId == postId));

            return commentRepositoryMock.Object;
        }

        [Fact]
        public void Given_GetAll_When_Invoked_Then_Returns_Existing_Comments()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<CommentController>>();

            // Act
            var actual = new CommentController(loggerMock, _commentRepository).GetAll();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(actual.Result);
            Assert.Equal(_expectedComments, okObjectResult.Value);
        }

        [Fact]
        public void Given_Get_When_Existing_Comment_Exists_Then_Returns_Comment()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<CommentController>>();
            var commentId = _expectedComments[0].Id.Value;

            // Act
            var actual = new CommentController(loggerMock, _commentRepository).Get(commentId);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(actual.Result);
            var comment = Assert.IsType<Comment>(okObjectResult.Value);
            Assert.Equal(_expectedComments[0], comment);
        }

        [Fact]
        public void Given_Get_When_Comment_Not_Found_Then_Returns_NotFound()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<CommentController>>();
            var commentId = Guid.NewGuid();

            // Act
            var actual = new CommentController(loggerMock, _commentRepository).Get(commentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(actual.Result);
        }

        [Fact]
        public void Given_Post_When_Invoked_Then_Creates_New_Comment()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<CommentController>>();
            var newComment = new Comment
            {
                Id = Guid.NewGuid(),
                PostId = Guid.NewGuid(),
                Content = "New comment",
                Author = "John Smith",
                CreationDate = DateTime.Now
            };

            // Act
            var actual = new CommentController(loggerMock, _commentRepository).Post(newComment);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actual.Result);
            var comment = Assert.IsType<Comment>(createdAtActionResult.Value);
            Assert.Equal(newComment, comment);
        }

        [Fact]
        public void Given_Put_When_Existing_Comment_Exists_Then_Updates_Comment()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<CommentController>>();
            var updatedComment = new Comment
            {
                Id = _expectedComments[0].Id,
                PostId = _expectedComments[0].PostId,
                Content = "Updated comment",
                Author = "Jane Doe",
                CreationDate = DateTime.Now
            };

            // Act
            var actual = new CommentController(loggerMock, _commentRepository).Put(updatedComment.Id.Value, updatedComment);

            // Assert
            var okResult = Assert.IsType<NoContentResult>(actual);
        }

        [Fact]
        public void Given_Put_When_Comment_Id_Does_Not_Match_Then_Returns_BadRequest()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<CommentController>>();
            var commentId = Guid.NewGuid();
            var updatedComment = new Comment
            {
                Id = Guid.NewGuid(),
                PostId = Guid.NewGuid(),
                Content = "Updated comment",
                Author = "Jane Doe",
                CreationDate = DateTime.Now
            };

            // Act
            var actual = new CommentController(loggerMock, _commentRepository).Put(commentId, updatedComment);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(actual);
        }

        [Fact]
        public void Given_Delete_When_Existing_Comment_Exists_Then_Deletes_Comment()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<CommentController>>();
            var commentId = _expectedComments[0].Id.Value;

            // Act
            var actual = new CommentController(loggerMock, _commentRepository).Delete(commentId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(actual);
        }

        [Fact]
        public void Given_Delete_When_Comment_Not_Found_Then_Returns_NotFound()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<CommentController>>();
            var commentId = Guid.NewGuid();

            // Act
            var actual = new CommentController(loggerMock, _commentRepository).Delete(commentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(actual);
        }
    }
}