
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using ZPackage;

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
        Prefabs.Instance.RunCoroutine(LocalCor());

    }
    IEnumerator LocalCor()
    {
        var insTutorial = Instantiate(TutorialPrefab, Vector3.zero, Quaternion.identity, Z.CanM.transform);
        RectTransform rect = insTutorial.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);

        rect.pivot = new Vector2(0.5f, 0f);

        rect.anchoredPosition = new Vector2(0f, 100f);
        // rect.localScale = Vector3.one;
        // rect.localRotation = Quaternion.identity;
        bool isDone = false;
        Transform position = null;
        Z.Player.RequestSelected((selected) =>
        {
            isDone = true;
            position = selected;
        });

        while (!isDone)
        {
            Debug.Log("Waiting for player to select a position...");
            yield return null;
        }
        Debug.Log("Selection made, instantiating bomb...");
        var insBomb = Instantiate(BombPrefab, position.position, Quaternion.identity);
        Destroy(insBomb.gameObject, 1.5f);
        Destroy(insTutorial.gameObject);
        yield return new WaitForSeconds(1f);
        insBomb.transform.GetChild(1).gameObject.SetActive(true);
    }
}