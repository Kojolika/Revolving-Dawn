using UnityEngine;
using Zenject;

namespace Views
{
    public class PlayerView : MonoBehaviour
    {
        public readonly Models.Player.PlayerClassModel playerClassModel;
        public PlayerView(Models.Player.PlayerClassModel playerClassModel)
        {
            this.playerClassModel = playerClassModel;
        }

        public class Factory : PlaceholderFactory<Models.Player.PlayerClassModel, PlayerView>
        {

        }
    }
}