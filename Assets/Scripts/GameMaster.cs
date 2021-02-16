using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameMaster : MonoBehaviour
{

    public static GameMaster Instance;
    private DVDPlayerBehaviour _player;
    [SerializeField] private StatusInfoManager _statusInfo;
    [SerializeField] private AudioManager _audio;
    private bool loadSceneTrigger;


    public enum GameMode
    {
        NORMAL, DEVIL
    }

    public GameMode gameMode;

    internal enum GameState
    {
        PAUSE, PLAY, CLEAR, REWIND, WIN, DEATH
    }

    private GameState _currState;
    internal GameState CurrState { get{return _currState;}}


    private int _wallIsHit;
    private int _targetColorIndex;
    private int _targetCorner;
    private Vector3 _startingPosition;
    private Stack<Vector3> _bouncedPositions;
    protected int _bounces;

    internal float remainingTime = 60f;
    private float _elapsedTime;
    private int _currentLevel;
    private bool _scoreShown = false;

    [SerializeField] private int harderLevel = 5;
    [SerializeField] private int hardestLevel = 7;
    [SerializeField] private int winLevel = 10;


    private void Awake() {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            GameObject.Destroy(this.gameObject);
        }

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<DVDPlayerBehaviour>();
        _bounces = 0;
        _bouncedPositions = new Stack<Vector3>();
        _startingPosition = Vector3.zero;
        _elapsedTime = 0f;
        loadSceneTrigger = false;
        gameMode = ModeDecider.Devil ? GameMode.DEVIL : GameMode.NORMAL;
    }

    // Start is called before the first frame update
    void Start()
    {   
        NewLevel();

        if(gameMode == GameMode.DEVIL)
        {
            StartCoroutine(_statusInfo.NoiseRoutine());
            StartCoroutine(_statusInfo.JamTextRoutine());
        }
        else{
            StartCoroutine(LoadMovieScene());
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(gameMode == GameMode.DEVIL)
        {
            if(CurrState != GameState.DEATH)
            {
                
                remainingTime -= Time.deltaTime;
                if(remainingTime < 0f)
                {
                    ChangeStateTo(GameState.DEATH);
                    return;
                }

                if(_elapsedTime % 1f > (_elapsedTime + Time.deltaTime) % 1f)
                {
                    _statusInfo.UpdateTime(_elapsedTime);
                    _statusInfo.UpdateText();
                }
                _elapsedTime += Time.deltaTime;
            }
        }
    }


    internal void NotifyWallHit(int wall)
    {
        _wallIsHit = (1 << wall | _wallIsHit);
        StartCoroutine(Expire(wall));
        CheckSuccess();
    }

    private void CheckSuccess()
    {
        var color = _player.CurrentColorIndex;

        bool colorWin = (_targetColorIndex + 2) % _player.GetColorLength() == (color + 1) % _player.GetColorLength();

        bool wallWin = _targetCorner == _wallIsHit;
        
        if(_currentLevel <= 5)
        {
            if(wallWin)
            {
                Debug.Log("Win level " + _currentLevel);
                ChangeStateTo(GameState.CLEAR);
                if(gameMode == GameMode.DEVIL) remainingTime += 5f;
                NewLevel();
                return;
            }
        }
        else
        {
            if(colorWin && wallWin)
            {
                Debug.Log("Win level " + _currentLevel);
                ChangeStateTo(GameState.CLEAR);
                if(gameMode == GameMode.DEVIL) remainingTime += 5f;
                NewLevel();
                return;
            }
        }

        _statusInfo.UpdateCurrent(++_bounces);
        _statusInfo.UpdateText();
        _bouncedPositions.Push(_player.transform.position);
        
    }

    private IEnumerator Expire(int wall)
    {
        yield return new WaitForSeconds(0.02f);
        _wallIsHit = _wallIsHit & ~(1 << wall);
    }

    internal void NewLevel(bool restart = false)
    {
        if  (restart) 
        {
            _currentLevel = 0;
            _bounces = 0;
            _statusInfo.UpdateCurrent(_bounces);
        }

        if(gameMode == GameMode.NORMAL && _currentLevel == winLevel)
        {
            ChangeStateTo(GameState.WIN);
            return;
        }
        
        _currentLevel++;
        DecideCorner();
        DecideColor((_currentLevel <= harderLevel ? -1 : 0));  

        if(_currentLevel > hardestLevel)
        {
            DecidePlayerPosition(true);
        }
        else
        {
            DecidePlayerPosition(false);
        }
        _statusInfo.UpdateTrackNumber(_currentLevel);
        _statusInfo.UpdateText();
        _bouncedPositions.Clear();
        ChangeStateTo(GameState.PAUSE);
    }
   
    internal void DecideCorner()
    {
        var random = new System.Random();
        _targetCorner = 0;

        _targetCorner = _targetCorner | 1 << (random.Next(10) < 5 ? 0 : 1);
        _targetCorner = _targetCorner | 1 << (random.Next(10) < 5 ? 2 : 3);
        _statusInfo.UpdateTarget(_targetCorner);
        _statusInfo.UpdateText();
    }

    internal void DecideColor(int flag = 0)
    {
        if(flag == -1)
        {
            _statusInfo.UpdateColor(-1);
            return;
        }
        _targetColorIndex = Random.Range(0, _player.GetColorLength());
        _statusInfo.UpdateColor(_targetColorIndex);
    }

    internal void DecidePlayerPosition(bool random = false)
    {
        if(random)
        {
            var newPos = Random.insideUnitSphere * 3f;
            newPos.z = 0f;
            _player.transform.position = newPos;
        }
        else
        {
            _player.transform.position = Vector3.zero;
        }
        _startingPosition = _player.transform.position;
    }

    internal void Rewind()
    {
        if(_currState != GameState.PLAY)
        {
            if(_currState == GameState.WIN)
            {
                if(_scoreShown == false)
                {
                    NoOps();
                    return;
                }
                NewLevel(true);
                return;
            }
            else if(gameMode == GameMode.NORMAL && _bounces == 0 && _bouncedPositions.Count == 0 && _currentLevel == 1)
            {
                loadSceneTrigger = true;
                return;
            }
            GameMaster.Instance.NoOps();
            return;
        }
        StartCoroutine(RewindRoutine());
    }
    
    private IEnumerator RewindRoutine()
    {
        ChangeStateTo(GameState.REWIND);
        _player.MovingVector = Vector3.zero;

        while(_bouncedPositions.Count > 0)
        {
            var nextDest = _bouncedPositions.Pop();
            
            while(Vector3.Distance(_player.transform.position, nextDest) > 0.25f)
            {
                _player.transform.position = Vector3.MoveTowards(_player.transform.position, nextDest, Time.deltaTime * 20f);
                yield return null;
            }

            if(_bouncedPositions.Count > 0) _player.ChangeColor();
        }

        _player.transform.position = _startingPosition;

        ChangeStateTo(GameState.PAUSE);
    }

    internal void Play(Vector3 pos)
    {
        if(_currState != GameState.PAUSE)
        {


            return;
        }
        
        ChangeStateTo(GameState.PLAY);
        _bouncedPositions.Push(_startingPosition);
        _player.SetTargetDirection(pos);



    }


    internal void ChangeStateTo(GameState state)
    {
        _currState = state;
        if(_currState == GameState.WIN)
        {
            StartCoroutine(WinRoutine());
            return;
        }

        if(_currState == GameState.DEATH)
        {
            StartCoroutine(DeathRoutine());
            return;
        }

        _statusInfo.UpdatePlayPause();
        _statusInfo.UpdateText();
    }
    
    private IEnumerator WinRoutine()
    {
        _statusInfo.BluePanel.enabled = true;

        _statusInfo.Tutorial.enabled = false;
        yield return new WaitForSeconds(3f);
        _statusInfo.ResultText.text = "RESULTS:\nYOU'VE BOUNCED ON THE WALL "+ _bounces +" TIMES.";
        _statusInfo.ResultText.enabled = true;
        yield return new WaitForSeconds(2f);
        _statusInfo.Instructions.enabled = true;
        
        var routine1 = StartCoroutine(_statusInfo.FlashTextRoutine());
        var routine2 = StartCoroutine(_statusInfo.ChangeTextRoutine(_bounces));
        _scoreShown = true;
        while(CurrState == GameState.WIN)
        {
            yield return null;
        }

        StopCoroutine(routine1);
        StopCoroutine(routine2);

        _statusInfo.BluePanel.enabled = false;
        _statusInfo.ResultText.enabled = false;
        _statusInfo.Instructions.enabled = false;
        _statusInfo.Tutorial.enabled = true;
        _scoreShown = false;
    }

    private IEnumerator DeathRoutine()
    {
        StartCoroutine(_statusInfo.DeathResultsRoutine(_elapsedTime));
        yield return new WaitForSeconds(Random.Range(14f, 20f));
        _audio.PlayDeath();
        yield return new WaitForSeconds(1.7f);
        Application.Quit();
    }

    internal void NoOps()
    {
        _statusInfo.ShowNoOps();
    }

    private IEnumerator LoadMovieScene()
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);
        asyncOperation.allowSceneActivation = false;

        while(!asyncOperation.isDone)
        {
            if(asyncOperation.progress >= 0.9f)
            {
                if(loadSceneTrigger == true)
                {
                    asyncOperation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }
}
