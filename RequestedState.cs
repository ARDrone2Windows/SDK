using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARDrone2Client.Common
{
    public enum RequestedState
    {
        None,
        Initialize,
        GetConfiguration,
        Land,
        TakeOff,
        Emergency,
        ResetEmergency
    }
}
