using UnityEngine;
using cards;
using characters;
using mana;

namespace fightInput
{
    public class PlayerTurnInputManager : MonoBehaviour
    {
        public Camera cardCam;
        public static PlayerTurnInputManager staticInstance;

        public PlayerInputState state;
        public bool isEnabled = false;
        public bool isPaused = false;

        public delegate void ManaMouseOver(Mana3D mana);
        public event ManaMouseOver OnManaMouseOver;
        public delegate void NoManaMouseOver();
        public event NoManaMouseOver OnNoManaMouseOver;

        public delegate void MouseEnterManaArea();
        public event MouseEnterManaArea OnMouseEnterManaArea;
        public delegate void MouseExitManaArea();
        public event MouseExitManaArea OnMouseExitManaArea;

        public delegate void MouseEnterPlayArea();
        public event MouseEnterPlayArea OnMouseEnterPlayArea;
        public delegate void MouseEnterCardArea();
        public event MouseEnterCardArea OnMouseEnterCardArea;

        public delegate void CardMouseEnter(Card3D card);
        public event CardMouseEnter OnCardMouseEnter;
        public delegate void CardMouseExit(Card3D card);
        public event CardMouseExit OnCardMouseExit;

        public delegate void LeftClicked();
        public event LeftClicked OnLeftClicked;
        public delegate void LeftClickedUp();
        public event LeftClickedUp OnLeftClickedUp;
        public delegate void RightClicked();
        public event RightClicked OnRightClicked;

        public delegate void EnemyMouseOver(Enemy enemy);
        public event EnemyMouseOver OnEnemyMouseOver;


        void TriggerManaMouseOver(Mana3D mana)
        {
            //Condition checks if any methods are subscribed to this event
            if (OnManaMouseOver != null)
            {
                OnManaMouseOver(mana);
            }
        }
        void TriggerNoManaMouseOver()
        {
            //Condition checks if any methods are subscribed to this event
            if (OnNoManaMouseOver != null)
            {
                OnNoManaMouseOver();
            }
        }
        void TriggerMouseEnterManaArea()
        {
            //Debug.Log("enter mana");
            //Condition checks if any methods are subscribed to this event
            if (OnMouseEnterManaArea != null)
            {
                OnMouseEnterManaArea();
            }
        }
        void TriggerMouseExitManaArea()
        {
            //Condition checks if any methods are subscribed to this event
            if (OnMouseExitManaArea != null)
            {
                OnMouseExitManaArea();
            }
        }
        void TriggerMouseEnterPlayArea()
        {
            //Condition checks if any methods are subscribed to this event
            if (OnMouseEnterPlayArea != null)
            {
                OnMouseEnterPlayArea();
            }
        }
        void TriggerMouseEnterCardArea()
        {
            //Condition checks if any methods are subscribed to this event
            if (OnMouseEnterCardArea != null)
            {
                OnMouseEnterCardArea();
            }
        }
        void TriggerMouseOverCard(Card3D card)
        {
            if (isPaused) return;
            if (!isEnabled) return;
            //Condition checks if any methods are subscribed to this event
            if (OnCardMouseEnter != null)
            {
                OnCardMouseEnter(card);
            }
        }
        void TriggerEnemyMouseOver(Enemy enemy)
        {

            if (OnEnemyMouseOver != null)
            {
                OnEnemyMouseOver(enemy);
            }
        }

        void TriggerOnCardMouseExit(Card3D card)
        {
            if (isPaused) return;
            if (!isEnabled) return;
            //Condition checks if any methods are subscribed to this event
            if (OnCardMouseExit != null)
            {
                OnCardMouseExit(card);
            }
        }
        void TriggerLeftClicked()
        {
            //Condition checks if any methods are subscribed to this event
            if (OnLeftClicked != null)
            {
                OnLeftClicked();
            }
        }
        void TriggerLeftClickedUp()
        {
            //Condition checks if any methods are subscribed to this event
            if (OnLeftClickedUp != null)
            {
                OnLeftClickedUp();
            }
        }
        void TriggerRightClicked()
        {
            //Condition checks if any methods are subscribed to this event
            if (OnRightClicked != null)
            {
                OnRightClicked();
            }
        }

        void MouseOver()
        {
            Mana3D currentMana = null;
            GameObject manaArea = null;

            Ray ray = cardCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, 100.0F);

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                if (hit.transform.gameObject.GetComponent<Mana3D>())
                {
                    currentMana = hit.transform.gameObject.GetComponent<Mana3D>();
                    TriggerManaMouseOver(currentMana);
                }
                if (hit.transform.gameObject.name == "CardHandArea")
                {
                    TriggerMouseEnterCardArea();
                }
                else if (hit.transform.gameObject.name == "CardPlayingArea")
                {
                    TriggerMouseEnterPlayArea();
                }
                if (hit.transform.gameObject.name == "ManaArea")
                {
                    TriggerMouseEnterManaArea();
                    manaArea = hit.transform.gameObject;
                }
            }

            if (!currentMana)
            {
                TriggerNoManaMouseOver();
            }
            if (!manaArea)
            {
                TriggerMouseExitManaArea();
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
                    TriggerEnemyMouseOver(currentEnemy);
                    return;
                }
            }
            TriggerEnemyMouseOver(currentEnemy);
        }
        void MouseInput()
        {

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                TriggerRightClicked();
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                TriggerLeftClicked();
            }
        }
        public void Enable(bool value)
        {
            isEnabled = value;
        }
        void Update()
        {
            if (isPaused) return;
            if (!isEnabled) return;

            MouseOver();
            MouseOverCharacter();
            MouseInput();
        }

        void Awake()
        {
            if (staticInstance == null)
                staticInstance = this;
            else
                Destroy(this);

            foreach (Card3D card in PlayerCardDecks.InstantiatedDeck)
            {
                card.OnMouseOverEvent += TriggerMouseOverCard;
                card.OnMouseExitEvent += TriggerOnCardMouseExit;
            }
        }
    }
}
