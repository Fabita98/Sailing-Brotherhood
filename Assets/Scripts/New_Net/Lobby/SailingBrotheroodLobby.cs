using QFSW.QC;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;

public class SailingBrotheroodLobby : NetworkBehaviour
{
    public static SailingBrotheroodLobby Instance { get; private set; }

    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_GAME_MODE = "GameMode";
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";


    private float heartbeatTimer;
    private float refreshLobbyListTimer = 5f;
    private string playerName;
    public const int maxPlayers = 4;
    private NetworkList<PlayerData> playerDataNetworkList;

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;

   public class OnLobbyListChangedEventArgs : EventArgs
   {
        public List<Lobby> lobbyList;
   }
    
    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float listLobbiesTimer;
    //Lobby fino qua

    //SailingBrotherhoodReady
    public event EventHandler OnReadyChanged;

    private Dictionary<ulong, bool> playerReadyDictionary;
    //end SailingBrotherhoodReady
    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;    

    public event EventHandler OnPlayerDataNetworkListChanged;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);  // Do not destroy SailingBrLobby when changing scene

        Authenticate(playerName);
        //await UnityServices.InitializeAsync();
        //AuthenticationService.Instance.SignedIn += () =>
        //{
        //    Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId + " as: " + playerName);
        //    //RefreshLobbyList();
        //};
        //await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerReadyDictionary = new Dictionary<ulong, bool>(); //Ready

        //Multiplayer
        playerName = "Jack_Sparrow" + UnityEngine.Random.Range(1, 100);
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }
    //SailingBrotherhoodLobby
    private void Update()
    {
        HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds but to use when running on more devices, each one with different ip
        HandleLobbyHeartbeat();
        //HandleLobbyPollForUpdate();
    }
    public async void Authenticate(string playerName)
    {
        this.playerName = playerName;

        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(playerName);
            await UnityServices.InitializeAsync(initializationOptions);

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId + "as: " + playerName);

            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            //RefreshLobbyList();
        }
    }

    private async void HandleLobbyHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private void HandleLobbyPollForUpdate()
    {
        if (joinedLobby == null &&
            UnityServices.State == ServicesInitializationState.Initialized &&
            AuthenticationService.Instance.IsSignedIn &&
            SceneManager.GetActiveScene().name == Loader.Scene.Lobby.ToString())
        {

            listLobbiesTimer -= Time.deltaTime;
            if (listLobbiesTimer <= 0f)
            {
                float listLobbiesTimerMax = 3f;
                listLobbiesTimer = listLobbiesTimerMax;
                ListLobbies();
            }
        }
    }

    [Command]
    //public async void CreateLobby()
    //{
    //    OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
    //    try
    //    {
    //        CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions()
    //        {
    //            Player = GetPlayer(),
    //            // if true -> not visible with ListLobbies
    //            IsPrivate = false,
    //            Data = new Dictionary<string, DataObject>
    //            {
    //                { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, "CasualRace")/*, DataObject.IndexOptions.S1*/}
    //            }
    //        };

    //        Allocation allocation = await AllocateRelay();


    //        // this for customized lobby creation options
    //        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("First lobby", maxPlayers, createLobbyOptions);

    //        hostLobby = lobby;
    //        joinedLobby = hostLobby;
    //        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    //        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
    //        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
    //        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
    //        NetworkManager.Singleton.StartHost();

    //        Debug.Log("You are the Host of the Lobby: " + lobby.Name + " Max Players: " + lobby.MaxPlayers + " Lobby ID: " + lobby.Id + " Lobby Code: " + lobby.LobbyCode);
    //        PrintPlayers(joinedLobby);            
    //    }
    //    catch (LobbyServiceException e) { 
    //        Debug.Log(e);
    //        OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
    //    }
    //}
    [Command]
    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, SailingBrotheroodLobby.maxPlayers, new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            });

            Allocation allocation = await AllocateRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                     { KEY_RELAY_JOIN_CODE , new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode) }
                 }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            SailingBrotheroodLobby.Instance.StartHost();
            //Loader.LoadNetwork(Loader.Scene.Lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    [Command]
    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    [Command]
    //public async void QuickJoinLobby()
    //{
    //    OnJoinStarted?.Invoke(this, EventArgs.Empty);
    //    try
    //    {
    //        //QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

    //        Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
    //        joinedLobby = lobby;
    //        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    //        Debug.Log(" You have just joined the lobby " + joinedLobby.Name + " as a client");

    //        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
    //        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
    //        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
    //        NetworkManager.Singleton.StartClient();

    //        //PrintPlayers(joinedLobby); not working because polling is every 1s
    //    }
    //    catch (LobbyServiceException e)
    //    {
    //        Debug.Log(e);
    //        OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
    //    }
    //}
    [Command]
    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            SailingBrotheroodLobby.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    [Command]
    public bool IsLobbyHost()
    {
        return hostLobby != null && hostLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    [Command]
    private async void ListLobbies()
    {
        try
        {            
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs {
                lobbyList = queryResponse.Results 
            });
            Debug.Log("Lobbies found: " + queryResponse.Results.Count + " ");
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log("Number of players in this lobby: " + lobby.Players.Count + " Max players: " + lobby.MaxPlayers );
                //Debug.Log("Lobby name: " + lobby.Name + " " + "Max players: " + lobby.MaxPlayers /*+ " " + lobby.Data["GameMode"].Value*/);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]
    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(SailingBrotheroodLobby.maxPlayers - 1);

            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);

            return default;
        }
    }
    [Command]
    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }
    [Command]
    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }
    [Command]
    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }
    [Command]
    public async void JoinWithCode(string lobbyCode)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            SailingBrotheroodLobby.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    [Command]
    public async void RefreshLobbyList()
    {
        try
        {
            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]
    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    [Command]
    private bool ArePlayersInLobby()
    {
        if (joinedLobby != null && joinedLobby.Players != null)
        {
            try
            {
                foreach (Player player in joinedLobby.Players)
                {
                    if (player.Id == AuthenticationService.Instance.PlayerId)
                    {
                        // This player is in this lobby
                        return true;
                    }
                }
            }
            catch (LobbyServiceException e) { Debug.Log(e); }
        }
        return false;

    }

    [Command]
    //public async void DeleteLobby()
    //{
    //    if (/*ArePlayersInLobby() &&*/ IsLobbyHost())
    //        try
    //        {
    //            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
    //        }
    //        catch (LobbyServiceException e) { Debug.Log(e); }
    //}
    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            //Id = AuthenticationService.Instance.PlayerId,
            Data = new Dictionary<string, PlayerDataObject>
                    {   //member -> visible to each lobby member: private -> only host, public -> public
                        { "PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName ) }
                    }
        };
    }
    public string GetPlayerName()
    {
        return playerName;
    }

    [Command]
    private void PrintPlayers()
    {
        PrintPlayers(joinedLobby);
    }

    private void PrintPlayers(Lobby lobby)
    {
        // In case of many lobbies
        /*"Players in lobby named: " + lobby.Name + " " + lobby.Data["GameMode"].Value*/
        Debug.Log("Players in the lobby named: " + lobby.Name);
        foreach (Player player in lobby.Players)
        {
            Debug.Log(/*"Player ID: " + player.Id + */ "Player Name: " + player.Data["PlayerName"].Value);
        }
    }
    //SailingBrotherhoodLobby end

    //Ready
    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                // This player is NOT ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            Instance.DeleteLobby();
            Loader.LoadNetwork(Loader.Scene.Pirate_Test_NETWORK);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    [Command]
    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
    //Ready end


    //Method taken from SailingBrotherhoodMultiplayer but corrected: Max 4 pLayers can join only when in lobby scene 
    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.Lobby.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            Debug.Log("The game has already started");
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= maxPlayers)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            Debug.Log("The game lobby is full");
            return;
        }
        connectionApprovalResponse.Approved = true;

    }
    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
    }
    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    //Destroy the objects related to the disconnected player
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
        });
    }
    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.clientId == clientId)
            {
                // Disconnected!
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerName = playerName;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerId = playerId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    //Not 4 local test purpose, yes for many computers
    private void HandleRefreshLobbyList()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
        {
            refreshLobbyListTimer -= Time.deltaTime;
            if (refreshLobbyListTimer < 0f)
            {
                float refreshLobbyListTimerMax = 5f;
                refreshLobbyListTimer = refreshLobbyListTimerMax;

                RefreshLobbyList();
            }
        }
    }

    //Not 4 prototype
    private async void MigrateLobbyHost()
    {
        // Customized host selection after host has left
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                // Grabs the 2nd player.Id
                HostId = joinedLobby.Players[1].Id,
            });
            joinedLobby = hostLobby;
            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }
    private async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions() { Player = GetPlayer() };
            //QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            Debug.Log("Joined lobby with code: " + lobbyCode);
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    private async void UpdateLobbyGameMode(string gameMode)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) }
                }
            });
        }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }
    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                }
            });
        }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }
    private async void KickPlayer()
    {
        try
        {   // kicks the 2nd player: the first player who is gonna to be the host
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
        }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }
}