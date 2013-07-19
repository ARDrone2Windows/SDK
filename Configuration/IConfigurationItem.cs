namespace ARDrone2Client.Common.Configuration
{
    public interface IConfigurationItem
    {
        string Key { get; }
        object Value { get; }
        bool TryUpdate(string value);
    }
}