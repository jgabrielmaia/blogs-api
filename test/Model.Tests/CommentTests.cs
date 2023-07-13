using Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model.Tests;

public class CommentTests
{
    [Fact]
    public void Given_Comment_WhenDataIsValid_ValidationHasNoErrors()
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            Content = "Valid content",
            CreationDate = DateTime.Now
        };
        var validationErrors = new List<ValidationResult>();
        var context = new ValidationContext(post, serviceProvider: null, items: null);

        // Act
        Validator.TryValidateObject(post, context, validationErrors, validateAllProperties: true);

        // Assert
        Assert.Empty(validationErrors);
    }

    [Theory]
    [InlineData(nameof(Post.Id))]
    [InlineData(nameof(Post.Title))]
    [InlineData(nameof(Post.Content))]
    [InlineData(nameof(Post.CreationDate))]
    public void Given_Post_WhenFieldIsAbsent_ValidationHasFieldRequiredError(string fieldName)
    {
        // Arrange
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            Content = "Valid content",
            CreationDate = DateTime.Now
        };
        
        # pragma warning disable CS8602
        var postField = post.GetType().GetProperty(fieldName);
        postField.SetValue(post, null);
        # pragma warning restore CS8602

        var validationErrors = new List<ValidationResult>();
        var context = new ValidationContext(post, serviceProvider: null, items: null);
        var expectedErrorMessage = $"The {fieldName} field is required.";

        // Act
        Validator.TryValidateObject(post, context, validationErrors, validateAllProperties: true);

        // Assert
        Assert.Single(validationErrors);
        var titleErrorMessage = validationErrors[0].ErrorMessage;
        Assert.Equal(expectedErrorMessage, titleErrorMessage);
    }

    [Theory]
    [InlineData(nameof(Post.Title), 30)]
    [InlineData(nameof(Post.Content), 1200)]
    public void Given_Post_WhenTitleIsAbsent_ValidationHasMaxLengthError(string fieldName, int fieldLength)
    {   
        var limitExceededLength = fieldLength + 1;
        var field = new string('*', limitExceededLength);
        // Arrange
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            Content = "Valid content",
            CreationDate = DateTime.Now
        };

        # pragma warning disable CS8602
        var postField = post.GetType().GetProperty(fieldName);
        postField.SetValue(post, field);
        # pragma warning restore CS8602

        var validationErrors = new List<ValidationResult>();
        var context = new ValidationContext(post, serviceProvider: null, items: null);
        var expectedErrorMessage = $"The field {fieldName} must be a string or array type with a maximum length of '{fieldLength}'.";

        // Act
        Validator.TryValidateObject(post, context, validationErrors, validateAllProperties: true);

        // Assert
        Assert.Single(validationErrors);
        var lengthErrorMessage = validationErrors[0].ErrorMessage;
        Assert.Equal(expectedErrorMessage, lengthErrorMessage);
    }
}