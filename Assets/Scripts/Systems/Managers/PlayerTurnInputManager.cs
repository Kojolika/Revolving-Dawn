using UnityEngine;
using Cards;
using Characters;
using Mana;
using System;
using FightInput;

namespace Systems.Managers
{
    public class PlayerTurnInputManager : MonoBehaviour, Systems.Managers.Base.IManager
    {
    public Camera cardCam;
    public static PlayerTurnInputManager StaticInstance;

    public PlayerInputState state;
    public bool isEnabled = false;
    public bool isPaused = false;

    public event Action<Mana3D> MouseEnterMana3D;
    public event Action MouseExitMana3D;
    public event Action MouseEnterManaArea;
    public event Action MouseExitManaArea;
    public event Action MouseEnterPlayArea;
    public event Action MouseEnterCardArea;
    public event Action LeftClicked;
    public event Action RightClicked;
    public event Action<Enemy> EnemyMouseOver;

    public delegate void CardMouseOverEvent(Card3D card);

    public event CardMouseOverEvent CardMouseOver;

    public delegate void CardMouseExitEvent(Card3D card);

    public event CardMouseExitEvent CardMouseExit;

    void TriggerMouseOverCard(Card3D card)
    {
        if (!IsInputEnabled()) return;

        CardMouseOver?.Invoke(card);
    }

    void TriggerMouseExitCard(Card3D card)
    {
        if (!IsInputEnabled()) return;

        CardMouseExit?.Invoke(card);
    }

    bool mouseEnteredPlayArea = false;
    bool mouseEnteredMana3D = false;
    bool mouseEnteredCardHandArea = false;
    bool mouseEnteredManaArea = false;

    void MouseOver()
    {
        Ray ray = cardCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, 100.0F);

        // localCheck bools handle if the raycast passed through an object in this raycast
        // Always set to false at the start but if they pass through and object they are set to true
        // If they are false they reset the global bool with matching name to be false
        // In essence this allows events to called once when a mouse mouses over an object instead of in every update loop
        bool mouseEnteredPlayAreaLocalCheck = false;
        bool mouseEnteredMana3DLocalCheck = false;
        bool mouseEnteredCardHandAreaLocalCheck = false;
        bool mouseEnteredManaAreaLocalCheck = false;

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.gameObject.TryGetComponent(out Mana3D mana))
            {
                if (!mouseEnteredMana3D)
                {
                    MouseEnterMana3D?.Invoke(mana);
                    mouseEnteredMana3D = true;
                }

                //Can trigger mouseOver events here for events that keep getting called
                mouseEnteredMana3DLocalCheck = true;
            }
            else if (hit.transform.gameObject.name == "CardHandArea")
            {
                if (!mouseEnteredCardHandArea)
                {
                    MouseEnterCardArea?.Invoke();
                    mouseEnteredCardHandArea = true;
                }

                mouseEnteredCardHandAreaLocalCheck = true;
            }
            else if (hit.transform.gameObject.name == "CardPlayingArea")
            {
                if (!mouseEnteredPlayArea)
                {
                    MouseEnterPlayArea?.Invoke();
                    mouseEnteredPlayArea = true;
                }

                mouseEnteredPlayAreaLocalCheck = true;
            }
            else if (hit.transform.gameObject.name == "ManaArea")
            {
                if (!mouseEnteredManaArea)
                {
                    MouseEnterManaArea?.Invoke();
                    mouseEnteredManaArea = true;
                }

                mouseEnteredManaAreaLocalCheck = true;
            }
        }

        if (!mouseEnteredMana3DLocalCheck && mouseEnteredMana3D)
        {
            MouseExitMana3D?.Invoke();
            mouseEnteredMana3D = false;
        }

        if (!mouseEnteredManaAreaLocalCheck && mouseEnteredManaArea)
        {
            MouseExitManaArea?.Invoke();
            mouseEnteredManaArea = false;
        }

        if (!mouseEnteredPlayAreaLocalCheck && mouseEnteredPlayArea)
        {
            mouseEnteredPlayArea = false;
        }

        if (!mouseEnteredCardHandAreaLocalCheck && mouseEnteredCardHandArea)
        {
            mouseEnteredCardHandArea = false;
        }
    }

    void MouseOverCharacter()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, 100.0F);
        Enemy currentEnemy = null;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.gameObject.GetComponent<Enemy>())
            {
                currentEnemy = hit.transform.gameObject.GetComponent<Enemy>();
                EnemyMouseOver?.Invoke(currentEnemy);
                return;
            }
        }

        EnemyMouseOver?.Invoke(currentEnemy);
    }

    void MouseInput()
    {

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            RightClicked?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            LeftClicked?.Invoke();
        }
    }

    public void Enable(bool value) => isEnabled = value;

    bool IsInputEnabled() => !isPaused && isEnabled;

    public void RegisterCardEvents(Card3D card)
    {
        card.OnMouseOverEvent += TriggerMouseOverCard;
        card.OnMouseExitEvent += TriggerMouseExitCard;
    }

    public void UnregisterCardEvents(Card3D card)
    {
        card.OnMouseOverEvent -= TriggerMouseOverCard;
        card.OnMouseExitEvent -= TriggerMouseExitCard;
    }

    void Update()
    {
        if (!IsInputEnabled()) return;

        MouseOver();
        MouseOverCharacter();
        MouseInput();
    }

    void Awake()
    {
        if (StaticInstance == null)
            StaticInstance = this;
        else
            Destroy(this);

    }

    }
}
