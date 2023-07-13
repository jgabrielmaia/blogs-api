using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Model;
using Repository.Interfaces;

namespace Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly BlogContext _blogContext;

        public PostRepository(BlogContext blogContext)
        {
            _blogContext = blogContext;
        }

        public virtual IEnumerable<Post> GetAll()
        {
            return _blogContext.Posts.ToList();
        }

        public virtual Post Get(Guid id)
        {
            return _blogContext.Posts.Find(id);
        }

        public virtual Post Create(Post post)
        {
            _blogContext.Posts.Add(post);
            _blogContext.SaveChanges();

            return post;
        }

        public virtual Post Update(Post post)
        {
            _blogContext.Entry(post).State = EntityState.Modified;
            _blogContext.SaveChanges();

            return post;
        }

        public virtual bool Delete(Guid id)
        {
            var post = this.Get(id);

            if (post == null)
            {
                return false;
            }

            _blogContext.Posts.Remove(post);
            _blogContext.SaveChanges();

            return true;
        }
    }
}