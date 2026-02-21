
using UnityEngine;

[CreateAssetMenu(fileName = "BombPowerUp", menuName = "ScriptableObjects/PowerUps/Bomb")]
public class Bomb : PowerUpData
{
    public GameObject BombPrefab;
    public override void ApplyEffect(GameObject dd, object context = null)
    {
        // dd.
        Instantiate(BombPrefab, dd.transform.position, Quaternion.identity);
    }
}