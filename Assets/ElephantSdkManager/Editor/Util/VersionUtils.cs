using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ElephantSdkManager.Model;
using UnityEditor;
using UnityEngine;

namespace ElephantSdkManager.Util
{
    public static class VersionUtils
    {
        public static int CompareVersions(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0;

            var versionA = VersionStringToInts(a);
            var versionB = VersionStringToInts(b);
            for (var i = 0; i < Mathf.Max(versionA.Length, versionB.Length); i++)
            {
                if (VersionPiece(versionA, i) < VersionPiece(versionB, i))
                    return -1;
                if (VersionPiece(versionA, i) > VersionPiece(versionB, i))
                    return 1;
            }

            return 0;
        }

        public static bool IsEqualVersion(string a, string b)
        {
            return a.Equals(b);
        }


        private static int VersionPiece(IList<int> versionInts, int pieceIndex)
        {
            return pieceIndex < versionInts.Count ? versionInts[pieceIndex] : 0;
        }


        private static int[] VersionStringToInts(string version)
        {
            int piece;
            if (version.Contains("_internal"))
            {
                version = version.Replace("_internal", string.Empty);
            }

            return version.Split('.')
                .Select(v => int.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out piece) ? piece : 0)
                .ToArray();
        }

        private static string CheckMediationPackageName(string packageName)
        {
            if (packageName.ToLower().Contains("gamekit-is"))
            {
                return Application.dataPath + "/RollicGames/RollicIronSourceIDs.cs";
            }

            if (packageName.ToLower().Contains("gamekit-max") || packageName.ToLower().Contains("gamekit-for"))
            {
                return Application.dataPath + "/RollicGames/RollicApplovinIDs.cs";
            }

            return null;
        }

        private static string GetElephantThirdParyIdsPath(string packageName)
        {
            if (packageName.ToLower().Contains("gamekit"))
            {
                return Application.dataPath + "/Elephant/Core/ElephantThirdPartyIds.cs";
            }

            return null;
        }

        public static void SetupElephantThirdPartyIDs(GameKitManifest gameKitManifest, string packageName)
        {
            if (gameKitManifest is null || gameKitManifest.data is null || gameKitManifest.data.appKey is null) return;

            var elephantPath = GetElephantThirdParyIdsPath(packageName);
            if (elephantPath is null) return;

            var lines = File.ReadAllLines(elephantPath);
            File.Delete(elephantPath);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Your IDs are being set...\n");

            var idMap = new Dictionary<string, string>
            {
                { "FacebookClientToken", gameKitManifest.data.facebookClientToken },
                { "FacebookAppId", gameKitManifest.data.facebookAppId },
                { "GameId", gameKitManifest.data.gameId },
                { "GameSecret", gameKitManifest.data.gameSecret },
                { "BundleName", gameKitManifest.data.bundle },
                { "AdjustAppKey", gameKitManifest.data.adjustAppKey },
                { "HelpShiftDomainAndroid", gameKitManifest.data.helpshiftDomainAndroid },
                { "HelpShiftAppIdAndroid", gameKitManifest.data.helpshiftAppIDAndroid },
                { "HelpshiftDomainIOS", gameKitManifest.data.helpshiftDomainIos },
                { "HelpShiftAppIdIOS", gameKitManifest.data.helpshiftAppIDIos }
            };

            using (var sw = File.AppendText(elephantPath))
            {
                foreach (var line in lines)
                {
                    var newLine = line;
                    foreach (var entry in idMap)
                    {
                        if (line.Trim().StartsWith($"public static string {entry.Key} ="))
                        {
                            newLine = ReplaceValue(line, entry.Value);
                            stringBuilder.Append($"{entry.Key}: {entry.Value}\n");
                            if (entry.Key == "BundleName")
                            {
                                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, entry.Value);
                                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, entry.Value);
                                stringBuilder.Append("Game Bundle Name is set to: " + entry.Value + "\n");
                            }

                            break;
                        }
                    }

                    sw.WriteLine(newLine);
                }
            }

            SetupAdjustTokens(gameKitManifest);

            Debug.Log(stringBuilder);
        }

        private static string ReplaceValue(string line, string newValue)
        {
            var startIndex = line.IndexOf('\"') + 1;
            var endIndex = line.LastIndexOf('\"');

            if (startIndex > 0 && endIndex >= startIndex)
            {
                return line.Substring(0, startIndex) + newValue + line.Substring(endIndex);
            }

            return line;
        }

        public static void SetupAdjustTokens(GameKitManifest gameKitManifest)
        {
            var adjustTokenClassPath = Application.dataPath + "/Elephant/Core/AdjustTokens.cs";
            if (!File.Exists(adjustTokenClassPath)) return;

            var lines = File.ReadAllLines(adjustTokenClassPath);
            File.Delete(adjustTokenClassPath);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Setting up Adjust tokens...\n");

            var tokenMap = new Dictionary<string, string>
            {
                { "FullScreenWatched_10", "Fs_watched_10" },
                { "FullScreenWatched_25", "Fs_watched_25" },
                { "FullScreenWatched_50", "Fs_watched_50" },
                { "Level_10", "lvl10" },
                { "Level_20", "lvl20" },
                { "Level_30", "lvl30" },
                { "Level_50", "lvl50" },
                { "Level_100", "lvl100" },
                { "RewardedWatched_10", "Rw_watched_10" },
                { "RewardedWatched_25", "Rw_watched_25" },
                { "RewardedWatched_50", "Rw_watched_50" },
                { "Timespend_10", "Timespend_10" },
                { "Timespend_30", "Timespend_30" },
                { "Timespend_60", "Timespend_60" },
                { "Timespend_120", "Timespend_120" },
                { "SkanCvUpdate", "skan_cv_update" }
            };

            using (var sw = File.AppendText(adjustTokenClassPath))
            {
                foreach (var line in lines)
                {
                    var newLine = line;
                    foreach (var entry in tokenMap)
                    {
                        if (line.Trim().StartsWith($"public static string {entry.Key} ="))
                        {
                            var token = GetToken(gameKitManifest, entry.Value);
                            newLine = ReplaceValue(line, token);
                            stringBuilder.Append($"Setting Adjust token for {entry.Key}: {token}\n");
                            break;
                        }
                    }

                    sw.WriteLine(newLine);
                }
            }

            Debug.Log(stringBuilder);
        }

        private static string GetToken(GameKitManifest gameKitManifest, string eventName)
        {
            var key = gameKitManifest.data.adjustEvents.Find(aEvent => aEvent.name.Equals(eventName));
            return key != null ? key.token : "";
        }

        public static void SetupGameKitIDs(GameKitManifest gameKitManifest, string packageName)
        {
            if (gameKitManifest?.data?.appKey is null) return;

            var rollicAdsPath = CheckMediationPackageName(packageName);
            if (rollicAdsPath is null) return;

            var lines = File.ReadAllLines(rollicAdsPath);
            File.Delete(rollicAdsPath);
            var stringBuilder = new StringBuilder();

            var idMap = new Dictionary<string, string>
            {
                { "AppKey", gameKitManifest.data.appKey },
                { "AppKey_ios", gameKitManifest.data.appKeyIos },
                { "AppKey_android", gameKitManifest.data.appKeyAndroid },
                { "BannerAdUnitIos", gameKitManifest.data.bannerAdUnitIos },
                { "InterstitialAdUnitIos", gameKitManifest.data.interstitialAdUnitIos },
                { "RewardedAdUnitIos", gameKitManifest.data.rewardedAdUnitIos },
                { "BannerAdUnitAndroid", gameKitManifest.data.bannerAdUnitAndroid },
                { "InterstitialAdUnitAndroid", gameKitManifest.data.interstitialAdUnitAndroid },
                { "RewardedAdUnitAndroid", gameKitManifest.data.rewardedAdUnitAndroid },
                { "GoogleIosId", gameKitManifest.data.googleAppIdIos },
                { "GoogleAndroidId", gameKitManifest.data.googleAppIdAndroid },
                { "AmazonAppIdIos", gameKitManifest.data.amazonAppIdIos },
                { "AmazonBannerSlotIdIos", gameKitManifest.data.amazonBannerSlotIdIos },
                { "AmazonInterstitialVideoSlotIdIos", gameKitManifest.data.amazonInterstitialVideoSlotIdIos },
                { "AmazonRewardedVideoSlotIdIos", gameKitManifest.data.amazonRewardedVideoSlotIdIos },
                { "AmazonAppIdAndroid", gameKitManifest.data.amazonAppIdAndroid },
                { "AmazonBannerSlotIdAndroid", gameKitManifest.data.amazonBannerSlotIdAndroid },
                { "AmazonInterstitialVideoSlotIdAndroid", gameKitManifest.data.amazonInterstitialVideoSlotIdAndroid },
                { "AmazonRewardedVideoSlotIdAndroid", gameKitManifest.data.amazonRewardedVideoSlotIdAndroid },
                { "FitoBannerAdUnitIos", gameKitManifest.data.fitoBannerAdUnitIos },
                { "FitoInterstitialAdUnitIos", gameKitManifest.data.fitoInterstitialAdUnitIos },
                { "FitoRewardedAdUnitIos", gameKitManifest.data.fitoRewardedAdUnitIos },
                { "FitoBannerAdUnitAndroid", gameKitManifest.data.fitoBannerAdUnitAndroid },
                { "FitoInterstitialAdUnitAndroid", gameKitManifest.data.fitoInterstitialAdUnitAndroid },
                { "FitoRewardedAdUnitAndroid", gameKitManifest.data.fitoRewardedAdUnitAndroid }
            };

            using (var sw = File.AppendText(rollicAdsPath))
            {
                foreach (var line in lines)
                {
                    var newLine = line;
                    foreach (var entry in idMap)
                    {
                        if (line.Trim().StartsWith($"public static string {entry.Key} ="))
                        {
                            newLine = ReplaceValue(line, entry.Value);
                            stringBuilder.Append($"{entry.Key}: {entry.Value}\n");
                            break;
                        }
                    }

                    sw.WriteLine(newLine);
                }
            }

            Debug.Log(stringBuilder);
        }

        #region IronSource Utils

        public static string GetVersionFromXML(string fileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string version = "";
            try
            {
                xmlDoc.LoadXml(File.ReadAllText("Assets/IronSource/Editor/" + fileName + ".xml"));
            }
            catch (Exception)
            {
                return version;
            }

            var unityVersion = xmlDoc.SelectSingleNode("dependencies/unityversion");
            if (unityVersion != null)
            {
                return (unityVersion.InnerText);
            }

            return version;
        }

        #endregion

        #region Max Utils

        public static Versions GetCurrentVersions(string dependencyPath)
        {
            XDocument dependency;
            try
            {
                dependency = XDocument.Load(dependencyPath);
            }
#pragma warning disable 0168
            catch (IOException exception)
#pragma warning restore 0168
            {
                // Couldn't find the dependencies file. The plugin is not installed.
                return new Versions();
            }

            // <dependencies>
            //  <androidPackages>
            //      <androidPackage spec="com.applovin.mediation:network_name-adapter:1.2.3.4" />
            //  </androidPackages>
            //  <iosPods>
            //      <iosPod name="AppLovinMediationNetworkNameAdapter" version="2.3.4.5" />
            //  </iosPods>
            // </dependencies>
            string androidVersion = null;
            string iosVersion = null;
            var dependenciesElement = dependency.Element("dependencies");
            if (dependenciesElement != null)
            {
                var androidPackages = dependenciesElement.Element("androidPackages");
                if (androidPackages != null)
                {
                    var adapterPackage = androidPackages.Descendants().FirstOrDefault(element =>
                        element.Name.LocalName.Equals("androidPackage")
                        && element.FirstAttribute.Name.LocalName.Equals("spec")
                        && element.FirstAttribute.Value.StartsWith("com.applovin"));
                    if (adapterPackage != null)
                    {
                        androidVersion = adapterPackage.FirstAttribute.Value.Split(':').Last();
                        // Hack alert: Some Android versions might have square brackets to force a specific version. Remove them if they are detected.
                        if (androidVersion.StartsWith("["))
                        {
                            androidVersion = androidVersion.Trim('[', ']');
                        }
                    }
                }

                var iosPods = dependenciesElement.Element("iosPods");
                if (iosPods != null)
                {
                    var adapterPod = iosPods.Descendants().FirstOrDefault(element =>
                        element.Name.LocalName.Equals("iosPod")
                        && element.FirstAttribute.Name.LocalName.Equals("name")
                        && element.FirstAttribute.Value.StartsWith("AppLovin"));
                    if (adapterPod != null)
                    {
                        iosVersion = adapterPod.Attributes()
                            .First(attribute => attribute.Name.LocalName.Equals("version")).Value;
                    }
                }
            }

            var currentVersions = new Versions();
            if (androidVersion != null && iosVersion != null)
            {
                currentVersions.Unity = string.Format("android_{0}_ios_{1}", androidVersion, iosVersion);
                currentVersions.Android = androidVersion;
                currentVersions.Ios = iosVersion;
            }
            else if (androidVersion != null)
            {
                currentVersions.Unity = string.Format("android_{0}", androidVersion);
                currentVersions.Android = androidVersion;
            }
            else if (iosVersion != null)
            {
                currentVersions.Unity = string.Format("ios_{0}", iosVersion);
                currentVersions.Ios = iosVersion;
            }

            return currentVersions;
        }

        public class Versions
        {
            public string Unity;
            public string Android;
            public string Ios;

            public override bool Equals(object value)
            {
                var versions = value as Versions;

                return versions != null
                       && Unity.Equals(versions.Unity)
                       && (Android == null || Android.Equals(versions.Android))
                       && (Ios == null || Ios.Equals(versions.Ios));
            }

            public bool HasEqualSdkVersions(Versions versions)
            {
                return versions != null
                       && AdapterSdkVersion(Android).Equals(AdapterSdkVersion(versions.Android))
                       && AdapterSdkVersion(Ios).Equals(AdapterSdkVersion(versions.Ios));
            }

            public override int GetHashCode()
            {
                return new { Unity, Android, Ios }.GetHashCode();
            }

            private static string AdapterSdkVersion(string adapterVersion)
            {
                var index = adapterVersion.LastIndexOf(".");
                return index > 0 ? adapterVersion.Substring(0, index) : adapterVersion;
            }
        }

        #endregion
    }
}