using UnityEngine;
using System;
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

        public delegate void MouseOverMana3D(Mana3D mana);
        public event MouseOverMana3D OnMouseEnterMana3D;
        public delegate void MouseExitMana3D();
        public event MouseExitMana3D OnMouseExitMana3D;

        public delegate void MouseEnterManaArea();
        public event MouseEnterManaArea OnMouseEnterManaArea;
        public delegate void MouseExitManaArea();
        public event MouseExitManaArea OnMouseExitManaArea;

        public delegate void MouseEnterPlayArea();
        public event MouseEnterPlayArea OnMouseEnterPlayArea;
        public delegate void MouseEnterCardArea();
        public event MouseEnterCardArea OnMouseEnterCardArea;

        public delegate void CardMouseOver(Card3D card);
        public event CardMouseOver onCardMouseOver;
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


        void TriggerMouseEnteredMana(Mana3D mana)
        {
            if (!IsInputEnabled()) return;
            //Condition checks if any methods are subscribed to this event
            if (OnMouseEnterMana3D != null)
            {
                OnMouseEnterMana3D(mana);
            }
        }
        void TriggerMouseExitMana3D()
        {
            if (!IsInputEnabled()) return;
            //Condition checks if any methods are subscribed to this event
            if (OnMouseExitMana3D != null)
            {
                OnMouseExitMana3D();
            }
        }
        void TriggerMouseEnterManaArea()
        {
            if (!IsInputEnabled()) return;
            //Condition checks if any methods are subscribed to this event
            if (OnMouseEnterManaArea != null)
            {
                OnMouseEnterManaArea();
            }
        }
        void TriggerMouseExitManaArea()
        {
            if (!IsInputEnabled()) return;
            //Condition checks if any methods are subscribed to this event
            if (OnMouseExitManaArea != null)
            {
                OnMouseExitManaArea();
            }
        }
        void TriggerMouseEnterPlayArea()
        {
            if (!IsInputEnabled()) return;
            //Condition checks if any methods are subscribed to this event
            if (OnMouseEnterPlayArea != null)
            {
                OnMouseEnterPlayArea();
            }
        }
        void TriggerMouseEnterCardArea()
        {
            if (!IsInputEnabled()) return;
            //Condition checks if any methods are subscribed to this event
            if (OnMouseEnterCardArea != null)
            {
                OnMouseEnterCardArea();
            }
        }
        void TriggerMouseOverCard(Card3D card)
        {
            if (!IsInputEnabled()) return;
            Debug.Log("Mouse over card");
            //Condition checks if any methods are subscribed to this event    
            if (onCardMouseOver != null)
            {
                onCardMouseOver(card);
            }
        }
        void TriggerMouseEnterCard(Card3D card)
        {
            if (!IsInputEnabled()) return;
            Debug.Log("Mouse Entered");
            //Condition checks if any methods are subscribed to this event
            if (OnCardMouseEnter != null)
            {
                OnCardMouseEnter(card);
            }
        }
        void TriggerMouseExitCard(Card3D card)
        {
            if (!IsInputEnabled()) return;
            Debug.Log("Mouse exited card");
            //Condition checks if any methods are subscribed to this event
            if (OnCardMouseExit != null)
            {
                OnCardMouseExit(card);
            }
        }
        void TriggerEnemyMouseOver(Enemy enemy)
        {
            if (!IsInputEnabled()) return;

            if (OnEnemyMouseOver != null)
            {
                OnEnemyMouseOver(enemy);
            }
        }
        void TriggerLeftClicked()
        {
            if (!IsInputEnabled()) return;
            //Condition checks if any methods are subscribed to this event
            if (OnLeftClicked != null)
            {
                OnLeftClicked();
            }
        }
        void TriggerLeftClickedUp()
        {
            if (!IsInputEnabled()) return;
            //Condition checks if any methods are subscribed to this event
            if (OnLeftClickedUp != null)
            {
                OnLeftClickedUp();
            }
        }
        void TriggerRightClicked()
        {
            if (!IsInputEnabled()) return;
            //Condition checks if any methods are subscribed to this event
            if (OnRightClicked != null)
            {
                OnRightClicked();
            }
        }

        bool mouseEnteredPlayArea = false;
        bool mouseEnteredMana3D = false;
        bool mouseEnteredCardHandArea = false;
        bool mouseEnteredManaArea = false;
        bool mouseEnteredCard = false;
        //Card3D currentCard = null;

        void MouseOver()
        {
            Ray ray = cardCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, 100.0F);

            //localCheck bools handle if the raycast passed through an object in this raycast
            //Always set to false at the start but if they pass through and object they are set to true
            //If they are false they reset the global bool with matching name to be false
            //In essence this allows events to called once when a mouse mouses over an object isntead of in every update loop
            bool mouseEnteredPlayAreaLocalCheck = false;
            bool mouseEnteredMana3DLocalCheck = false;
            bool mouseEnteredCardHandAreaLocalCheck = false;
            bool mouseEnteredManaAreaLocalCheck = false;
            //bool mouseEnteredCardLocalCheck = false;

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                /* 
                                if (hit.transform.gameObject.TryGetComponent<Card3D>(out Card3D card))
                                {
                                    if (!mouseEnteredCard || currentCard != card)
                                    {
                                        if (currentCard != card) TriggerMouseExitCard(currentCard);
                                        TriggerMouseEnterCard(card);
                                        mouseEnteredCard = true;
                                        currentCard = card;
                                    }
                                    TriggerMouseOverCard(card);
                                    mouseEnteredCardLocalCheck = true;
                                } */
                if (hit.transform.gameObject.TryGetComponent<Mana3D>(out Mana3D mana))
                {
                    if (!mouseEnteredMana3D)
                    {
                        TriggerMouseEnteredMana(mana);
                        mouseEnteredMana3D = true;
                    }
                    //Can trigger mouseOver events here for events that keep getting called
                    mouseEnteredMana3DLocalCheck = true;
                }
                else if (hit.transform.gameObject.name == "CardHandArea")
                {
                    if (!mouseEnteredCardHandArea)
                    {
                        TriggerMouseEnterCardArea();
                        mouseEnteredCardHandArea = true;
                    }
                    mouseEnteredCardHandAreaLocalCheck = true;
                }
                else if (hit.transform.gameObject.name == "CardPlayingArea")
                {
                    if (!mouseEnteredPlayArea)
                    {
                        TriggerMouseEnterPlayArea();
                        mouseEnteredPlayArea = true;
                    }
                    mouseEnteredPlayAreaLocalCheck = true;
                }
                else if (hit.transform.gameObject.name == "ManaArea")
                {
                    if (!mouseEnteredManaArea)
                    {
                        TriggerMouseEnterManaArea();
                        mouseEnteredManaArea = true;
                    }
                    mouseEnteredManaAreaLocalCheck = true;
                }
            }

/*             if (!mouseEnteredCardLocalCheck && mouseEnteredCard)
            {
                TriggerMouseExitCard(currentCard);
                mouseEnteredCard = false;
            } */
            if (!mouseEnteredMana3DLocalCheck && mouseEnteredMana3D)
            {
                TriggerMouseExitMana3D();
                mouseEnteredMana3D = false;
            }
            if (!mouseEnteredManaAreaLocalCheck && mouseEnteredManaArea)
            {
                TriggerMouseExitManaArea();
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
        bool IsInputEnabled()
        {
            if (isPaused || !isEnabled) return false;
            else return true;
        }
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
            if (staticInstance == null)
                staticInstance = this;
            else
                Destroy(this);

        }

    }
}
