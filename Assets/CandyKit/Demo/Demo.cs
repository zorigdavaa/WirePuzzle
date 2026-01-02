using UnityEngine;
using UnityEngine.SceneManagement;
using CandyKitSDK;

public class Demo : MonoBehaviour
{
    [SerializeField] private GameObject m_RewardedAdFeedbackText;
    [SerializeField] private RectTransform m_BannerPlaceholder;

    private void Start()
    {
        HideRewardedAdSuccessFeedback();

        CandyKit.ShowBanner();
    }

    public void CallInterstitialAd()
    {
        CandyKit.ShowInterstitial("demoInterstitial", () =>
        {
            Debug.Log("Interstitial displayed");
        });
    }

    public void CallRewardedVideoAd()
    {
        CandyKit.ShowRewardedVideo("DemoAd", (isSuccess) =>
        {
            if (isSuccess)
            {
                m_RewardedAdFeedbackText.SetActive(true);
                Invoke("HideRewardedAdSuccessFeedback", 2f);
            }
            else
            {
                Debug.Log("Rewarded ad failed");
            }
        });
    }

    private void HideRewardedAdSuccessFeedback()
    {
        m_RewardedAdFeedbackText.SetActive(false);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(gameObject.scene.buildIndex);
    }
}
