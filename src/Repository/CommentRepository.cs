using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Model;
using Repository.Interfaces;

namespace Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly BlogContext _blogContext;

        public CommentRepository(BlogContext blogContext)
        {
            _blogContext = blogContext;
        }

        public virtual IEnumerable<Comment> GetAll()
        {
            return _blogContext.Comments.ToList();
        }

        public virtual Comment Get(Guid id)
        {
            return _blogContext.Comments.Find(id);
        }

        public virtual Comment Create(Comment comment)
        {
            _blogContext.Comments.Add(comment);
            _blogContext.SaveChanges();

            return comment;
        }

        public virtual Comment Update(Comment comment)
        {
            _blogContext.Entry(comment).State = EntityState.Modified;
            _blogContext.SaveChanges();

            return comment;
        }

        public virtual bool Delete(Guid id)
        {
            var comment = this.Get(id);

            if (comment == null)
            {
                return false;
            }

            _blogContext.Comments.Remove(comment);
            _blogContext.SaveChanges();

            return true;
        }

        public virtual IEnumerable<Comment> GetByPostId(Guid postId)
        {
            return _blogContext.Comments.Where(comment => comment.PostId == postId).ToList();
        }
    }
}