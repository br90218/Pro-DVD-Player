using UnityEngine;

public class TrailBehaviour : MonoBehaviour
{
    internal Vector3 destination;
    private LineRenderer _lineRenderer;
    internal bool drawLine;

    private void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
        drawLine = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!drawLine)
        {
            _lineRenderer.enabled = false;
            return;
        }

        _lineRenderer.enabled = true;

        _lineRenderer.SetPosition(0, transform.parent.position);
        _lineRenderer.SetPosition(1, destination);

    }   

}
