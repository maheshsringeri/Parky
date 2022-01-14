using System.Collections.Generic;


namespace ParkyWeb.Models.ViewModel
{
    public class IndexVM
    {
        public IEnumerable<NationalPark> nationalParks { get; set; }
        public IEnumerable<Trail> trails { get; set; }
    }
}
