using EngineerHomework.Models;
using System.Collections.Generic;

namespace EngineerHomework.Interfaces
{
    public interface IOrgCollection
    {
        Org GetOrg(int orgId);

        List<Org> GetOrgTree(int orgId, bool inclusive);
    }
}
