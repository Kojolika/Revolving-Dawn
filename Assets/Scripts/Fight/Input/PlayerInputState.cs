using System;
using Models;
using Systems.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;
using Zenject;

namespace Fight
{
    // Finite State Machine for player inputs and events
    public abstract class PlayerInputState : ITickable
    {
        public PlayerInputState(InputActionAsset inputActionAsset, Camera camera)
        {

        }

        public Action<CardView> CardHovered;

        public abstract void Transition(PlayerInputState nextState);
        public abstract void Exit();
        public abstract void Tick();
    }
}