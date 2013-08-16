using System.Threading;
using ARDrone2Client.Common.Workers;
using System.Collections.Generic;
using Windows.System.Threading;
using System;
using ARDrone2Client.Common;

namespace ARDrone2Client.Common.Workers
{
    public class WatchdogWorker : WorkerBase
    {
        private ThreadPoolTimer _Timer;
        private List<WorkerBase> _Workers = new List<WorkerBase>();
        public WatchdogWorker(DroneClient droneClient, WorkerBase[] workers)
        {
            foreach (WorkerBase wb in workers)
            {
                _Workers.Add(wb);
            }
        }
        public override void Start()
        {
            base.Start();
            if (_Timer != null)
                return;
            _Timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(timerElapsedHandler), TimeSpan.FromMilliseconds(100));
        }
        public override void Stop()
        {
            if (_Timer != null)
            {
                _Timer.Cancel();
                _Timer = null;
            }
            base.Stop();
        }

        protected override void Loop()
        {
        }
        public override bool IsAlive
        {
            get
            {
                return true;
            }
        }
        private void timerElapsedHandler(ThreadPoolTimer timer)
        {
#if DEBUG
            return;
#else
            foreach (WorkerBase wb in _Workers)
            {
                if (!wb.IsAlive)
                {
                    wb.Stop();
                    //TODO AD Add watchdog log
                    wb.Start();
                }
            }
#endif
        }

    }
}