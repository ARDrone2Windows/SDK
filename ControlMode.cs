namespace ARDrone2Client.Common
{
    public enum ControlMode
    {
        NoControlMode = 0, // Doing nothing
        ARDroneUpdateControlMode, // Not used
        PicUpdateControlMode, // Not useds
        LogsGetControlMode, // Not used
        CfgGetControlMode, // Send active configuration file to a client through the 'control' socket TCP 5559
        AckControlMode, // Reset command mask in navdata
        CustomCfgGetControlMode // Requests the list of custom configuration IDs
    }
}