using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Utils
{
    #region Singleton
    public static Utils U = new Utils();
    #endregion

    public IEnumerator ChangeScene(string scene, float waitSeconed = 0)
    {
        yield return new WaitForSeconds(waitSeconed);

        PhotonView[] PVs = GameObject.FindObjectsOfType<PhotonView>();
        foreach (PhotonView pv in PVs)
        {
            GameObject.Destroy(pv);
        }
        PhotonNetwork.LoadLevel(scene);
    }
}
