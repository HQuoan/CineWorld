using CineWorld.Services.ReactionAPI.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.ReactionAPI.Extensions
{
    public static class PaginationExtention
    {
        public async static Task<PagedList<T>> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            int totalItems = source.Count();
            List<T> items = await source.Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
            return new PagedList<T> (items, totalItems, pageNumber, pageSize);

        }
    }
}
