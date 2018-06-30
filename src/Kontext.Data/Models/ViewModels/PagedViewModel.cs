using System.Collections.Generic;

namespace Kontext.Data.Models.ViewModels
{
    public class PagedViewModel<TParentViewModel, TChildViewModel> where TParentViewModel : class where TChildViewModel : class
    {
        public PagedViewModel(TParentViewModel viewModel, IEnumerable<TChildViewModel> childViewModels, int totalItemCount, int pageCount, int pageSize, int currentPage)
        {
            ViewModel = viewModel;
            ChildViewModels = childViewModels;
            TotalItemCount = totalItemCount;
            PageCount = pageCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
        }

        public int TotalItemCount { get; protected set; }
        public int PageCount { get; protected set; }
        public int PageSize { get; protected set; }
        public int CurrentPage { get; protected set; }
        public TParentViewModel ViewModel { get; protected set; }
        public IEnumerable<TChildViewModel> ChildViewModels { get; protected set; }

        public int NextPage
        {
            get { return CurrentPage < PageCount ? (CurrentPage + 1) : CurrentPage; }
        }

        public int PreviousPage
        {
            get { return CurrentPage > 1 ? (CurrentPage - 1) : CurrentPage; }
        }

        public int StartPage
        {
            get
            {
                int offset = CurrentPage + 10 - PageCount;
                if (offset > 0)
                    return (CurrentPage - offset) > 0 ? (CurrentPage - offset) : 1;
                else return CurrentPage;
            }
        }

        public int EndPage
        {
            get
            {
                int offset = PageCount - CurrentPage;
                if (offset > 10)
                    return CurrentPage + 10;
                else return PageCount;
            }
        }

        public bool HasNextPage => CurrentPage < PageCount;

        public bool HasPreviousPage => CurrentPage > 1;
    }
}
