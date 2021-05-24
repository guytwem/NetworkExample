using System.Net;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Battlecars.Networking;

using Mirror;

using TMPro;

namespace Battlecars.UI
{
    public class ConnectionMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField hostNameText;
        [SerializeField] private TMP_InputField gameNameText;
        [SerializeField] private TMP_InputField playerNameText;
        [SerializeField] private Button hostStartGame;

        [SerializeField] private DiscoveredGame gameTemplate;
        [SerializeField] private Transform foundGamesHolder;

        [SerializeField] private BattlecarsNetworkManager networkManager;

        private Dictionary<IPAddress, DiscoveredGame> discoveredGames = new Dictionary<IPAddress, DiscoveredGame>();

        // Start is called before the first frame update
        void Start()
        {
            hostNameText.onSubmit.AddListener(_value => 
            {
                networkManager.PlayerName = _value;
                gameNameText.interactable = true;
                gameNameText.Select();
            });

            playerNameText.onEndEdit.AddListener(_value => networkManager.PlayerName = _value);

            hostStartGame.onClick.AddListener(() =>
            {
                if(!string.IsNullOrEmpty(gameNameText.text) && !string.IsNullOrEmpty(hostNameText.text))
                {
                    networkManager.GameName = gameNameText.text;
                    networkManager.StartHost();
                }
            });

            networkManager.discovery.onServerFound.AddListener(OnDetectServer);
            networkManager.discovery.StartDiscovery();
        }

        public bool IsJoinPlayerNamed() => !string.IsNullOrEmpty(playerNameText.text);

        private void OnDetectServer(DiscoveryResponse _response)
        {
            // Here we have recieved a server that is broadcasting on the network
            if(!discoveredGames.ContainsKey(_response.EndPoint.Address))
            {
                // We haven't already found a game with this IP, so make it
                DiscoveredGame game = Instantiate(gameTemplate, foundGamesHolder);
                game.gameObject.SetActive(true);

                // Setup the game using the response and add it to the list
                game.Setup(_response, networkManager);
                discoveredGames.Add(_response.EndPoint.Address, game);
            }
            else
            {
                DiscoveredGame game = discoveredGames[_response.EndPoint.Address];
                if(game.GameName != _response.gameName)
                {
                    game.UpdateResponse(_response);
                }
            }
        }
    }
}