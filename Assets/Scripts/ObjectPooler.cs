using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages a reusable pool of GameObjects to avoid repeated Instantiate/Destroy calls.
/// Attach to an empty GameObject and assign a prefab and initial pool size in the Inspector.
/// </summary>
public class ObjectPooler : MonoBehaviour
{
    [Header("Pool Settings")]
    [Tooltip("The prefab to pool.")]
    [SerializeField] private GameObject _prefab;
    [Tooltip("Number of instances created on Start. Pool will grow automatically if exhausted.")]
    [SerializeField] private int _initialSize = 10;

    private List<GameObject> _pool;

    private void Start()
    {
        _pool = new List<GameObject>(_initialSize);

        for (int i = 0; i < _initialSize; i++)
        {
            CreateAndRegister();
        }
    }

    /// <summary>
    /// Instantiates a new instance of the prefab, deactivates it, and registers it in the pool.
    /// </summary>
    /// <returns>The newly created inactive GameObject.</returns>
    private GameObject CreateAndRegister()
    {
        GameObject obj = Instantiate(_prefab, transform);
        obj.SetActive(false);
        _pool.Add(obj);
        return obj;
    }

    /// <summary>
    /// Returns the first inactive object in the pool.
    /// If all objects are active, a new one is created and added to the pool.
    /// </summary>
    /// <returns>An inactive pooled GameObject, ready to be activated and used.</returns>
    public GameObject GetPooledObject()
    {
        foreach (GameObject obj in _pool)
        {
            if (!obj.activeSelf)
            {
                return obj;
            }
        }

        return CreateAndRegister();
    }
}
