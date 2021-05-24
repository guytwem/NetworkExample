using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Battlecars.Networking;

namespace Battlecars.UI
{
    public class LobbyPlayerSlot : MonoBehaviour
    {
        public bool IsTaken => player != null;
        public BattlecarsPlayerNet Player => player;
        public bool IsLeft { get; private set; } = false;

        [SerializeField] private TextMeshProUGUI nameDisplay;
        [SerializeField] private Button playerButton;
        [SerializeField] private ColorBlock stateColors = ColorBlock.defaultColorBlock;

        private BattlecarsPlayerNet player = null;

        // Set the player in this slot to the passed player
        public void AssignPlayer(BattlecarsPlayerNet _player) => player = _player;

        public void SetSide(bool _left) => IsLeft = _left;

        // Update is called once per frame
        void Update()
        {
            // If the slot is empty then set the button shouldn't be active
            playerButton.interactable = IsTaken;
            // If the player is set, then display their name, otherwise display "Awaiting player..."
            nameDisplay.text = IsTaken ? GetPlayerName() : "Awaiting Player...";
            playerButton.targetGraphic.color = player == null ? stateColors.disabledColor :
                    player.ready ? stateColors.pressedColor : stateColors.normalColor;
        }

        private string GetPlayerName() => string.IsNullOrEmpty(player.username) ? $"Player {player.playerId + 1}" : player.username;
    }
}