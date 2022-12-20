using UnityEngine;

public class TurnOnShadows : MonoBehaviour 

{
    private void Start()
    {
        this.gameObject.GetComponentInChildren<SpriteRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        Destroy(this);
    }
}