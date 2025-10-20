using UnityEngine;
using System.Collections;
using NUnit.Framework.Api;

/// <summary>
/// Spritesheet for Flappy Bird found here: http://www.spriters-resource.com/mobile_phone/flappybird/sheet/59537/
/// Audio for Flappy Bird found here: https://www.sounds-resource.com/mobile/flappybird/sound/5309/
/// </summary>
public class FlappyScript : MonoBehaviour
{

    public AudioClip FlyAudioClip, DeathAudioClip, ScoredAudioClip;
    public Sprite GetReadySprite;
    public float RotateUpSpeed = 1, RotateDownSpeed = 1;
    public GameObject IntroGUI, DeathGUI;
    public Collider2D restartButtonGameCollider;
    public float VelocityPerJump = 3;
    public float XSpeed = 1;
    private Rigidbody2D rbody;
    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
    }
    // Use this for initialization
    void Start()
    {

    }

    FlappyYAxisTravelState flappyYAxisTravelState;

    enum FlappyYAxisTravelState
    {
        GoingUp, GoingDown
    }

    Vector3 birdRotation = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        //handle back key in Windows Phone
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (GameStateManager.GameState == GameState.Intro)
        {
            MoveBirdOnXAxis();
            if (WasTouchedOrClicked())
            {
                BoostOnYAxis();
                GameStateManager.GameState = GameState.Playing;
                IntroGUI.SetActive(false);
                ScoreManagerScript.Score = 0;
            }
        }

        else if (GameStateManager.GameState == GameState.Playing)
        {
            MoveBirdOnXAxis();
            if (WasTouchedOrClicked())
            {
                BoostOnYAxis();
            }

        }

        else if (GameStateManager.GameState == GameState.Dead)
        {
            Vector2 contactPoint = Vector2.zero;

            if (Input.touchCount > 0)
                contactPoint = Input.touches[0].position;
            if (Input.GetMouseButtonDown(0))
                contactPoint = Input.mousePosition;

            //check if user wants to restart the game
            if (restartButtonGameCollider == Physics2D.OverlapPoint
                (Camera.main.ScreenToWorldPoint(contactPoint)))
            {
                GameStateManager.GameState = GameState.Intro;
                Application.LoadLevel(Application.loadedLevelName);
            }
        }

    }


    void FixedUpdate()
    {
        //just jump up and down on intro screen
        if (GameStateManager.GameState == GameState.Intro)
        {
            if (rbody.linearVelocity.y < -1) //when the speed drops, give a boost
                rbody.AddForce(new Vector2(0, rbody.mass * 5500 * Time.deltaTime)); //lots of play and stop 
                                                        //and play and stop etc to find this value, feel free to modify
        }
        else if (GameStateManager.GameState == GameState.Playing || GameStateManager.GameState == GameState.Dead)
        {
            FixFlappyRotation();
        }
    }

    bool WasTouchedOrClicked()
    {
        if (Input.GetButtonUp("Jump") || Input.GetMouseButtonDown(0) || 
            (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended))
            return true;
        else
            return false;
    }

    void MoveBirdOnXAxis()
    {
        transform.position += new Vector3(Time.deltaTime * XSpeed, 0, 0);
    }

    void BoostOnYAxis()
    {
        rbody.linearVelocity = new Vector2(0, VelocityPerJump);
        GetComponent<AudioSource>().PlayOneShot(FlyAudioClip);
    }



    /// <summary>
    /// when the flappy goes up, it'll rotate up to 45 degrees. when it falls, rotation will be -90 degrees min
    /// </summary>
    private void FixFlappyRotation()
    {
        if (rbody.linearVelocity.y > 0) flappyYAxisTravelState = FlappyYAxisTravelState.GoingUp;
        else flappyYAxisTravelState = FlappyYAxisTravelState.GoingDown;

        float degreesToAdd = 0;

        switch (flappyYAxisTravelState)
        {
            case FlappyYAxisTravelState.GoingUp:
                degreesToAdd = 6 * RotateUpSpeed;
                break;
            case FlappyYAxisTravelState.GoingDown:
                degreesToAdd = -3 * RotateDownSpeed;
                break;
            default:
                break;
        }
        //solution with negative eulerAngles found here: http://answers.unity3d.com/questions/445191/negative-eular-angles.html

        //clamp the values so that -90<rotation<45 *always*
        birdRotation = new Vector3(0, 0, Mathf.Clamp(birdRotation.z + degreesToAdd, -90, 45));
        transform.eulerAngles = birdRotation;
    }

    /// <summary>
    /// check for collision with pipes
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (GameStateManager.GameState == GameState.Playing)
        {
            if (col.gameObject.tag == "Pipeblank") //pipeblank is an empty gameobject with a collider between the two pipes
            {
                GetComponent<AudioSource>().PlayOneShot(ScoredAudioClip);
                ScoreManagerScript.Score++;
            }
            else if (col.gameObject.tag == "Pipe")
            {
                if(TopInfoPresenter.Instance.IsExistHeart())
                {
                    //goto blink
                    BlinkAndGhost(1f);
                    DecHeart(1);
                }
                else
                {
                    FlappyDies();
                }
                    
            }

            if(col.gameObject.tag == "Coin")
            {
                IncCoin(1);
                DestroyCoin(col.gameObject);
            }
            if (col.gameObject.tag == "Heart")
            {
                IncHeart(1);
                DestroyHeart(col.gameObject);
            }
        }
    }
    IEnumerator coBlinkAndGhost(float duration)
    {
        float startTime = Time.time;
        var collider = GetComponent<Collider2D>();
        collider.enabled = false;

        var sprCom = GetComponent<SpriteRenderer>();
        while( Time.time < startTime + duration)
        {
            if (Time.frameCount % 2 == 0)
                sprCom.color = Color.red;
            //sprCom.enabled = false;
            else
                sprCom.color = Color.white;
            //sprCom.enabled = true;

            yield return new WaitForSeconds(0.01f);
        }
        //yield return new WaitForSeconds(duration);
        sprCom.color = Color.white;
        collider.enabled = true;
        yield return null;
    }

    Coroutine coHandle = null;

    void BlinkAndGhost(float duration)
    {
        if (coHandle != null)
            StopCoroutine(coHandle);
            coHandle = StartCoroutine(coBlinkAndGhost(duration));
    }

    void IncCoin(int amount)
    {
        TopInfoPresenter.Instance.IncCoin(amount);
    }

    void DecHeart(int amount)
    {
        TopInfoPresenter.Instance.DecHeart(amount);
    }
    void IncHeart(int amount)
    {
        TopInfoPresenter.Instance.IncHeart(amount);
    }

    void DestroyHeart(GameObject heart)
    {
        heart.SetActive(false);
    }

    void DestroyCoin(GameObject coin)
    {
        coin.SetActive(false);
    }

    [SerializeField]

    void OnCollisionEnter2D(Collision2D col)
    {
        if (GameStateManager.GameState == GameState.Playing)
        {
            if (col.gameObject.tag == "Floor")
            {
                FlappyDies();
            }
        }
    }

    void FlappyDies()
    {
        GameStateManager.GameState = GameState.Dead;
        DeathGUI.SetActive(true);
        GetComponent<AudioSource>().PlayOneShot(DeathAudioClip);
    }

}
