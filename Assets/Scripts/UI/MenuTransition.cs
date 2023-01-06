using UnityEngine;
using UnityEngine.UI;

public class MenuTransition : MonoBehaviour 
{
    [SerializeField] Button button;
    [SerializeField] GameObject menuToTransitionTo;

    void Awake() 
    {
        button = this.gameObject.GetComponent<Button>();
    }


    void Transition(GameObject menu)
    {
        menuToTransitionTo.SetActive(true);
        
    }
    void OnEnable() 
    {
        button.onClick.AddListener(() => Transition(menuToTransitionTo));
    }
    void OnDisable() 
    {
        button.onClick.RemoveAllListeners();
    }
}