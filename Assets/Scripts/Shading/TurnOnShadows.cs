using UnityEngine;

public class TurnOnShadows : MonoBehaviour 

{
    private void Start()
    {
        this.gameObject.GetComponent<SpriteRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
}