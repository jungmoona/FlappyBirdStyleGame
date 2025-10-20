using System.Collections;
using UnityEngine;

public class TopInfoPresenter : MonoBehaviour
{
    static public TopInfoPresenter Instance { get; private set; }
    private TopInfoModel _model;
    [SerializeField] TopInfoView _view;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _model = new TopInfoModel(initialCoin: 0, initialHeart: 3);
        _model.OnChange += () =>
        {
            Debug.Log($"TopInfoPresenter detected model change Coin:{_model.Coin} Heart:{_model.Heart}");
            _view.SetCoin(_model.Coin);
            _view.SetHeart(_model.Heart);
        };
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("start TopInfoPresenter");
        _model.IncHeart(0);
        _model.IncCoin(0);
    }
    public void IncCoin(int amount)
    {
        _model.IncCoin(amount);
    }

    public void IncHeart(int amount)
    {
        _model.IncHeart(amount);
    }

    public void DecHeart(int amount)
    {
        _model.DecHeart(amount);
    }

    public bool IsExistHeart()
    {
        return _model.Heart > 0;
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
