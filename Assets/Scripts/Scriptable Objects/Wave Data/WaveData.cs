using UnityEngine;

[CreateAssetMenu(fileName = "BlankWaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    public EnemyType EnemyWaveType;
    public float SpawnInterval;
    public int EnemiesPerWave;
    public int TimeUntilNextWave;
}
