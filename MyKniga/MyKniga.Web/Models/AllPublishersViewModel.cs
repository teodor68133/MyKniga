namespace MyKniga.Web.Models
{
    using System.Collections.Generic;

    public class AllPublishersViewModel
    {
        public IEnumerable<PublisherDetailsViewModel> Publishers { get; set; }
    }
}