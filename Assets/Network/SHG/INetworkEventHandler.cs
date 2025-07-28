using System;

namespace SHG
{
  public interface INetworkEventReciever
  {
    public void ReceiveEvent(object[] args);    
  }

  public interface INetworkEventHandler
  {
    public Action OnNetworkConnected { get; set; }
    public Action OnNetworkDisconnected { get; set; }
    public Action OnJoinedToRoom { get; set; }
    public void Register<T>(T sender) where T: class, INetworkEventReciever;
    public void SendEvent<T>(T sender, object[] data) where T: class, INetworkEventReciever ;
  }
}
