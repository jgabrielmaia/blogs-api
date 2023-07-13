using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public record Comment
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid? PostId { get; set; }

        [Required]
        [MaxLength(120)]
        public string Content { get; set; }

        [Required]
        [MaxLength(30)]
        public string Author { get; set; }

        [Required]
        public DateTime? CreationDate { get; set; }
    }
}