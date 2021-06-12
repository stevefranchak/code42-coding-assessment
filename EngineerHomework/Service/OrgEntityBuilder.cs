using EngineerHomework.Interfaces;
using EngineerHomework.Models;
using System.Collections.Generic;

namespace EngineerHomework.Service
{
    public class OrgEntityBuilder : IEntityBuilder<Org>
    {
        private List<Org> orgList = new List<Org>();
        public void AddEntityFromCsvLine(string csvLine)
        {
            orgList.Add(Org.Generate(csvLine));
        }

        public List<Org> GetEntityList()
        {
            return orgList;
        }

    }
}
