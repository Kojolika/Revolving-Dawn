using UnityEngine;

public class EnemyMoveIconHover : MonoBehaviour {
    
    bool goingUp = true;
    float speed = 0.03f;
    Vector3 pos1;
    Vector3 pos2;
    void Start() {
        pos1 = new Vector3(this.transform.localPosition.x,this.transform.localPosition.y + .02f, this.transform.localPosition.z);
        pos2 = new Vector3(this.transform.localPosition.x,this.transform.localPosition.y - .02f, this.transform.localPosition.z);

        System.Random r = new System.Random();
        var rndNumber = new decimal(r.NextDouble());
        float rndNum = (float)rndNumber * Vector3.Distance(pos1,pos2);
        this.transform.localPosition = new Vector3(pos2.x, pos2.y + rndNum, pos2.z);
    }
    void LateUpdate() 
    {   
        if(this.transform.localPosition.y >= pos1.y) goingUp = !goingUp;
        else if (this.transform.localPosition.y <= pos2.y) goingUp = !goingUp;

        if(goingUp)
        {
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, pos1, speed * Time.deltaTime);
        }
        else
        {
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, pos2, speed * Time.deltaTime);
        }
        
    }
}