using System;
using System.Reflection;

namespace ARDrone2Client.Common.Configuration
{
    public static class CommandHelper
    {
        public static string ToCommand<T>(this ReadWriteItem<T> item)
        {
            Type type = typeof (T);

            if (type == typeof (string))
                return Command.Config(item.Key, (string)(object)item.Value);

            if (type == typeof (int))
                return Command.Config(item.Key, (int)(object)item.Value);

            if (type == typeof (bool))
                return Command.Config(item.Key, (bool)(object)item.Value);

            if (type == typeof (float))
                return Command.Config(item.Key, (float)(object)item.Value);
            if (type.GetTypeInfo().IsEnum)
                return Command.Config(item.Key, (Enum)(object)item.Value);
            throw new NotSupportedException();
        }
    }
}