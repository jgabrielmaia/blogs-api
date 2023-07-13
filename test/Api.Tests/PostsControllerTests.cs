using System;
using System.Collections.Generic;
using System.Linq;
using Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model;
using Repository;
using Xunit;

namespace Api.Tests
{
    public class PostControllerTests
    {
        private readonly PostRepository _postRepository;
        private readonly CommentRepository _commentRepository;

        private static Guid _commonPostId = Guid.NewGuid();

        private readonly List<Post> _expectedPosts = new List<Post>
        {
            new Post
            {
                Id = _commonPostId,
                Title = "First post",
                Content = "First post content",
                CreationDate = DateTime.Now
            },
            new Post
            {
                Id = Guid.NewGuid(),
                Title = "Second post",
                Content = "Second post content",
                CreationDate = DateTime.Now
            }
        };

        private readonly List<Comment> _expectedComments = new List<Comment>
        {
            new Comment
            {
                Id = Guid.NewGuid(),
                PostId = _commonPostId,
                Content = "First comment",
                Author = "John Doe",
                CreationDate = DateTime.Now
            },
            new Comment
            {
                Id = Guid.NewGuid(),
                PostId = _commonPostId,
                Content = "Second comment",
                Author = "Jane Smith",
                CreationDate = DateTime.Now
            },
        };

        public PostControllerTests()
        {
            _postRepository = SetupPostRepository();
            _commentRepository = SetupCommentRepository();
        }

        public PostRepository SetupPostRepository()
        {
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var blogContextMock = new Mock<BlogContext>(options);
            var postRepositoryMock = new Mock<PostRepository>(blogContextMock.Object);

            postRepositoryMock.Setup(repo => repo.GetAll()).Returns(_expectedPosts);
            postRepositoryMock.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns((Guid id) => _expectedPosts.FirstOrDefault(p => p.Id == id));
            postRepositoryMock.Setup(repo => repo.Create(It.IsAny<Post>())).Returns((Post post) =>
            {
                _expectedPosts.Add(post);
                return post;
            });
            postRepositoryMock.Setup(repo => repo.Update(It.IsAny<Post>())).Returns((Post post) => post);
            postRepositoryMock.Setup(repo => repo.Delete(It.IsAny<Guid>())).Returns((Guid id) =>
            {
                var post = _expectedPosts.SingleOrDefault(p => p.Id == id);
                if (post != null)
                {
                    _expectedPosts.Remove(post);
                    return true;
                }
                return false;
            });

            return postRepositoryMock.Object;
        }

        public CommentRepository SetupCommentRepository()
        {
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var blogContextMock = new Mock<BlogContext>(options);
            var commentRepositoryMock = new Mock<CommentRepository>(blogContextMock.Object);

            commentRepositoryMock.Setup(repo => repo.GetByPostId(_commonPostId)).Returns(_expectedComments);

            return commentRepositoryMock.Object;
        }

        [Fact]
        public void Given_GetAll_When_Invoked_Then_Returns_Existing_Posts()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<PostController>>();

            // Act
            var actual = new PostController(loggerMock, _commentRepository, _postRepository).GetAll();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(actual.Result);
            var posts = Assert.IsType<List<Post>>(okObjectResult.Value);
            Assert.Equal(_expectedPosts, posts);
        }

        [Fact]
        public void Given_Get_When_Existing_Post_Exists_Then_Returns_Post()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<PostController>>();
            var postId = _expectedPosts[0].Id.Value;

            // Act
            var actual = new PostController(loggerMock, _commentRepository, _postRepository).Get(postId);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(actual.Result);
            var post = Assert.IsType<Post>(okObjectResult.Value);
            Assert.Equal(_expectedPosts[0], post);
        }

        [Fact]
        public void Given_Get_When_Post_Not_Found_Then_Returns_NotFound()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<PostController>>();
            var postId = Guid.NewGuid();

            // Act
            var actual = new PostController(loggerMock, _commentRepository, _postRepository).Get(postId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(actual.Result);
        }

        [Fact]
        public void Given_Post_When_Invoked_Then_Creates_New_Post()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<PostController>>();
            var newPost = new Post
            {
                Id = Guid.NewGuid(),
                Title = "New post",
                Content = "New post content",
                CreationDate = DateTime.Now
            };

            // Act
            var actual = new PostController(loggerMock, _commentRepository, _postRepository).Post(newPost);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actual.Result);
            var post = Assert.IsType<Post>(createdAtActionResult.Value);
            Assert.Equal(newPost, post);
        }

        [Fact]
        public void Given_Put_When_Existing_Post_Exists_Then_Updates_Post()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<PostController>>();
            var postId = _expectedPosts[0].Id.Value;
            var updatedPost = new Post
            {
                Id = _expectedPosts[0].Id,
                Title = "Updated post",
                Content = "Updated post content",
                CreationDate = DateTime.Now
            };

            // Act
            var actual = new PostController(loggerMock, _commentRepository, _postRepository).Put(postId, updatedPost);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(actual);
        }

        [Fact]
        public void Given_Put_When_Post_Id_Does_Not_Match_Then_Returns_BadRequest()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<PostController>>();
            var postId = Guid.NewGuid();
            var updatedPost = new Post
            {
                Id = Guid.NewGuid(),
                Title = "Updated post",
                Content = "Updated post content",
                CreationDate = DateTime.Now
            };

            // Act
            var actual = new PostController(loggerMock, _commentRepository, _postRepository).Put(postId, updatedPost);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(actual);
        }

        [Fact]
        public void Given_Delete_When_Existing_Post_Exists_Then_Deletes_Post()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<PostController>>();
            var postId = _expectedPosts[0].Id.Value;

            // Act
            var actual = new PostController(loggerMock, _commentRepository, _postRepository).Delete(postId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(actual);
        }

        [Fact]
        public void Given_Delete_When_Post_Not_Found_Then_Returns_NotFound()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<PostController>>();
            var postId = Guid.NewGuid();

            // Act
            var actual = new PostController(loggerMock, _commentRepository, _postRepository).Delete(postId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(actual);
        }

        [Fact]
        public void Given_GetComments_When_Existing_Comments_Exist_Then_Returns_Comments()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<PostController>>();
            var postId = _commonPostId;

            // Act
            var actual = new PostController(loggerMock, _commentRepository, _postRepository).GetComments(postId);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(actual.Result);
            var comments = Assert.IsType<List<Comment>>(okObjectResult.Value);
            Assert.Equal(_expectedComments, comments);
        }

        [Fact]
        public void Given_GetComments_When_No_Comments_Exist_Then_Returns_NotFound()
        {
            // Arrange
            var loggerMock = Mock.Of<ILogger<PostController>>();
            var postId = Guid.NewGuid();

            // Act
            var actual = new PostController(loggerMock, _commentRepository, _postRepository).GetComments(postId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(actual.Result);
        }
    }
}