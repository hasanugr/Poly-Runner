using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager instance;

    [Header("ADMOB Settings")]
    public bool IsTestMode;
    public string InterstitialID = "ca-app-pub-8338191387048913/6619351570";
    public string RewardedID = "ca-app-pub-8338191387048913/3158139788";
    [HideInInspector]
    public bool IsUserEarnedReward = false;

    private InterstitialAd Interstitial;
    private RewardedAd Rewarded;

    public delegate void AwardWon();
    public event AwardWon OnAwardWon;

    // Start is called before the first frame update
    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        
        if (IsTestMode)
        {
            InterstitialID = "ca-app-pub-3940256099942544/1033173712";
            RewardedID = "ca-app-pub-3940256099942544/5224354917";
        }

        RequestInterstitial();
        RequestRewardedAd();
    }

    void Awake()
    {
        MakeSingleton();
    }

#region Public Functions

    public void ShowInterstitial()
    {
        if (Interstitial.IsLoaded())
        {
            Interstitial.Show();
        }
    }

    public void ShowRewardedAd()
    {
        if (Rewarded.IsLoaded())
        {
            Rewarded.Show();
        }
    }

#endregion

#region Private Functions

    private void RequestInterstitial()
    {
        Interstitial = new InterstitialAd(InterstitialID);
        Interstitial.OnAdClosed += Interstitial_OnAdClosed;
        AdRequest request = new AdRequest.Builder().Build();
        Interstitial.LoadAd(request);
    }

    private void Interstitial_OnAdClosed(object sender, System.EventArgs e)
    {
        RequestInterstitial();
    }

    private void RequestRewardedAd()
    {
        Rewarded = new RewardedAd(RewardedID);
        Rewarded.OnUserEarnedReward += Rewarded_OnUserEarnedReward;
        Rewarded.OnAdClosed += Rewarded_OnAdClosed;
        AdRequest request = new AdRequest.Builder().Build();
        Rewarded.LoadAd(request);
    }

    private void Rewarded_OnAdClosed(object sender, System.EventArgs e)
    {
        RequestRewardedAd();
    }

    private void Rewarded_OnUserEarnedReward(object sender, Reward e)
    {
        IsUserEarnedReward = true;
        OnAwardWon();
    }

    private void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
#endregion
}
