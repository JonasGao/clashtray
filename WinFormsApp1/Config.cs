﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    internal class Config
    {
        private const string ConfigFileName = ".config";

        public string ClashFileName { get; set; }

        public string ProfileFileName { get; set; }

        public bool AutoStartupClash { get; set; }

        public bool EnableHostingProfile { get; set; }

        public List<HostingProfile> Profiles { get; set; }
        private Config()
        {
            ClashFileName = "clash.exe";
            ProfileFileName = "";
            AutoStartupClash = false;
            EnableHostingProfile = false;
            Profiles = new List<HostingProfile>();
        }

        public Config(string clashFileName, string profileFileName, bool autoStartupClash, bool enableHostingProfile, List<HostingProfile> profiles)
        {
            ClashFileName = clashFileName;
            ProfileFileName = profileFileName;
            AutoStartupClash = autoStartupClash;
            EnableHostingProfile = enableHostingProfile;
            Profiles = profiles;
        }

        public void Save()
        {
            File.WriteAllLines(ConfigFileName, new string[]{
                ClashFileName,
                ProfileFileName,
                AutoStartupClash.ToString(),
                EnableHostingProfile.ToString(),
                EncodeProfiles(Profiles)
            });
        }

        public static Config ReadConfig()
        {
            if (!File.Exists(ConfigFileName))
            {
                return new Config();
            }

            var lines = File.ReadAllLines(ConfigFileName);
            string clashFileName = null!;
            string profileFileName = "";
            bool autoStartupClash = false;
            bool enableHostingProfile = false;
            List<HostingProfile> profiles = new List<HostingProfile>();
            if (lines.Length > 0)
            {
                clashFileName = lines[0];
            }

            if (string.IsNullOrEmpty(clashFileName))
            {
                clashFileName = "clash.exe";
            }

            if (lines.Length > 1)
            {
                profileFileName = lines[1];
            }

            if (lines.Length > 2)
            {
                string value = lines[2];
                autoStartupClash = string.Equals(value, "True");
            }

            if (lines.Length > 3)
            {
                string value = lines[3];
                enableHostingProfile = string.Equals(value, "True");
            }

            if (lines.Length > 4)
            {
                string value = lines[4];
                profiles = DecodeProfiles(value);
            }

            return new Config(clashFileName, profileFileName, autoStartupClash, enableHostingProfile, profiles);
        }

        private static string EncodeProfiles(List<HostingProfile> profiles)
        {
            StringBuilder builder = new StringBuilder();
            foreach (HostingProfile profile in profiles)
            {
                builder.Append($"{profile.Name},{profile.URL};");
            }
            return builder.ToString();
        }

        private static List<HostingProfile> DecodeProfiles(string value)
        {
            List<HostingProfile> profiles = new();
            if (string.IsNullOrEmpty(value))
            {
                return profiles;
            }
            foreach (var item in value.Split(";", StringSplitOptions.RemoveEmptyEntries))
            {
                var values = item.Split(",");
                profiles.Add(new HostingProfile { Name = values[0], URL = values[1] });
            }
            return profiles;
        }
    }
}
