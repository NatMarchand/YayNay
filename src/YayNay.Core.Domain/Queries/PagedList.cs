using System;
using System.Collections.Generic;

namespace NatMarchand.YayNay.Core.Domain.Queries
{
    public class PagedList<T>
    {
        public IReadOnlyList<T> Values { get; }
        public Paging? Paging { get; }

        public PagedList(IReadOnlyList<T> values, Paging? paging = default)
        {
            Values = values;
            Paging = paging;
        }
        
        public static readonly PagedList<T> Empty = new PagedList<T>(Array.Empty<T>()); 
    }

    public class Paging
    {
        public int CurrentPage { get; }
        public int TotalPages { get; }

        public Paging(int currentPage, int totalPages)
        {
            CurrentPage = currentPage >= 0 && currentPage < totalPages ? currentPage : throw new ArgumentOutOfRangeException(nameof(currentPage));
            TotalPages = totalPages > 0 ? totalPages : throw new ArgumentOutOfRangeException(nameof(totalPages));
        }
    }
}
