using System.Net.Http;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
namespace ParkyWeb.Repository
{
    public class TrailsRepository : Repository<Trail>, ITrailsRepository
    {
        private readonly IHttpClientFactory _clientFactory;

        public TrailsRepository(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            this._clientFactory = clientFactory;
        }
    }
}
