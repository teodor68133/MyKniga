namespace MyKniga.Web.Models
{
    using System.Collections.Generic;

    public class AllBooksViewModel
    {
        public IEnumerable<PublisherListingViewModel> Publishers { get; set; }
        public IEnumerable<TagDisplayViewModel> Tags { get; set; }
    }
}