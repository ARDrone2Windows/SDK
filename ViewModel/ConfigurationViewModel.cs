using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using ARDrone2Client.Common.Configuration;
using ARDrone2Client.Common.Configuration.Sections;
using ARDrone2Client.Common.ViewModel;

namespace ARDrone2Client.Common.ViewModel
{
    public static class ConfigurationViewModelHelper
    {
        public static List<ConfigurationSectionViewModel> InitializeConfigurationSections(DroneConfiguration configuration)
        {
            List<ConfigurationSectionViewModel> configurationSections = new List<ConfigurationSectionViewModel>();

            configurationSections.Add(new ConfigurationSectionViewModel("control", GetItems(configuration, configuration.Control, typeof(ControlSection))));
            configurationSections.Add(new ConfigurationSectionViewModel("custom", GetItems(configuration, configuration.Custom, typeof(CustomSection))));
            configurationSections.Add(new ConfigurationSectionViewModel("detect", GetItems(configuration, configuration.Detect, typeof(DetectSection))));
            configurationSections.Add(new ConfigurationSectionViewModel("general", GetItems(configuration, configuration.General, typeof(GeneralSection))));
            configurationSections.Add(new ConfigurationSectionViewModel("gps", GetItems(configuration, configuration.Gps, typeof(GpsSection))));
            configurationSections.Add(new ConfigurationSectionViewModel("leds", GetItems(configuration, configuration.Leds, typeof(LedsSection))));
            configurationSections.Add(new ConfigurationSectionViewModel("network", GetItems(configuration, configuration.Network, typeof(NetworkSection))));
            configurationSections.Add(new ConfigurationSectionViewModel("pic", GetItems(configuration, configuration.Pic, typeof(PicSection))));
            configurationSections.Add(new ConfigurationSectionViewModel("syslog", GetItems(configuration, configuration.Syslog, typeof(SyslogSection))));
            configurationSections.Add(new ConfigurationSectionViewModel("userbox", GetItems(configuration, configuration.Userbox, typeof(UserboxSection))));
            configurationSections.Add(new ConfigurationSectionViewModel("video", GetItems(configuration, configuration.Video, typeof(VideoSection))));

            return configurationSections;
        }

        public static void UpdateConfigurationSections(List<ConfigurationSectionViewModel> sections, DroneConfiguration configuration)
        {
            sections.First<ConfigurationSectionViewModel>(section => section.SectionName.Equals("control"))
                .UpdateItems(GetItems(configuration, configuration.Control, typeof(ControlSection)));
            sections.First<ConfigurationSectionViewModel>(section => section.SectionName.Equals("custom"))
                .UpdateItems(GetItems(configuration, configuration.Custom, typeof(CustomSection)));
            sections.First<ConfigurationSectionViewModel>(section => section.SectionName.Equals("detect"))
                .UpdateItems(GetItems(configuration, configuration.Detect, typeof(DetectSection)));
            sections.First<ConfigurationSectionViewModel>(section => section.SectionName.Equals("general"))
                .UpdateItems(GetItems(configuration, configuration.General, typeof(GeneralSection)));
            sections.First<ConfigurationSectionViewModel>(section => section.SectionName.Equals("gps"))
                .UpdateItems(GetItems(configuration, configuration.Gps, typeof(GpsSection)));
            sections.First<ConfigurationSectionViewModel>(section => section.SectionName.Equals("leds"))
                .UpdateItems(GetItems(configuration, configuration.Leds, typeof(LedsSection)));
            sections.First<ConfigurationSectionViewModel>(section => section.SectionName.Equals("network"))
                .UpdateItems(GetItems(configuration, configuration.Network, typeof(NetworkSection)));
            sections.First<ConfigurationSectionViewModel>(section => section.SectionName.Equals("pic"))
                .UpdateItems(GetItems(configuration, configuration.Pic, typeof(PicSection)));
            sections.First<ConfigurationSectionViewModel>(section => section.SectionName.Equals("syslog"))
                .UpdateItems(GetItems(configuration, configuration.Syslog, typeof(SyslogSection)));
            sections.First<ConfigurationSectionViewModel>(section => section.SectionName.Equals("userbox"))
                .UpdateItems(GetItems(configuration, configuration.Userbox, typeof(UserboxSection)));
            sections.First<ConfigurationSectionViewModel>(section => section.SectionName.Equals("video"))
                .UpdateItems(GetItems(configuration, configuration.Video, typeof(VideoSection)));
        }

        private static List<IConfigurationItem> GetItems(DroneConfiguration configuration, object section, Type type)
        {
            List<IConfigurationItem> configurationItems = new List<IConfigurationItem>();

            var configItems = type.GetRuntimeFields()
                .Where(field => field.GetValue(section) is IConfigurationItem);

            foreach (var configItem in configItems)
            {
                configurationItems.Add((IConfigurationItem)configItem.GetValue(section));
            }

            return configurationItems;
        }
    }    
    
    public static class ConfigurationItemExtension
    {
        public static void AddRange(this List<ConfigurationItemViewModel> configurationItemViewModels, List<IConfigurationItem> configurationItems)
        {
            foreach (var configurationItem in configurationItems)
            {
                configurationItemViewModels.Add(new ConfigurationItemViewModel(configurationItem));
            }
        }
    }
    
    public class ConfigurationSectionViewModel : BaseViewModel
    {
        public string SectionName { get; set; }
        public List<ConfigurationItemViewModel> ConfigItems { get; set; }

        public ConfigurationSectionViewModel(string sectionName, List<IConfigurationItem> configItems)
        {
            this.SectionName = sectionName;

            if (configItems != null && configItems.Count > 0)
            {
                this.ConfigItems = new List<ConfigurationItemViewModel>();
                this.ConfigItems.AddRange(configItems);
            }
        }

        public void UpdateItems(List<IConfigurationItem> configItems)
        {
            foreach(IConfigurationItem item in configItems)
            {
                this.ConfigItems.Find(i => i.ConfigItem == item.Key).ConfigValue =
                    (item.Value == null) ? string.Empty : item.Value.ToString();
            }
        }
    }

    public class ConfigurationItemViewModel : BaseViewModel
    {
        #region ShortConfigItem
        private string shortConfigItem;
        public string ShortConfigItem
        {
            get
            {
                return this.shortConfigItem;
            }

            set
            {
                if (value != this.shortConfigItem)
                {
                    this.shortConfigItem = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region ConfigItem
        private string configItem;
        public string ConfigItem
        {
            get
            {
                return this.configItem;
            }

            set
            {
                if (value != this.configItem)
                {
                    this.configItem = value;
                    this.shortConfigItem = value.Substring(value.IndexOf(":") + 1);
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region ConfigValue
        private string configValue;
        public string ConfigValue
        {
            get
            {
                return this.configValue;
            }

            set
            {
                if (value != this.configValue)
                {
                    this.configValue = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region IsWritable
        private string isWritable;
        public string IsWritable
        {
            get
            {
                return this.isWritable;
            }

            set
            {
                if (value != this.isWritable)
                {
                    this.isWritable = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        public ConfigurationItemViewModel(IConfigurationItem configurationItem)
        {
            this.ConfigItem = configurationItem.Key;
            this.ConfigValue = (configurationItem.Value == null) ? string.Empty : configurationItem.Value.ToString();
            this.IsWritable = (configurationItem is ReadWriteItem<bool> ||
                configurationItem is ReadWriteItem<int>   ||
                configurationItem is ReadWriteItem<float> ||
                configurationItem is ReadWriteItem<Enum>  ||
                configurationItem is ReadWriteItem<string>).ToString();
        }
    }
}
