using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 _projectileDirection;
    private TowerData _towerData;
    private float _projectileDuration = 2;

    public void ShootProjectile(TowerData towerData, Vector3 projectileDirection)
    {
        _towerData = towerData;
        _projectileDirection = projectileDirection;
    }

    private void Update()
    {
        if (_projectileDuration <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            _projectileDuration -= Time.deltaTime;
            transform.position += new Vector3(_projectileDirection.x, _projectileDirection.y) * _towerData.TowerProjectileSpeed * Time.deltaTime;
        }
    }
}
