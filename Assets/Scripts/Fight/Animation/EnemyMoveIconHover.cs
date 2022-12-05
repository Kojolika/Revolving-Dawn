using UnityEngine;

public class EnemyMoveIconHover : MonoBehaviour {
    
    bool goingUp = true;
    float speed = 0.2f;
    Vector3 pos1;
    Vector3 pos2;
    void Start() {
        pos1 = new Vector3(this.transform.position.x,this.transform.position.y + .1f, this.transform.position.z);
        pos2 = new Vector3(this.transform.position.x,this.transform.position.y - .1f, this.transform.position.z);

        this.transform.position = pos1;
    }
    void FixedUpdate() 
    {
        if(this.transform.position.y >= pos1.y) goingUp = !goingUp;
        else if (this.transform.position.y <= pos2.y) goingUp = !goingUp;

        if(goingUp)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, pos1, speed * Time.deltaTime);
        }
        else
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, pos2, speed * Time.deltaTime);
        }
        
    }
}