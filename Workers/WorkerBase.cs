using System;
using Windows.Foundation;
using Windows.System.Threading;
using ARDrone2Client.Common.Helpers;

namespace ARDrone2Client.Common.Workers
{
    public abstract class WorkerBase : DisposableBase
    {
        private IAsyncAction threadPoolWorkItem;

        public virtual bool IsAlive
        {
            get { return threadPoolWorkItem != null && !threadPoolWorkItem.Status.ToString().Equals(""); }
        }

        public virtual void Start()
        {
            if (IsAlive) return;
            lock (this)
            {
                if (IsAlive) return;

                threadPoolWorkItem = ThreadPool.RunAsync((_) => RunLoop());
            }
        }

        public virtual void Stop()
        {
            if (threadPoolWorkItem != null)
            {
                threadPoolWorkItem.Cancel();
            }
        }

        private void RunLoop()
        {
            try
            {
                Loop();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                OnException(exception);
            }
        }

        protected virtual void Loop()
        {
        }

        protected virtual void OnException(Exception exception)
        {
            //Trace.TraceError("{0} - Exception: {1}", GetType(), exception.Message);
        }

        protected override void DisposeOverride()
        {
            Stop();
        }
    }
}
