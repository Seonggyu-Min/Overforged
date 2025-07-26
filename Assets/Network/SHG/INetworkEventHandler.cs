using System;

namespace SHG
{
  public interface INetworkEventHandler
  {
    public Action OnNetworkConnected { get; set; }
    public Action OnNetworkDisconnected { get; set; }
    public Action OnJoinedToRoom { get; set; }
  }
}
