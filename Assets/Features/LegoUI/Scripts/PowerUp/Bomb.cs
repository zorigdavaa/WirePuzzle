
using UnityEngine;

[CreateAssetMenu(fileName = "BombPowerUp", menuName = "ScriptableObjects/PowerUps/Bomb")]
public class Bomb : PowerUpData
{
    public GameObject BombPrefab;
    public override void ApplyEffect(GameObject gameObject, object context = null)
    {
        Instantiate(BombPrefab, gameObject.transform.position, Quaternion.identity);
    }
}