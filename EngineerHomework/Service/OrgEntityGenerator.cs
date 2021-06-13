using EngineerHomework.Interfaces;
using EngineerHomework.Models;

namespace EngineerHomework.Service
{
    public class OrgEntityGenerator : IEntityGenerator<Org>
    {
        public Org Generate(string csvLine)
        {
            return Org.Generate(csvLine);
        }
    }
}
