using ChatApp.Server.Networking.Data;

namespace ChatApp.Server.Actioning
{
    public interface IActionClient<T>
    {
        void Received(T action);
    }
}
