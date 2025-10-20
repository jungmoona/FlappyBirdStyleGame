using UnityEngine;

public class TopInfoView : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI coinText;
    [SerializeField] TMPro.TextMeshProUGUI heartText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void SetCoin(int coin)
    {
        Debug.Log("SetCoin:" + coin);
        coinText.text= coin.ToString();
    }

    public void SetHeart(int heart)
    {
        Debug.Log("SetHeart:" + heart); 
        heartText.text = heart.ToString();
    }

    //public void SetScore(int score)
    //{
    //    scoreText.text = score.ToString();
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
