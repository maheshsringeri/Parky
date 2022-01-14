using System.Net.Http;
using ParkyWeb.Repository.IRepository;
using ParkyWeb.Models;
namespace ParkyWeb.Repository
{
    public class NationalParkRepository : Repository<NationalPark>, INationalParkRepository
    {
        private readonly IHttpClientFactory _clientFactory;

        public NationalParkRepository(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            this._clientFactory = clientFactory;
        }
    }
}
