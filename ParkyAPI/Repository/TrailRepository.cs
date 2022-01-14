using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System.Collections.Generic;
using ParkyAPI.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ParkyAPI.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public TrailRepository(ApplicationDBContext context)
        {
            this._dbContext = context;
        }

        public bool CreateTrail(Trail trail)
        {
            _dbContext.Trails.Add(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            _dbContext.Trails.Remove(trail);
            return Save();
        }

        public Trail GetTrail(int id)
        {
            return _dbContext.Trails.Include(x => x.NationalPark).FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Trail> GetTrails()
        {
            return _dbContext.Trails.Include(x => x.NationalPark).OrderBy(x => x.Name).ToList();
        }

        public ICollection<Trail> GetTrailsInNationalPark(int npId)
        {

            return _dbContext.Trails.Include(x => x.NationalPark).Where(x => x.NationalParkId == npId).ToList();
        }

        public bool Save()
        {
            return _dbContext.SaveChanges() >= 0 ? true : false;
        }

        public bool TrailExists(string name)
        {
            return _dbContext.Trails.Any(x => x.Name == name);
        }

        public bool TrailExists(int id)
        {
            return _dbContext.Trails.Any(x => x.Id == id);
        }

        public bool UpdateTrail(Trail trail)
        {
            _dbContext.Trails.Update(trail);
            return Save();
        }
    }
}
