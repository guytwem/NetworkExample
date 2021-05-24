using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections;
using Battlecars.UI;
using Battlecars.Player;
using System;

namespace Battlecars.Networking
{
    public class BattlecarsPlayerNet : NetworkBehaviour
    {
        [SyncVar] public byte playerId;
        [SyncVar] public string username = "";
        [SyncVar] public bool ready = false;

        [SerializeField] private new CameraMotor camera;
        [SerializeField] private PlayerMotor playerMotor;
        [SerializeField] private GameObject[] matchObjects;

        public UnityEvent onMatchStarted = new UnityEvent();

        private Lobby lobby;
        private bool hasJoinedLobby = false;

        public void StartMatch()
        {
            if(isLocalPlayer)
                CmdStartMatch();
        }

        public void SetUsername(string _name)
        {
            if(isLocalPlayer)
            {
                // Only localplayers can call Commands as localplayers are the only
                // ones who have the authority to talk to the server
                CmdSetUsername(_name);
            }
        }

        public void SetReady(bool _ready)
        {
            if(isLocalPlayer)
            {
                // Only localplayers can call Commands as localplayers are the only
                // ones who have the authority to talk to the server
                CmdSetReady(_ready);
            }
        }

        public void AssignPlayerToSlot(bool _left, int _slotId, byte _playerId)
        {
            if(isLocalPlayer)
                CmdAssignPlayerToLobbySlot(_left, _slotId, _playerId);
        }

        #region Commands

        [Command]
        public void CmdSetUsername(string _name) => username = _name;

        [Command]
        public void CmdSetReady(bool _ready) => ready = _ready;

        [Command]
        public void CmdAssignPlayerToLobbySlot(bool _left, int _slotId, byte _playerId) =>
            RpcAssignPlayerToLobbySlot(_left, _slotId, _playerId);

        [Command]
        public void CmdStartMatch() => RpcStartMatch();

        #endregion

        #region RPCs

        [ClientRpc]
        public void RpcAssignPlayerToLobbySlot(bool _left, int _slotId, byte _playerId)
        {
            // If this is running on the host client, we don't need to set the player
            // to the slot, so just ignore this call
            if(BattlecarsNetworkManager.Instance.IsHost)
                return;

            // Find the Lobby in the scene and set the player to the correct slot
            StartCoroutine(AssignPlayerToLobbySlotDelayed(BattlecarsNetworkManager.Instance.GetPlayerForId(_playerId),
                _left, _slotId));
        }

        [ClientRpc]
        public void RpcStartMatch()
        {
            foreach(BattlecarsPlayerNet p in BattlecarsNetworkManager.Instance.Players)
            {
                foreach (GameObject matchObject in matchObjects)
                    matchObject.SetActive(true);
            }

            
            
            LevelManager.LoadLevel("Gameplay");
            
            BattlecarsPlayerNet player = BattlecarsNetworkManager.Instance.LocalPlayer;
            FindObjectOfType<Lobby>().OnMatchStarted();
            player.playerMotor.Enable();
            player.camera.Enable();
        }

        #endregion

        #region Coroutines

        private IEnumerator AssignPlayerToLobbySlotDelayed(BattlecarsPlayerNet _player, bool _left, int _slotId)
        {
            // Keep trying to get the lobby until it's not null
            Lobby lobby = FindObjectOfType<Lobby>();
            while(lobby == null)
            {
                yield return null;

                lobby = FindObjectOfType<Lobby>();
            }

            // Lobby successfully got, so assign the player
            lobby.AssignPlayerToSlot(_player, _left, _slotId);
        }

        #endregion

        private void Awake()
        {
            foreach(GameObject matchObject in matchObjects)
                matchObject.SetActive(false);
        }

        // Start is called before the first frame update
        private void Start()
        {
            SetUsername(BattlecarsNetworkManager.Instance.PlayerName);
        }

        // Update is called once per frame
        private void Update()
        {
            // Determine if we are on the host client
            if(BattlecarsNetworkManager.Instance.IsHost)
            {
                // Attempt to get the lobby if we haven't already joined a lobby
                if(lobby == null && !hasJoinedLobby)
                    lobby = FindObjectOfType<Lobby>();

                // Attempt to join the lobby if we haven't already and the lobby is set
                if(lobby != null && !hasJoinedLobby)
                {
                    hasJoinedLobby = true;
                    lobby.OnPlayerConnected(this);
                }
            }
        }

        public override void OnStartClient()
        {
            BattlecarsNetworkManager.Instance.AddPlayer(this);
        }

        // Runs only when the object is connected is the local player
        public override void OnStartLocalPlayer()
        {
            // Load the scene with the lobby
            LevelManager.LoadLevel("InGameMenus");
        }

        // Runs when the client is disconnected from the server
        public override void OnStopClient()
        {
            // Remove the playerID from the server
            BattlecarsNetworkManager.Instance.RemovePlayer(playerId);
        }
    }
}