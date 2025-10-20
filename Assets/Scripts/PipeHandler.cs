using UnityEngine;
using System.Linq;
using System.Collections;

public class PipeHandler : MonoBehaviour
{
    public Transform[] Hearts;
    public Transform[] Coins;
    private void Awake()
    {
    }

    [SerializeField] float showProbability = 50f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int n=0; n < 3; n++)
        {
            Hearts[n].gameObject.SetActive(false);
            Coins[n].gameObject.SetActive(false);
        }
            

        if ( Random.Range(0, 100) < showProbability)
        {
            var r = Random.Range(0, 3);
            if ( Random.Range(0, 2) < 1)
            {
                Hearts[r].gameObject.SetActive(true);
            }
            else
            {
                Coins[r].gameObject.SetActive(true);
            }

            
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
