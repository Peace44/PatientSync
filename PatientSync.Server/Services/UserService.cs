using System.Collections.Generic;
using System.Linq;
using PatientSync.Server.Models;
using PatientSync.Server.Repositories;

namespace PatientSync.Server.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository _repository;

        public UserService(IRepository repository)
        {
            _repository = repository;
        }



        public IEnumerable<User> GetAllUsers() => _repository.GetUsers();
        public User GetUserById(int id) => _repository.GetUsers().FirstOrDefault(u => u.ID == id);
        public User GetUserByUsername(string username) => _repository.GetUserByUsername(username);
        public void AddUser(User user) => _repository.AddUser(user);
        public void UpdateUser(User user) => _repository.UpdateUser(user);
        public void DeleteUser(int id) => _repository.DeleteUser(id);
    }
}
