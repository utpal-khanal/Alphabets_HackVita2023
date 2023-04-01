using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyingPlaceHolder : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 1f;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(timeToDestroy);
        Destroy(gameObject);
    }

    void Update()
    {
        
    }
}
