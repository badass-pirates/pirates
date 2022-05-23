using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class Serialization<T>
{
    public Serialization(List<T> _target) => target = _target;
    public List<T> target;
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
