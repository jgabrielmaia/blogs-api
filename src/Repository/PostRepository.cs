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

        public virtual Post Update(Post updatedPost)
        {
            var existingPost = Get(updatedPost.Id.Value);
            if (existingPost != null)
            {
                _blogContext.Entry(existingPost).CurrentValues.SetValues(updatedPost);
                _blogContext.SaveChanges();
            }

            return existingPost;
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