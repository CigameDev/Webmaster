using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling instance;
    private List<GameObject> listObject = new List<GameObject>();
    [SerializeField] GameObject noteTextPrefab;
    [SerializeField] int amountToPool = 5;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }    
        else
        {
            Destroy(instance);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for(int i=0;i<amountToPool; i++)
        {
            GameObject obj = Instantiate(noteTextPrefab);
            obj.SetActive(false);
            listObject.Add(obj);
        }
    }

    public GameObject GetPoolObject()
    {
        for(int i =0;i < listObject.Count;i++)
        {
            if (!listObject[i].activeInHierarchy)return listObject[i];
        }    

        GameObject newObj = Instantiate(noteTextPrefab);
        listObject.Add(newObj);
        return newObj;
    }   
    
    public void ReturnPooling(GameObject obj)
    {
        obj.SetActive(false);
    }    
}
