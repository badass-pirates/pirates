using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPile : MonoBehaviour
{
    public GameObject coin;

    GameObject coinPile;
    Vector3 coinPilePos;

    public CoinPile()
    {
        coin = Resources.Load<GameObject>("Coin");
        coinPile = null;
        coinPilePos = new Vector3(0, 0, 0);
    }

    public CoinPile(GameObject obj)
    {
        coin = Resources.Load<GameObject>("Coin");
        coinPile = obj;
        coinPilePos = coinPile.transform.position;
    }

    public int Count()
    {
        if (coinPile != null) return coinPile.transform.childCount;
        return 0;
    }

    public int AddCoins(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(coin, coinPilePos + new Vector3(0, 0, 0), Quaternion.identity);
            if (coinPile != null)
                obj.transform.parent = coinPile.transform;
        }
        return Count();
    }

    public int AddCoin()
    {
        return AddCoins(1);
    }

    public int RemoveCoins(int count)
    {
        int startIndex = Count() - count;
        for (int i = 0; i < count; i++)
            Destroy(this.transform.GetChild(i));

        return Count();
    }

    public int RemoveCoinAt(int index)
    {
        Destroy(this.transform.GetChild(index));
        return Count();
    }

    public int RemoveCoinAll()
    {
        RemoveCoins(Count());

        return Count();
    }
}
