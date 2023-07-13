using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Xunit;

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
            PostId = Guid.NewGuid(),
            Author = "Valid Author",
            Content = "Valid content",
            CreationDate = DateTime.Now
        };
        var validationErrors = new List<ValidationResult>();
        var context = new ValidationContext(comment, serviceProvider: null, items: null);

        // Act
        Validator.TryValidateObject(comment, context, validationErrors, validateAllProperties: true);

        // Assert
        Assert.Empty(validationErrors);
    }

    [Theory]
    [InlineData(nameof(Comment.PostId))]
    [InlineData(nameof(Comment.Author))]
    [InlineData(nameof(Comment.Content))]
    [InlineData(nameof(Comment.CreationDate))]
    public void Given_Post_WhenFieldIsAbsent_ValidationHasFieldRequiredError(string fieldName)
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = Guid.NewGuid(),
            Author = "Valid Author",
            Content = "Valid content",
            CreationDate = DateTime.Now
        };

#pragma warning disable CS8602
        var postField = comment.GetType().GetProperty(fieldName);
        postField.SetValue(comment, null);
#pragma warning restore CS8602

        var validationErrors = new List<ValidationResult>();
        var context = new ValidationContext(comment, serviceProvider: null, items: null);
        var expectedErrorMessage = $"The {fieldName} field is required.";

        // Act
        Validator.TryValidateObject(comment, context, validationErrors, validateAllProperties: true);

        // Assert
        Assert.Single(validationErrors);
        var titleErrorMessage = validationErrors[0].ErrorMessage;
        Assert.Equal(expectedErrorMessage, titleErrorMessage);
    }

    [Theory]
    [InlineData(nameof(Comment.Author), 30)]
    [InlineData(nameof(Comment.Content), 120)]
    public void Given_Post_WhenTitleIsAbsent_ValidationHasMaxLengthError(string fieldName, int fieldLength)
    {
        var limitExceededLength = fieldLength + 1;
        var field = new string('*', limitExceededLength);
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = Guid.NewGuid(),
            Author = "Valid Author",
            Content = "Valid content",
            CreationDate = DateTime.Now
        };

#pragma warning disable CS8602
        var postField = comment.GetType().GetProperty(fieldName);
        postField.SetValue(comment, field);
#pragma warning restore CS8602

        var validationErrors = new List<ValidationResult>();
        var context = new ValidationContext(comment, serviceProvider: null, items: null);
        var expectedErrorMessage = $"The field {fieldName} must be a string or array type with a maximum length of '{fieldLength}'.";

        // Act
        Validator.TryValidateObject(comment, context, validationErrors, validateAllProperties: true);

        // Assert
        Assert.Single(validationErrors);
        var lengthErrorMessage = validationErrors[0].ErrorMessage;
        Assert.Equal(expectedErrorMessage, lengthErrorMessage);
    }
}