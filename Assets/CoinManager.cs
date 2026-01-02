using System.Collections;
using UnityEditor;
using UnityEngine;
using ZPackage;

public class CoinManager : GenericSingleton<CoinManager>
{
    // public GameObject CoinPF;
    Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    public void GetCoin(Vector3 worldPos)
    {
        GameObject Coin = Instantiate(Prefabs.Instance.CoinPF, worldPos + Vector3.up, Quaternion.identity, transform);
        StartCoroutine(LocalCor(Coin));

    }
    IEnumerator LocalCor(GameObject coin)
    {
        float t = 0f;
        float time = 0f;
        float duration = 1.0f;
        Vector3 initial = coin.transform.position;
        // Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(cam, Z.CanM.Coin.transform.position);
        // screenPos.z = 50; // or desired distance
        // Vector3 target = cam.ScreenToWorldPoint(screenPos);


        while (time < duration)
        {
            time += Time.deltaTime;
            t = time / duration;
            // coin.transform.position = Vector3.Lerp(initial, target, t);
            yield return null;
        }
        Destroy(coin);
        Z.GM.Coin++;
    }
}
