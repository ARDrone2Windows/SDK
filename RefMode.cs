namespace ARDrone2Client.Common
{
    public enum RefMode
    {
        Default = 1 << 18 | 1 << 20 | 1 << 22 | 1 << 24 | 1 << 28,
        
        //pas d'état à attendre
        Land = (0 << 9) | Default,
        
        //attente state feedback start
        Takeoff = (1 << 9) | Default,
        
        //=select à 0 faire un reset avant de reprendre, 1 pour le mettre en emergency
        Emergency = (1 << 8) | Default,
        ResetEmergency = (0 << 8) | Default

    }
}