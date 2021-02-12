﻿using Blog.Models;
using Blog.Models.Comments;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Blog.ViewModels;
using Blog.Helpers;

namespace Blog.Data.Repository
{
    public class Repository : IRepository
    {
        private readonly AppDbContext _ctx;

        public Repository(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        public void AddPost(Post post)
        {
            _ctx.Posts.Add(post);
        }

        public List<Post> GetAllPosts()
        {
            return _ctx.Posts.ToList();
        }
 
        public IndexViewModel GetAllPosts(
            int pageNumber,
            string category,
            string search)
        {
            Func<Post, bool> InCategory = (post) => { return post.Category.ToLower().Equals(category.ToLower()); };

            int pageSize = 5;
            int skipAmount = pageSize * (pageNumber - 1);
            var query = _ctx.Posts.AsNoTracking().AsQueryable();

            if (!String.IsNullOrEmpty(category))
                query = query.Where(x => x.Category.ToLower().Equals(category.ToLower()));
            if (!String.IsNullOrEmpty(search))
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{search}%") || EF.Functions.Like(x.Body, $"%{search}%")
                || EF.Functions.Like(x.Description, $"%{search}%"));
            int postsCount = query.Count();
            int pageCount = (int)Math.Ceiling((double)postsCount / pageSize);

            return new IndexViewModel
            {
                PageNumber = pageNumber,
                PageCount = pageCount,
                NextPage = postsCount > skipAmount + pageSize,
                Pages = PageHelper.PageNumbers(pageNumber, pageCount).ToList(),
                Category = category,
                Search = search,
                Posts = query
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .ToList()
            };

        }
        //private IEnumerable<int> PageNumbers(int pageNumber, int pageCount)
        //{
        //    if(pageCount <= 5)
        //    {
        //        for (int i = 1; i <= pageCount; i++)
        //        {
        //            yield return i;
        //        }
        //    }
        //    else
        //    {
        //        int midPoint = pageNumber < 3 ? 3 : pageNumber >= pageCount - 2 ? pageCount - 2
        //  : pageNumber;

        //        int lowerBound = midPoint  - 2;
        //        int upperBound = midPoint  + 2;


        //        if (lowerBound != 1)
        //        {
        //            yield return 1;
        //            if (lowerBound - 1 > 1)
        //            {
        //                yield return -1;
        //            }
        //        }
        //        for (int i = midPoint - 2; i <= upperBound; i++)
        //        {
        //            yield return i;

        //        }
        //        if (upperBound != pageCount)
        //        {
        //            if (pageCount - upperBound > 1)
        //            {
        //                yield return -1;
        //            }
        //            yield return pageCount;
        //        }
        //    }
                
 
        //}
        public Post GetPost(int id)
        {
            return _ctx.Posts
                .IncludeCore(p => p.MainComments)
                .ThenInclude(mc => mc.SubComents)
                .FirstOrDefault(x => x.Id == id);

        }

        public void RemovePost(int id)
        {
            _ctx.Posts.Remove(GetPost(id));
        }

     
        public void UpdatePost(Post post)
        {
            _ctx.Posts.Update(post);
        }
        public async Task<bool> SaveChangesAsync()
        {
           if(await _ctx.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public void AddSubComment(SubComment comment)
        {
            _ctx.SubComments.Add(comment);
        }

    }
}
