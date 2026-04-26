using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<int> OnLivesChanged;

    private int _playerHealth = 100;

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleOnEnemyReachedEnd;
    }
    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleOnEnemyReachedEnd;
    }

    private void Start()
    {
        OnLivesChanged?.Invoke(_playerHealth);
    }

    private void HandleOnEnemyReachedEnd(EnemyData data)
    {
        _playerHealth = Mathf.Clamp(_playerHealth - data.EnemyDamage, 0, 100);
        OnLivesChanged?.Invoke(_playerHealth);
    }
}
