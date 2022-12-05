using UnityEngine;

public class EnemyMoveIconTwirl : MonoBehaviour 
{
    void FixedUpdate() 
    {
        this.transform.Rotate(0f, 0f, .9f, Space.Self);
    }
}