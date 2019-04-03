namespace MyKniga.Web.Models
{
    using System.Collections.Generic;

    public class TagListingViewModel
    {
        public IEnumerable<TagDisplayViewModel> Tags { get; set; }
    }
}