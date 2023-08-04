using System;
using upos_device_simulation.Models;

namespace upos_device_simulation.Interfaces
{
    public interface IPaypinpad
    {
        event EventHandler<PinEnteredEventArgs> PinEntered;
        void Start(CardSwipeEventArgs cardInfo = null);
        string CheckDeviceHealth();
    }
}
