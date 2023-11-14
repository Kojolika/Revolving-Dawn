using Characters.Model;

namespace Characters
{
    public class PlayerMVC : CharacterMVC
    {
        private PlayerDefinition definition;

        public CharacterMVC.IModel Model => definition;
        public CharacterMVC.IView View { get; }
        public CharacterMVC.IController Controller { get; }
    }
}