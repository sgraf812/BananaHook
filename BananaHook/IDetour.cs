namespace BananaHook
{
    public interface IDetour
    {
        object Invoke(params object[] parameters);
    }
}