using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace ElephantSdkManager.Model
{
    [Serializable]
    public class GameKitManifest
    {
        public GameKitSdk data;
    }
    
    [Serializable]
    public class GameKitSdk
    {
        public string version;
        public string mediation;
        public string bundle;
        public string gameId;
        public string gameSecret;
        public string facebookAppId;
        public string adjustAppKey;
        public string facebookClientToken;
        public string appKey;
        public string appKeyIos;
        public string appKeyAndroid;
        public string bannerAdUnitIos;
        public string interstitialAdUnitIos;
        public string rewardedAdUnitIos;
        public string bannerAdUnitAndroid;
        public string interstitialAdUnitAndroid;
        public string rewardedAdUnitAndroid;
        public string googleAppIdIos;
        public string googleAppIdAndroid;
        public List<AdjustEvent> adjustEvents;
        public string amazonAppIdIos;
        public string amazonBannerSlotIdIos;
        public string amazonInterstitialVideoSlotIdIos;
        public string amazonRewardedVideoSlotIdIos;
        public string amazonAppIdAndroid;
        public string amazonBannerSlotIdAndroid;
        public string amazonInterstitialVideoSlotIdAndroid;
        public string amazonRewardedVideoSlotIdAndroid;
        public string fitoBannerAdUnitIos;
        public string fitoInterstitialAdUnitIos;
        public string fitoRewardedAdUnitIos;
        public string fitoBannerAdUnitAndroid;
        public string fitoInterstitialAdUnitAndroid;
        public string fitoRewardedAdUnitAndroid;
        public string helpshiftDomainAndroid;
        public string helpshiftAppIDAndroid;
        public string helpshiftDomainIos;
        public string helpshiftAppIDIos;
    }
    
    [Serializable]
    public class AdjustEvent
    {
        public int id;
        public string name;
        public string token;
    }
}