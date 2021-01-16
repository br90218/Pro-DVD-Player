using UnityEngine;

public class WallBehaviour : MonoBehaviour
{
    private enum WallDirection{
        UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3
    }

    [SerializeField] private WallDirection _wallDirection;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            GameMaster.Instance.NotifyWallHit((int) _wallDirection);
            var otherDVD = other.gameObject.GetComponent<DVDPlayerBehaviour>();
            otherDVD.ChangeColor();
            ChangeDirection(otherDVD);
        }    
    }

    private void ChangeDirection(DVDPlayerBehaviour otherDVD)
    {
        var vector = otherDVD.MovingVector;
        var multiplier = GameMaster.Instance.CurrState == GameMaster.GameState.REWIND ? 5f : 1f;
        if(_wallDirection == WallDirection.UP || _wallDirection == WallDirection.DOWN)
        {
            
            otherDVD.MovingVector = new Vector3(vector.x, -vector.y, 0f).normalized * multiplier;
        }
        else
        {
            otherDVD.MovingVector = new Vector3(-vector.x, vector.y, 0f).normalized * multiplier;
        }
    }
}
