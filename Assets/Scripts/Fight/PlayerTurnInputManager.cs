using UnityEngine;
using cards;

namespace fight
{
    public class PlayerTurnInputManager
    {
        PlayerState state;

        public delegate void MouseEnterPlayArea();
        public event MouseEnterPlayArea TriggerMouseEnterPlayArea;

        public delegate void MouseEnterCardArea();
        public event MouseEnterCardArea TriggerMouseEnterCardArea;

        public delegate void CardMouseOver(Card card);
        public event CardMouseOver TriggerCardMouseOver;

        public delegate void LeftClicked();
        public event LeftClicked TriggerLeftClicked;

        public delegate void RightClicked();
        public event RightClicked TriggerRightClicked;

        void OnMouseEnterPlayArea(){
            //Condition checks if any methods are subscribed to this event
            if(TriggerMouseEnterPlayArea != null){
                TriggerMouseEnterPlayArea();
            }
        }
        void OnMouseEnterCardArea(){
            //Condition checks if any methods are subscribed to this event
            if(TriggerMouseEnterCardArea != null){
                TriggerMouseEnterCardArea();
            }
        }
        void IsCardMouseOver(Card card){
            //Condition checks if any methods are subscribed to this event
            if(TriggerCardMouseOver != null){
                TriggerCardMouseOver(card);
            }
        }
        void IsLeftClicked(){
            //Condition checks if any methods are subscribed to this event
            if(TriggerLeftClicked!= null){
                TriggerLeftClicked();
            }
        }
        void IsRightClicked(){
            //Condition checks if any methods are subscribed to this event
            if(TriggerRightClicked!= null){
                TriggerRightClicked();
            }
        }
        void Start() {
            state = new DefaultState();
        }

        void Update() {
            
        }
    }
}
