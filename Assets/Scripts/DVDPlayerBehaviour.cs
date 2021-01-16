using UnityEngine;

public class DVDPlayerBehaviour : MonoBehaviour
{
    [SerializeField] private Color[] _colors;
    public int CurrentColorIndex {get; private set;}
    private Renderer _renderer;
    
    internal Vector3 MovingVector;
    public bool IsMoved {get; private set;}


    private void Awake()
    {   
        CurrentColorIndex = 0;
        _renderer = GetComponent<Renderer>();
        _renderer.sharedMaterial.color = _colors[CurrentColorIndex];
        MovingVector = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    
    }
    
    private void FixedUpdate() {
        if(GameMaster.Instance.CurrState == GameMaster.GameState.PLAY)
        {
            transform.position += MovingVector * Time.fixedDeltaTime * 5f;
        }
    }


    internal void ChangeColor()
    {
        var coefficient = GameMaster.Instance.CurrState == GameMaster.GameState.REWIND ? -1 : 1;     
        CurrentColorIndex = (CurrentColorIndex + coefficient + _colors.Length ) % _colors.Length;
        _renderer.sharedMaterial.color = _colors[CurrentColorIndex];
    }

    internal int GetColorLength()
    {
        return _colors.Length;
    }

    internal void SetTargetDirection(Vector3 target, float speed = 1f)
    {
        var vector = target - transform.position;
        MovingVector = vector.normalized * speed;
        IsMoved = true;
    }
}
