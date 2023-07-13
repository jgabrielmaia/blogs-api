using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public record Post
    {
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1200)]
        public string Content { get; set; }

        [Required]
        public DateTime? CreationDate { get; set; }
    }
}