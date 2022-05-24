using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> target;

    public Serialization(List<T> target)
    {
        this.target = target;
    }

    public List<T> ToList()
    {
        return target;
    }
}

public class Utils
{
    #region Singleton
    public static Utils U = new Utils();
    #endregion

    public IEnumerator ChangeScene(string scene, float waitSeconed = 0)
    {
        if (!PhotonNetwork.IsMasterClient) yield break;
        yield return new WaitForSeconds(waitSeconed);

        PhotonNetwork.DestroyAll();
        PhotonNetwork.LoadLevel(scene);
    }
}
