using System.Collections.Generic;

namespace EngineerHomework.Interfaces
{
    public interface IEntityBuilder<T>
    {
        void AddEntityFromCsvLine(string csvLine);

        List<T> GetEntityList();
    }
}
