using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Address.Api.Services
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetUserAddress(int userId);
    }

    public class AddressService : IAddressService
    {
        readonly List<Address> Addresses = new List<Address>();
        private DateTime _recoveryTime = DateTime.UtcNow;
        private static readonly Random _random = new();

        public AddressService()
        {
            for (int i = 0; i < 10; i++)
            {
                Addresses.Add(new Address()
                {
                     HouseNumber = i.ToString(),
                     Street = $"Street{i}",
                     UserId = i 
                });
            }
        }

        public async Task<IEnumerable<Address>> GetUserAddress(int userId)
        {
            if (_recoveryTime > DateTime.UtcNow)
            {
                throw new Exception("Service broken");
            }

            if (_recoveryTime < DateTime.UtcNow && _random.Next(1, 4) == 1)
            {
                _recoveryTime = DateTime.UtcNow.AddSeconds(10);
            }

            return await Task
                .FromResult(
                    Addresses.Where(w => w.UserId == userId)
                    .ToList());
        }
    }
}
