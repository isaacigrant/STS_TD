using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BlankRoundData", menuName = "Scriptable Objects/RoundData")]
public class RoundData : ScriptableObject
{
    public WaveData[] Waves;
}
