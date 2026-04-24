using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject _poolerPrefab;
    [SerializeField] private int _poolSize;

    private List<GameObject> _poolList;

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

    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(_poolerPrefab, transform);
        obj.SetActive(false);
        _poolList.Add(obj);
        return obj;
    }
}
