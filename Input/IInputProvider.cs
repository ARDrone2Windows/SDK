using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARDrone2Client.Common.Input
{
    public interface IInputProvider : IDisposable
    {
        DroneClient DroneClient { get; }
        string Name { get; }
    }
}
