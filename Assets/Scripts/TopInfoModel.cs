using UnityEngine;

public class TopInfoModel
{
    public event System.Action OnChange;

    private int _coin;
    public int Coin { 
        get { return _coin; } 
        private set {
            _coin = value;
            OnChange?.Invoke();
        } 
    }
    private int _heart;
    public int Heart { 
        get { return _heart; } 
        private set
        {
            _heart = value;
            OnChange?.Invoke();
        }
    }

    public TopInfoModel(int initialCoin, int initialHeart)
    {
        Coin = initialCoin;
        Heart = initialHeart;
    }

    public void IncCoin(int amount)
    {
        Coin += amount;
        Debug.Log($"refresh Coin:{Coin}");
    }

    public void IncHeart(int amount)
    {
        Heart += amount;
        Debug.Log($"refresh Heart:{Heart}");
    }

    public void DecHeart(int amount)
    {
        Heart = Mathf.Max(0,Heart- amount);

        Debug.Log($"refresh Heart:{Heart}");
    }
}