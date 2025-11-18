using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniLibrary.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, StringLength(250)]
        public string Title { get; set; }

        [StringLength(150)]
        public string Author { get; set; }

        public int? Year { get; set; }

        [StringLength(100)]
        public string Genre { get; set; }

        [StringLength(50)]
        public string ISBN { get; set; }

        [Range(0, 5)]
        [Column(TypeName = "decimal")]
        public decimal? Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
