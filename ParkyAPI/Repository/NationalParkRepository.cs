using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System.Collections.Generic;
using ParkyAPI.Data;
using System.Linq;

namespace ParkyAPI.Repository
{
    public class NationalParkRepository : INationalParkRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public NationalParkRepository(ApplicationDBContext dBContext)
        {
            this._dbContext = dBContext;
        }

        public bool CreateNationalPark(NationalPark nationalPark)
        {
            _dbContext.NationalParks.Add(nationalPark);
            return Save();
        }

        public bool DeleteNationalPark(NationalPark nationalPark)
        {
            _dbContext.NationalParks.Remove(nationalPark);
            return Save();
        }

        public NationalPark GetNationalPark(int id)
        {
            return _dbContext.NationalParks.FirstOrDefault(q => q.Id == id);
        }

        public ICollection<NationalPark> GetNationalParks()
        {
            return _dbContext.NationalParks.OrderBy(q => q.Name).ToList();
        }

        public bool NationalParkExists(string name)
        {
            return _dbContext.NationalParks.Any(q => q.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool NationalParkExists(int Id)
        {
            return _dbContext.NationalParks.Any(q => q.Id == Id);
        }

        public bool Save()
        {
            return _dbContext.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateNationalPark(NationalPark nationalPark)
        {
            _dbContext.NationalParks.Update(nationalPark);
            return Save();
        }
    }
}
