using System;
using UnityEngine;

/// <summary>
/// Tracks player health and broadcasts changes so UI and other systems can react.
/// Subscribes to <see cref="Enemy.OnEnemyReachedEnd"/> to apply damage when enemies breach the defenses.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Fired whenever player health changes. Passes the new health value as an int.
    /// </summary>
    public static event Action<int> OnHealthChanged;

    [Header("Player Health")]
    [Tooltip("Starting health value. Decreases each time an enemy reaches the end.")]
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
    }
    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
        OnHealthChanged?.Invoke(_currentHealth);
    }

    /// <summary>
    /// Called when an <see cref="Enemy"/> reaches the end of the path.
    /// Reduces health by the enemy's damage value, clamped at zero.
    /// </summary>
    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _currentHealth = Mathf.Max(_currentHealth - data.EnemyDamage, 0);
        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
            HandleGameOver();
    }

    /// <summary>
    /// Called when player health reaches zero.
    /// Placeholder — add scene transition, UI trigger, or analytics here.
    /// </summary>
    private void HandleGameOver()
    {
        Debug.Log("[GameManager] Game over — health reached zero.");
    }
}
