using System;
using System.Collections.Generic;

namespace QuickQuiz.Helpers
{
    public class Pager
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public List<int> Pages { get; set; }

        public Pager()
        {
            // Default constructor
        }

        public Pager(int totalItems, int? page, int pageSize = 5)
        {
            TotalItems = totalItems;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((decimal)TotalItems / PageSize);
            CurrentPage = page ?? 1;

            if (CurrentPage < 1)
                CurrentPage = 1;
            else if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            StartPage = CurrentPage - 3;
            EndPage = CurrentPage + 2;

            if (StartPage <= 0)
            {
                EndPage -= (StartPage - 1);
                StartPage = 1;
            }
            else if (EndPage > TotalPages)
            {
                StartPage -= (EndPage - TotalPages);
                EndPage = TotalPages;
            }

            StartIndex = (CurrentPage - 1) * PageSize;
            EndIndex = Math.Min(StartIndex + PageSize - 1, TotalItems - 1);

            Pages = new List<int>();
            for (int i = StartPage; i <= EndPage; i++)
            {
                Pages.Add(i);
            }
        }

        public object GetPagingInfo()
        {
            return new
            {
                totalItems = TotalItems,
                currentPage = CurrentPage,
                pageSize = PageSize,
                totalPages = TotalPages,
                startPage = StartPage,
                endPage = EndPage,
                startIndex = StartIndex,
                endIndex = EndIndex,
                pages = Pages
            };
        }
    }
}