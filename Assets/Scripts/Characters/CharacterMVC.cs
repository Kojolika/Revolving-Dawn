using UnityEngine;

namespace Characters
{
    public interface CharacterMVC
    {
        public IModel Model { get; }
        public IView View { get; }
        public IController Controller { get; }

        public interface IModel
        {
            int ID { get; }
            //TData Data { get; }
        }

        public interface IView
        {
            Transform transform { get; }
        }

        public interface IController
        {
            void DoMove(System.Collections.Generic.List<CharacterMVC> targets);
        }

        public interface IHealth
        {
            float CurrentHealth { get; }
            float MaxHealth { get; }

            void Heal(float amount);
            void TakeDamage(float amount);
            void SetHealth(float amount);
        }
    }
}