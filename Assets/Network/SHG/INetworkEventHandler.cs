using System;

namespace SHG
{
  public interface INetworkEventReciever
  {
    public void ReceiveEvent(object[] args);    
  }

  public interface INetworkEventHandler
  {
    public enum EventReceiver
    {
      Others,
      Master,
      All
    }
    public Action OnNetworkConnected { get; set; }
    public Action OnNetworkDisconnected { get; set; }
    public Action OnJoinedToRoom { get; set; }
    public void Register<T>(T sender) where T: class, INetworkEventReciever;
    public void SendEvent<T>(T sender, object[] data, EventReceiver reciever = EventReceiver.Others) where T: class, INetworkEventReciever ;
  }
}
