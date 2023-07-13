using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Model;


namespace Repository.Interfaces
{
    public interface IPostRepository
    {
        public IEnumerable<Post> GetAll();

        public Post Get(Guid id);

        public Post Create(Post post);

        public Post Update(Post post);

        public bool Delete(Guid id);

    }
}