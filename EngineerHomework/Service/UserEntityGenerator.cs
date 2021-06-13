using EngineerHomework.Interfaces;
using EngineerHomework.Models;

namespace EngineerHomework.Service
{
    public class UserEntityGenerator : IEntityGenerator<User>
    {
        public User Generate(string csvLine)
        {
            return User.Generate(csvLine);
        }
    }
}
