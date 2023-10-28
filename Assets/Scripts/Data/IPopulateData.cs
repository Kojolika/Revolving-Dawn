using Cysharp.Threading.Tasks;

namespace Data
{
    public interface IPopulateData<D>
    {
        void Populate(D data);

        UniTask PopulateAsync(D data)
        {
            return UniTask.CompletedTask;
        }
    }

    public class Null
    {
    }
}