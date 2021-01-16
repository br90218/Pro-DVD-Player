using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private DVDPlayerBehaviour _playerBehaviour;
    private Vector2 _screenPointerLocation;
    
    private TrailBehaviour _trail;

    private void Awake() {
        _playerBehaviour = GetComponent<DVDPlayerBehaviour>();
        _trail = GetComponentInChildren<TrailBehaviour>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
#if UNITY_STANDALONE

        if(Input.GetMouseButton(0))
        {
           if (GameMaster.Instance.CurrState == GameMaster.GameState.PAUSE)
           {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0f;

                _trail.destination = pos;
                _trail.drawLine = true;
           }
        }
        else
        {
            _trail.drawLine = false;
        }

        if(Input.GetMouseButtonUp(0))
        {
            if(GameMaster.Instance.CurrState == GameMaster.GameState.PAUSE)
            {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0f;
                GameMaster.Instance.Play(pos);
                return;
            }
            GameMaster.Instance.NoOps();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            GameMaster.Instance.Rewind();
        }

        if(Input.GetKeyDown(KeyCode.F1))
        {
            GameMaster.Instance.NewLevel();
        }




#elif UNITY_ANDROID
#endif
    }


}
