using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class StatusInfoManager : MonoBehaviour
{

    private TextMeshProUGUI _text;

    public Image BluePanel;
    public Image ScarePanel;
    public Image NoOperationImage;
    public TextMeshProUGUI ScareText;
    public TextMeshProUGUI ResultText; 
    public TextMeshProUGUI Instructions;
    public TextMeshProUGUI Tutorial;
    public TextMeshProUGUI DeathText;


    private int _trackNumber;
    private string _target;
    private string _color;
    private int _par;
    private int _current;
    private string _status;
    private string scareText;
    private float time;

    private Coroutine _noOperationsCoroutine;


    private void Awake() {
        _text = GetComponent<TextMeshProUGUI>();
        _color = string.Empty;
    }

    // Start is called before the first frame update
    void Start()
    {
        BluePanel.enabled = false;
        ScareText.enabled = false;
        ResultText.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void UpdateText()
    {
        string result = string.Empty;
        
        if(GameMaster.Instance.gameMode == GameMaster.GameMode.NORMAL)
        {
            result = "TRACK:" + _trackNumber + "\n";
            result += "TARGET:" + _target;
            if(_color.Length == 0) result += "\n";
            else result += ", " + _color + "\n";


            result += "BOUNCES:" + _current + '\n';

            result += _status;
        }
        else
    {
            result += "TARGET:" + _target;
            if(_color.Length == 0) result += "\n";
            else result += ", " + _color + "\n";
            result += "ELAPSED TIME:" + time.ToString("F0") + "\n";
            result += scareText + "\n";
        }
        

        _text.text = result;
    }




    internal void UpdateTarget(int value)
    {
        switch(value)
        {
            case 0b1001:
                _target = "TOP-RIGHT";
                break;
            case 0b1010:
                _target = "BOT-RIGHT";
                break;
            case 0b0101:
                _target = "TOP-LEFT";
                break;
            case 0b0110:
                _target = "BOT-LEFT";
                break;
        }
    }

    internal void UpdateColor(int value)
    {
        switch(value)
        {
            case -1:
                _color = string.Empty;
                break;
            case 0:
                _color = "RED";
                break;
            case 1:
                _color = "GREEN";
                break;
            case 2:
                _color = "BLUE";
                break;
            case 3:
                _color = "WHITE";
                break;  
        }
    }

    internal void UpdateCurrent(int value)
    {
        _current = value;
    }

    internal void UpdateTrackNumber(int value)
    {
        _trackNumber = value;
    }

    internal void UpdatePlayPause()
    {
        switch(GameMaster.Instance.CurrState)
        {
            case GameMaster.GameState.PLAY:
                _status = ">PLAY";
                break;
            case GameMaster.GameState.PAUSE:
                _status = "||PAUSE";
                break;
            case GameMaster.GameState.REWIND:
                _status = "<<REWIND";
                break;
            case GameMaster.GameState.CLEAR:
                _status = "CLEAR";
                break;
        }
    }

    internal void UpdateTime(float time)
    {
        this.time = time;
    }

    public IEnumerator FlashTextRoutine()
    {
        while(true)
        {
            if(ScareText.enabled == false && Random.Range(0f, 99f) < 20f)
            {
                ScareText.enabled = true;
                yield return new WaitForSeconds(0.1f);
                ScareText.enabled = false;
            }
            yield return new WaitForSeconds(Random.Range(0f,10f));
        }
    }

    public IEnumerator ChangeTextRoutine(int bounces)
    {
        while(true)
        {
            var changed = false;
            if(changed == false && Random.Range(0f, 99f) < 20f)
            {
                ResultText.text = "RESULTS:\nYOU'VE BORED ME "+ bounces +" TIMES.";
                yield return new WaitForSeconds(0.1f);
                ResultText.text = "RESULTS:\nYOU'VE BOUNCED ON THE WALL "+ bounces +" TIMES.";
            }
            yield return new WaitForSeconds(Random.Range(0f,10f));
        }
    }

    public IEnumerator JamTextRoutine()
    {
        while(true)
        {
            if(Random.Range(0f, 99f) < 99f * Mathf.Lerp(1f, 0f, GameMaster.Instance.remainingTime / 15f))
            {
                scareText = "COMEJOINMEMYSOULISLONELYPLEASEGIVEMEYOURSOUL" + Random.Range(0f,999f);
            }
            else
            {
                scareText = string.Empty;
            }

            yield return new WaitForSeconds(Random.Range(0f,4f));
        }
    }

    public IEnumerator NoiseRoutine()
    {
        while(true)
        {
            ScarePanel.color = Color.Lerp(Color.white, Color.clear, GameMaster.Instance.remainingTime / 30f);
            yield return null;
        }
    }

    internal void ShowNoOps()
    {
        if(_noOperationsCoroutine == null)
        {
            _noOperationsCoroutine = StartCoroutine(NoOpsRoutine());
        }
    }

    private IEnumerator NoOpsRoutine()
    {
        NoOperationImage.enabled = true;
        yield return new WaitForSeconds(2f);
        NoOperationImage.enabled = false;
        _noOperationsCoroutine = null;
    }

    internal IEnumerator DeathResultsRoutine(float time)
    {
        yield return new WaitForSeconds(2f);
        DeathText.enabled = true;
        DeathText.text = "YOU'VE MANAGED TO EXTEND YOUR LIFE FOR ANOTHER " + time.ToString("F0") + " SECONDS.";
        yield return new WaitForSeconds(5f);
        DeathText.text = string.Empty;
        yield return new WaitForSeconds(2f);
        DeathText.text = "NOW COME JOIN ME";
        while(true)
        {
            yield return null;
        }
    }

}
