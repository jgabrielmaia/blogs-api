using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public record Post
    {
        [Key]
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1200)]
        public string Content { get; set; }

        [Required]
        public DateTime? CreationDate { get; set; }

        public virtual IEnumerable<Comment> Comments { get; set; }
    }
}