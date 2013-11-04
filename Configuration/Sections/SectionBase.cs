﻿using System;
using System.Collections.Generic;
using System.Globalization;


namespace ARDrone2Client.Common.Configuration.Sections
{
    public class SectionBase
    {
        private readonly DroneConfiguration _configuration;
        private readonly string _name;

        public SectionBase(DroneConfiguration configuration, string name)
        {
            _configuration = configuration;
            _name = name;
        }

        private string FullKey(string index)
        {
            return _name + ":" + index;
        }

        protected String GetString(string index)
        {
            string value;
            if (_configuration.Items.TryGetValue(FullKey(index), out value))
            {
                return value;
            }
            return default(String);
        }

        protected Int32 GetInt32(string index)
        {
            string value;
            if (_configuration.Items.TryGetValue(FullKey(index), out value))
            {
                return Int32.Parse(value);
            }
            return default(Int32);
        }

        protected Single GetSingle(string index)
        {
            string value;
            if (_configuration.Items.TryGetValue(FullKey(index), out value))
            {
                return Single.Parse(value);
            }
            return default(Single);
        }

        protected Double GetDouble(string index)
        {
            string value;
            if (_configuration.Items.TryGetValue(FullKey(index), out value))
            {
                return Double.Parse(value);
            }
            return default(Double);
        }

        protected Boolean GetBoolean(string index)
        {
            string value;
            if (_configuration.Items.TryGetValue(FullKey(index), out value))
            {
                return Boolean.Parse(value);
            }
            return default(Boolean);
        }

        protected FlightAnimation GetFlightAnimation(string index)
        {
            string value;
            if (_configuration.Items.TryGetValue(FullKey(index), out value))
            {
                return FlightAnimation.Parse(value);
            }
            return default(FlightAnimation);
        }

        protected UserboxCommand GetUserboxCommand(string index)
        {
            string value;
            if (_configuration.Items.TryGetValue(FullKey(index), out value))
            {
                return UserboxCommand.Parse(value);
            }
            return default(UserboxCommand);
        }

        protected T GetEnum<T>(string index)
        {
            string value;
            if (_configuration.Items.TryGetValue(FullKey(index), out value))
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            return default(T);
        }

        protected void Set(string index, string value)
        {
            string key = FullKey(index);
            if (_configuration.Items.ContainsKey(key) == false)
            {
                _configuration.Items.Add(key, value);
            }
            else
            {
                _configuration.Items[key] = value;
            }
            _configuration.Changes.Enqueue(new KeyValuePair<string, string>(key, value));
        }

        protected void Set(string index, Int32 value)
        {
            Set(index, value.ToString("D"));
        }

        protected void Set(string index, Single value)
        {
            Set(index, value.ToString(CultureInfo.InvariantCulture));
        }

        protected void Set(string index, Boolean value)
        {
            Set(index, value.ToString().ToUpper());
        }

        protected void Set(string index, FlightAnimation value)
        {
            Set(index, value.ToString());
        }

        protected void Set(string index, UserboxCommand value)
        {
            Set(index, value.ToString());
        }

        protected void SetEnum<T>(string index, Enum value)
        {
            Set(index, value.ToString("D"));
        }

        public static int ToInt(float value)
        {
            int result = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
            return result;
        }

        public static float ToSingle(uint value)
        {
            float result = BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
            return result;
        }
    }
}
