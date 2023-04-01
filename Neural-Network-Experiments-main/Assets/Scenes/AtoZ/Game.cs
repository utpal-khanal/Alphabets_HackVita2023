using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] GameObject[] allDigitsGameObj;
    [SerializeField] AudioClip[] allSounds;
    int a;
    [SerializeField] float soundDelay = 0.5f;
    //[SerializeField] Vector3 ObjPosition;

    public GameObject InstantiateADigit(int a)
    {
        this.a = a;
        GameObject go = Instantiate(allDigitsGameObj[a], gameObject.transform.position, Quaternion.identity);
        Invoke("GiveSound", soundDelay);
        go.transform.localScale = new Vector3(8, 8, 8);
        go.transform.localRotation = Quaternion.Euler(0, -180, 0);
        return go;
    }

    public GameObject InstantiateADigitForTheGame(int a , Vector3 position)
    {
        this.a = a;
        GameObject go = Instantiate(allDigitsGameObj[a], position, Quaternion.identity);
        Invoke("GiveSound", soundDelay);
        go.transform.localScale = new Vector3(3, 3, 3);
        go.transform.localRotation = Quaternion.Euler(0, -180, 0);
        return go;
    }

    public void GiveSound()
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(allSounds[a]);
    }
}
