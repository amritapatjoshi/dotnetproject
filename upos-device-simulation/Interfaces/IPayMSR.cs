using System;
using upos_device_simulation.Models;

namespace upos_device_simulation.Interfaces
{
    public interface IPayMSR
    {
        event EventHandler<CardSwipeEventArgs> CardSwiped;
        void Start();
    }
}
