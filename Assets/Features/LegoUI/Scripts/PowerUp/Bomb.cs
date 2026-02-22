
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BombPowerUp", menuName = "ScriptableObjects/PowerUps/Bomb")]
public class Bomb : PowerUpData
{
    public GameObject BombPrefab;
    public GameObject TutorialPrefab;
    public override void ApplyEffect(GameObject dd, object context = null)
    {
        // dd.
        // Instantiate(BombPrefab, dd.transform.position, Quaternion.identity);
        // StartCoroutine(LocalCor(dd.transform));
        Prefabs.Instance.RunCoroutine(LocalCor(dd.transform));

    }
    IEnumerator LocalCor(Transform transform)
    {
        Instantiate(TutorialPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Instantiate(BombPrefab, transform.position, Quaternion.identity);
    }
}