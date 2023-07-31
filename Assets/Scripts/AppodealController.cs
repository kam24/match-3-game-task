using AppodealStack.ConsentManagement.Api;
using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;
using UnityEngine;

namespace Assets.Scripts
{
    public class AppodealController : MonoBehaviour
    {
        private const string _appKey = "54254b724336509d88889c393995b7b963c38b04a7b8bc4d";

        private void Start()
        {
            Initialize();
            ShowBannerTop();
            ShowInterstitial();
        }

        public void Initialize()
        {
            Appodeal.SetTesting(true);
            Appodeal.MuteVideosIfCallsMuted(true);
            int adTypes = AppodealAdType.Mrec | AppodealAdType.Interstitial | AppodealAdType.Banner;
            Appodeal.Initialize(_appKey, adTypes);
        }

        public void ShowInterstitial()
        {
            if (Appodeal.IsLoaded(AppodealAdType.Interstitial) && Appodeal.CanShow(AppodealAdType.Interstitial, "default") && !Appodeal.IsPrecache(AppodealAdType.Interstitial))
            {
                Appodeal.Show(AppodealShowStyle.Interstitial);
            }
            else if (!Appodeal.IsAutoCacheEnabled(AppodealAdType.Interstitial))
            {
                Appodeal.Cache(AppodealAdType.Interstitial);
            }
        }

        public void ShowBannerTop()
        {
            Appodeal.Show(AppodealShowStyle.BannerTop, "default");
            Appodeal.Show(AppodealAdType.Banner);
        }

        public void HideBannerView()
        {
            Appodeal.HideBannerView();
        }

        public void ShowMrecView()
        {
            Appodeal.ShowMrecView(AppodealViewPosition.VerticalTop, AppodealViewPosition.HorizontalCenter, "default");
        }

        public void HideMrecView()
        {
            Appodeal.HideMrecView();
        }
    }
}
