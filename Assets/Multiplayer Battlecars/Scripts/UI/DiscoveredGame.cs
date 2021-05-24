using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Battlecars.Networking;

namespace Battlecars.UI
{
    [RequireComponent(typeof(Button))]
    public class DiscoveredGame : MonoBehaviour
    {
        public string GameName => response.gameName;

        [SerializeField] private TextMeshProUGUI gameName;
        [SerializeField] private TextMeshProUGUI hostName;
        [SerializeField] private TextMeshProUGUI playerCount;
        [SerializeField] private ConnectionMenu connectionMenu;
        
        private BattlecarsNetworkManager networkManager;
        private DiscoveryResponse response;
        
        public void Setup(DiscoveryResponse _response, BattlecarsNetworkManager _manager)
        {
            UpdateResponse(_response);
            networkManager = _manager;

            Button button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(JoinGame);
        }

        public void UpdateResponse(DiscoveryResponse _response)
        {
            response = _response;
            gameName.text = $"{response.gameName}";
            hostName.text = $"{response.hostName}";
            playerCount.text = $"{response.playerCount} / {response.maxPlayers}";
        }

        private void JoinGame()
        {
            // When we click the button, connect to the server displayed on the button
            networkManager.networkAddress = response.EndPoint.Address.ToString();
            if(connectionMenu.IsJoinPlayerNamed())
            {
                networkManager.StartClient();
            }
        }
    }
}