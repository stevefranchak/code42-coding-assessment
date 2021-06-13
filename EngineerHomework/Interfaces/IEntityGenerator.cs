namespace EngineerHomework.Interfaces
{
    public interface IEntityGenerator<T>
    {
        T Generate(string csvLine);
    }
}
