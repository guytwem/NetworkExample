using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Battlecars.Networking;

namespace Battlecars.UI
{
    public class Lobby : MonoBehaviour
    {
        private List<LobbyPlayerSlot> leftTeamSlots = new List<LobbyPlayerSlot>();
        private List<LobbyPlayerSlot> rightTeamSlots = new List<LobbyPlayerSlot>();

        [SerializeField] private int requiredPlayerCount = 2;
        [SerializeField] private GameObject leftTeamHolder;
        [SerializeField] private GameObject rightTeamHolder;
        [SerializeField] private Button readyUpButton;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Camera uiCam;

        // Flipping bool that determines which column the connected player will be added to
        private bool assigningToLeft = true;

        private BattlecarsPlayerNet localPlayer;

        public void AssignPlayerToSlot(BattlecarsPlayerNet _player, bool _left, int _slotId)
        {
            // Get the correct slot list depending on the left param
            List<LobbyPlayerSlot> slots = _left ? leftTeamSlots : rightTeamSlots;
            // Assign the player to the relevant slot in this list
            slots[_slotId].AssignPlayer(_player);
        }

        public void OnPlayerConnected(BattlecarsPlayerNet _player)
        {
            bool assigned = false;

            // If the player is the localplayer, assign it
            if(_player.isLocalPlayer && localPlayer == null) 
            {
                localPlayer = _player;
                localPlayer.onMatchStarted.AddListener(OnMatchStarted);
            }

            List<LobbyPlayerSlot> slots = assigningToLeft ? leftTeamSlots : rightTeamSlots;

            // Loop through each item in the list and run a lambda with the item at that index
            slots.ForEach(slot =>
            {
                // If we have assigned the value already, return from the lambda
                if (assigned)
                {
                    return;
                }
                else if (!slot.IsTaken)
                {
                    // If we haven't already assigned the player to a slot and this slot
                    // hasn't been taken, assign the player to this slot and flag 
                    // as slot been assigned
                    slot.AssignPlayer(_player);
                    slot.SetSide(assigningToLeft);
                    assigned = true;
                }
            });

            for(int i = 0; i < leftTeamSlots.Count; i++)
            {
                LobbyPlayerSlot slot = leftTeamSlots[i];
                if(slot.IsTaken)
                    localPlayer.AssignPlayerToSlot(slot.IsLeft, i, slot.Player.playerId);
            }

            for (int i = 0; i < rightTeamSlots.Count; i++)
            {
                LobbyPlayerSlot slot = rightTeamSlots[i];
                if (slot.IsTaken)
                    localPlayer.AssignPlayerToSlot(slot.IsLeft, i, slot.Player.playerId);
            }

            // Flip the flag so that the next one will end up in the other list
            assigningToLeft = !assigningToLeft;
        }

        // Start is called before the first frame update
        private void Start()
        {
            // Fill the two lists with their slots
            leftTeamSlots.AddRange(leftTeamHolder.GetComponentsInChildren<LobbyPlayerSlot>());
            rightTeamSlots.AddRange(rightTeamHolder.GetComponentsInChildren<LobbyPlayerSlot>());

            readyUpButton.onClick.AddListener(() =>
            {
                BattlecarsPlayerNet player = BattlecarsNetworkManager.Instance.LocalPlayer; 
                player.SetReady(!player.ready);
            });

            startGameButton.onClick.AddListener(() => localPlayer.StartMatch());
        }

        public void OnMatchStarted()
        {
            uiCam.enabled = false;
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
            startGameButton.interactable = AllPlayersReady();
        }

        private bool AllPlayersReady()
        {
            int playerCount = 0;

            foreach(LobbyPlayerSlot slot in leftTeamSlots)
            {
                if(slot.Player == null)
                    continue;

                playerCount++;

                if(!slot.Player.ready)
                    return false;
            }

            foreach(LobbyPlayerSlot slot in rightTeamSlots)
            {
                if(slot.Player == null)
                    continue;

                playerCount++;

                if(!slot.Player.ready)
                    return false;
            }

            return playerCount >= requiredPlayerCount && BattlecarsNetworkManager.Instance.IsHost;
        }
    }
}