using CarSalesPortal.DataAccess.Data;
using CarSalesPortal.DataAccess.Repository.IRepository;
using CarSalesPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CarSalesPortal.DataAccess.Repository
{
    public class CarRepository : Repository<Car>, ICarRepository
    {
        private ApplicationDbContext _db;

        public CarRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }     

        public void Update(Car obj)
        {
            var objFromDb = _db.Cars.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Make = obj.Make;
                objFromDb.Model = obj.Model;
                objFromDb.Year = obj.Year;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Colour = obj.Colour;
                objFromDb.Price = obj.Price;
                objFromDb.Status = obj.Status;
               


                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }

            }
        }
    }
}
