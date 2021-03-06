﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JoyOI.Blog.Models
{
    public class Post : IConvertible<PostViewModel>
    {
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        [MaxLength(256)]
        public string Url { get; set; }

        [MaxLength(128)]
        public string Title { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }
        
        public DateTime Time { get; set; }

        public bool IsPage { get; set; }

        public string ProblemId { get; set; }

        public string ProblemTitle { get; set; }

        [ForeignKey("Catalog")]
        public Guid? CatalogId { get; set; }

        public virtual Catalog Catalog { get; set; }

        public virtual ICollection<PostTag> Tags { get; set; } = new List<PostTag>();

        PostViewModel IConvertible<PostViewModel>.ToType()
        {
            return new PostViewModel
            {
                Id = Id,
                Summary = Summary,
                Catalog = Catalog,
                CatalogId = CatalogId,
                Tags = Tags.ToList(),
                Time = Time,
                Title = Title,
                Url = Url,
                ProblemId = ProblemId,
                ProblemTitle = ProblemTitle
            };
        }
    }
}
