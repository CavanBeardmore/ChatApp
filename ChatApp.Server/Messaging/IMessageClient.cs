using ChatApp.Server.Networking.Data;

namespace ChatApp.Server.Messaging
{
    public interface IMessageClient<T>
    {
        void Send(T message);

        void Received(T message);
    }
}
