using UnityEngine;

[CreateAssetMenu(fileName = "BlankTowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    public float TowerAttackSpeed;
    public float TowerDamage;
    public float TowerProjectileSpeed;
    public float TowerRange;
}
