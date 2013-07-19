﻿using System;

namespace ARDrone2Client.Common.Helpers
{
    public abstract class DisposableBase : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && _disposed == false)
            {
                DisposeOverride();

                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        protected abstract void DisposeOverride();
    }
}
