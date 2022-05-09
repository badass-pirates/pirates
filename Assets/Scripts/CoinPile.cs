using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPile : MonoBehaviour
{
    public GameObject coin;
    List<GameObject> coins;

    public int AddCoins(int count)
    {
        for(int i = 0 ; i < count; i++)
            coins.Add(Instantiate(coin, transform.position, Quaternion.identity));
            
        return  coins.Count;
    }
    
    public int AddCoin()
    {
        return AddCoins(1);
    }

    public int RemoveCoins(int count)
    {
        int startIndex = coins.Count - count - 1;
        for(int i = 0; i < count; i++)
            Destroy(coins[startIndex + i]);
        coins.RemoveRange(startIndex, count);

        return coins.Count;
    }

    public int RemoveCoinAt(int index)
    {
        Destroy(coins[index]);
        coins.RemoveAt(index);
        
        return coins.Count;
    }

    public int RemoveCoinAll()
    {
        RemoveCoins(coins.Count);

        return coins.Count;
    }
}
