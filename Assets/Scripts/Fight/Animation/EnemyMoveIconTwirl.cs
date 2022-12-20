using UnityEngine;

public class EnemyMoveIconTwirl : MonoBehaviour 
{
    void LateUpdate() 
    {
        this.transform.Rotate(0f, 0f, .1f, Space.Self);
    }
}