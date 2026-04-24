using UnityEngine;

[CreateAssetMenu(fileName = "BlankEnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float EnemyDamage;
    public float EnemyHealth;
    public float EnemySpeed;
}
