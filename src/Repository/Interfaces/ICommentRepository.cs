using System.Collections.Generic;
using System;
using Model;


namespace Repository.Interfaces
{
    public interface ICommentRepository
    {
        public IEnumerable<Comment> GetAll();

        public Comment Get(Guid id);

        public Comment Create(Comment post);

        public Comment Update(Comment post);

        public bool Delete(Guid id);

        public IEnumerable<Comment> GetByPostId(Guid postId);

    }
}