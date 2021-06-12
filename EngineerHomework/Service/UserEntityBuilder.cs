using System.Collections.Generic;
using EngineerHomework.Interfaces;
using EngineerHomework.Models;

namespace EngineerHomework.Service
{
    public class UserEntityBuilder : IEntityBuilder<User>
    {
        private List<User> userList = new List<User>();
        public void AddEntityFromCsvLine(string csvLine)
        {
            userList.Add(User.Generate(csvLine));
        }

        public List<User> GetEntityList()
        {
            return userList;
        }
    }
}
