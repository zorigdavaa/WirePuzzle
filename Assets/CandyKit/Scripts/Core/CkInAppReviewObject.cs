using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
// using Google.Play.Review;
#endif


public class CkInAppReviewObject : MonoBehaviour
{
// #if UNITY_ANDROID
//     private ReviewManager _reviewManager;
//     private PlayReviewInfo _playReviewInfo;
// #endif


//     public void Initialize()
//     {
// #if UNITY_ANDROID
//         _reviewManager = new ReviewManager();
//         StartCoroutine(RequestReviewInfoObject());
// #endif
//     }

//     private IEnumerator RequestReviewInfoObject()
//     {
// #if UNITY_ANDROID
//         var requestFlowOperation = _reviewManager.RequestReviewFlow();
//         yield return requestFlowOperation;
//         if (requestFlowOperation.Error != ReviewErrorCode.NoError)
//         {
//             Debug.LogError("Google Play Review Error: " + requestFlowOperation.Error.ToString());
//             yield break;
//         }
//         _playReviewInfo = requestFlowOperation.GetResult();
// #else
//         yield return null;
// #endif
//     }

//     private IEnumerator LaunchReviewCor()
//     {
// #if UNITY_ANDROID
//         var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
//         yield return launchFlowOperation;
//         _playReviewInfo = null; // Reset the object
//         if (launchFlowOperation.Error != ReviewErrorCode.NoError)
//         {
//             Debug.LogError("Google Play Review Error: " + launchFlowOperation.Error.ToString());
//             yield break;
//         }
// #else
//         yield return null;
// #endif
//     }

//     public void LaunchReview()
//     {
//         StartCoroutine(LaunchReviewCor());
//     }
}
