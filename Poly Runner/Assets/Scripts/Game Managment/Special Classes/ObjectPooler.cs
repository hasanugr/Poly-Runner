using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler
{
    private GameObject _objectPrefab;
    private Stack<GameObject> _objectPool = new Stack<GameObject>();

    public ObjectPooler(GameObject prefab)
    {
        this._objectPrefab = prefab;
    }

    public void FillThePool(int miktar)
    {
        for (int i = 0; i < miktar; i++)
        {
            GameObject obj = Object.Instantiate(_objectPrefab);
            SendObjectToPool(obj);
        }
    }

    public GameObject GetObjectFromPool()
    {
        if (_objectPool.Count > 0)
        {
            GameObject obj = _objectPool.Pop();
            obj.gameObject.SetActive(true);

            return obj;
        }

        return Object.Instantiate(_objectPrefab);
    }

    public GameObject GetObjectFromPoolAtPosition(Vector3 pos, Quaternion rot)
    {
        if (_objectPool.Count > 0)
        {
            GameObject obj = _objectPool.Pop();
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.gameObject.SetActive(true);

            return obj;
        }

        return Object.Instantiate(_objectPrefab);
    }

    public void SendObjectToPool(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        _objectPool.Push(obj);
    }
}