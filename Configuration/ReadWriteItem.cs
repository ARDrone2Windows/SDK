﻿namespace ARDrone2Client.Common.Configuration
{
    public class ReadWriteItem<T> : ReadOnlyItem<T>
    {
        public ReadWriteItem(string key)
            : base(key)
        {
        }

        public ReadWriteItem<T> Set(T value)
        {
            Value = value;
            return this;
        }
    }
}