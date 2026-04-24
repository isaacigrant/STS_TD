using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject _poolerPrefab;
    [SerializeField] private int _poolSize;

    private List<GameObject> _poolList;

    /// <summary>
    /// <para>Checks each object in the Pool List, if one is inactive return that object. Else create a new object and use it.</para>
    /// <see cref="CreateNewObject"/>
    /// </summary>
    public GameObject GetObjectInPool()
    {
        foreach (GameObject obj in _poolList)
        {
            if (!obj.activeSelf)
            {
                return obj;
            }
        }

        return CreateNewObject();
    }

    private void Start()
    {
        _poolList = new List<GameObject>();

        for (int i = 0; i < _poolSize; i++)
        {
            CreateNewObject();
        }
    }

    /// <summary>Instantiate's a new object, deactivates it, adds it to the pool list and then returns the new object.</summary>
    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(_poolerPrefab, transform);
        obj.SetActive(false);
        _poolList.Add(obj);
        return obj;
    }
}
