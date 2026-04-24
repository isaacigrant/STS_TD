using UnityEngine;

[CreateAssetMenu(fileName = "BlankWaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    public EnemyType EnemyWaveType;
    public int EnemiesPerWave;
    public float SpawnInterval;
}
