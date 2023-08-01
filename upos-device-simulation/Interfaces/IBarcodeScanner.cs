﻿using System;
using upos_device_simulation.Models;

namespace upos_device_simulation.Interfaces
{
    public interface IBarcodeScanner
    {
        event EventHandler<ScannedEventArgs> Scanned;
        void Start();

    }
}
