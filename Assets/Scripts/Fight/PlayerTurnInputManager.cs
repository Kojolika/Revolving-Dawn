using UnityEngine;
using cards;
using characters;

namespace fight
{
    public class PlayerTurnInputManager : MonoBehaviour
    {
        public Camera cardCam;

        public PlayerInputState state;
        bool isEnabled;

        public delegate void MouseEnterPlayArea();
        public event MouseEnterPlayArea TriggerMouseEnterPlayArea;

        public delegate void MouseEnterCardArea();
        public event MouseEnterCardArea TriggerMouseEnterCardArea;

        public delegate void CardMouseOver(Card card);
        public event CardMouseOver TriggerCardMouseOver;
        public delegate void NoCardMouseOver();
        public event NoCardMouseOver TriggerNoCardMouseOver;

        public delegate void LeftClicked();
        public event LeftClicked TriggerLeftClicked;

        public delegate void RightClicked();
        public event RightClicked TriggerRightClicked;

        public delegate void EnemyMouseOver(Enemy enemy);
        public event EnemyMouseOver TriggerEnemyMouseOver;

        void OnMouseEnterPlayArea()
        {
            //Debug.Log("Mouse entered play area");
            //Condition checks if any methods are subscribed to this event
            if (TriggerMouseEnterPlayArea != null)
            {
                TriggerMouseEnterPlayArea();
            }
        }
        void OnMouseEnterCardArea()
        {
            //Debug.Log("Mouse entered card area");
            //Condition checks if any methods are subscribed to this event
            if (TriggerMouseEnterCardArea != null)
            {
                TriggerMouseEnterCardArea();
            }
        }
        void IsCardMouseOver(Card card)
        {
            //Debug.Log("Mouse over card: " + card);
            //Condition checks if any methods are subscribed to this event
            if (TriggerCardMouseOver != null)
            {
                TriggerCardMouseOver(card);
            }
        }
        void IsEnemyMouseOver(Enemy enemy)
        {
            if(TriggerEnemyMouseOver != null)
            {
                TriggerEnemyMouseOver(enemy);
            }
        }

        void OnNoCardMouseOver()
        {
            //Debug.Log("No mouse over card");
            //Condition checks if any methods are subscribed to this event
            if (TriggerNoCardMouseOver != null)
            {
                TriggerNoCardMouseOver();
            }
        }    
        void IsLeftClicked()
        {
            //Debug.Log("Left Clicked");
            //Condition checks if any methods are subscribed to this event
            if (TriggerLeftClicked != null)
            {
                TriggerLeftClicked();
            }
        }
        void IsRightClicked()
        {
            //Debug.Log("Right Clicked");
            //Condition checks if any methods are subscribed to this event
            if (TriggerRightClicked != null)
            {
                TriggerRightClicked();
            }
        }

        void MouseOver()
        {
            Ray ray = cardCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, 100.0F);
            Card currentCard = null;
            Enemy currentEnemy = null;
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                if (hit.transform.gameObject.GetComponent(typeof(Card)) && currentCard == null)
                {
                    currentCard = hit.transform.gameObject.GetComponent(typeof(Card)) as Card;
                    IsCardMouseOver(currentCard);
                }
                if (hit.transform.gameObject.name == "CardHandArea")
                {
                    OnMouseEnterCardArea();
                }
                else if (hit.transform.gameObject.name == "CardPlayingArea")
                {
                    OnMouseEnterPlayArea();
                }
                else if (hit.transform.gameObject.GetComponent<Enemy>())
                {
                    currentEnemy = hit.transform.gameObject.GetComponent<Enemy>();
                    IsEnemyMouseOver(currentEnemy);
                }
                else
                {
                    IsEnemyMouseOver(null);
                }
            }

            if(currentCard == null)
            {
                OnNoCardMouseOver();
            }


        }
        void MouseInput(){

            if(Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse1))
            {
                IsRightClicked();
            }
            if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse0))
            {
                IsLeftClicked();
            }
        }
        public void Enable(bool value){
            isEnabled = value;
        }
        void Update()
        {
            if(!isEnabled) return;

            MouseOver();
            MouseInput();
        }
    }
}
