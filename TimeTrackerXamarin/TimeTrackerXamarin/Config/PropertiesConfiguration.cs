using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace TimeTrackerXamarin.Config
{
    public class PropertiesConfiguration : IConfiguration
    {
#if DEBUG
        private const string ConfigFile = "devconfig.properties";
#else
        private const string ConfigFile = "config.properties";
#endif

        class ConfigurationError : Exception
        {
            public ConfigurationError(string message) : base(message)
            {
            }
        }
        
        public string ApiUrl { get; private set; }
        public int FrameLength { get; private set; }

        public int FrameTimeout { get; private set; }

        public bool IsDebug { get; private set; }

        public int InactivityNotificationTime { get; private set; }
        public int AppDisabledTime { get; private set; }

        public void Load()
        {
            var properties = new Dictionary<string, string>();
            using (var stream = FileSystem.OpenAppPackageFileAsync(ConfigFile).Result)
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        try
                        {
                            if (line.StartsWith("#"))
                            {
                                continue;
                            }
                            var split = line.Split('=');
                            var key = split[0];
                            var value = string.Join("=", split.Skip(1));
                            properties.Add(key, value);
                        }
                        catch (Exception e)
                        {
                            throw new ConfigurationError($"Error while parsing line '{line}'");
                        }
                    }
                }
            }

            
            ApiUrl = properties["api-url"];
            if (ApiUrl == null)
            {
                throw new ConfigurationError("Missing api-url property.");
            }

            var frameLengthString = properties["frame-length"]; 
            if (frameLengthString == null)
            {
                throw new ConfigurationError("Missing frame-length property.");
            }

            if (!int.TryParse(frameLengthString, out var frameLength))
            {
                throw new ConfigurationError($"{frameLengthString} is not a valid int.");
            }
            
            FrameLength = frameLength;

            var frameTimeoutString = properties["frame-timeout"];
            if (frameTimeoutString == null)
            {
                throw new ConfigurationError("Missing frame-timeout property.");
            }

            if (!int.TryParse(frameTimeoutString, out var frameTimeout))
            {
                throw new ConfigurationError($"{frameTimeoutString} is not a valid int.");
            }
            
            FrameTimeout = frameTimeout;
            
            var inactivityTimeString = properties["inactivity-notification-time"];
            if (inactivityTimeString == null)
            {
                throw new ConfigurationError("Missing inactivityTimeString property.");
            }

            if (!int.TryParse(inactivityTimeString, out var inactivityTime))
            {
                throw new ConfigurationError($"{inactivityTimeString} is not a valid int.");
            }

            InactivityNotificationTime = inactivityTime;

            var appDisabledTimeString = properties["inactivity-notification-time"];
            if (appDisabledTimeString == null)
            {
                throw new ConfigurationError("Missing inactivityTimeString property.");
            }

            if (!int.TryParse(appDisabledTimeString, out var appDisabledTime))
            {
                throw new ConfigurationError($"{appDisabledTimeString} is not a valid int.");
            }

            AppDisabledTime = appDisabledTime;

            var debugString = properties["debug"];
            if (debugString == null)
            {
                throw new ConfigurationError("Missing debug property.");
            }
            
            if (!bool.TryParse(debugString, out var debug))
            {
                throw new ConfigurationError($"{frameTimeoutString} is not a valid boolean.");
            }

            IsDebug = debug;
        }
    }
}