using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform Target;
    
    private Vector3 initialOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        initialOffset = transform.position - Target.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Target.position + initialOffset;
    }
}
