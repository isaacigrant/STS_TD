using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private TowerData _towerData;

    private List<Enemy> _enemiesInRange;
    private BoxCollider2D _towerCollider;

    private void Start()
    {
        _towerCollider = GetComponent<BoxCollider2D>();

        _towerCollider.size = new Vector2(_towerData.TowerRange, _towerData.TowerRange);
        _enemiesInRange = new List<Enemy>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, new Vector3(_towerData.TowerRange, _towerData.TowerRange));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            _enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            
            if (_enemiesInRange.Contains(enemy))
            {
                _enemiesInRange.Remove(enemy);
            }
        }
    }
}
