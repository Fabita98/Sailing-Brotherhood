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
//This script works like the KitchenChaosMultiplayer but we have only lobby and then Race_scene
public class LobbyTest : NetworkBehaviour
{
    public static LobbyTest Instance { get; private set; }

    public const string KEY_PLAYER_NAME = "PlayerName";
    //public const string KEY_PLAYER_CHARACTER = "Character";
    public const string KEY_GAME_MODE = "GameMode";

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private float refreshLobbyListTimer = 5f;
    private string playerName;
    public const int maxPlayers = 4;
    private NetworkList<PlayerData> playerDataNetworkList;


    public event EventHandler OnJoinStarted;    
    public event EventHandler OnTryingToJoinGame;

    public event EventHandler OnFailedToJoinGame;
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler OnLeftLobby;

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    
    public event EventHandler OnReadyChanged;

    public event EventHandler OnPlayerDataNetworkListChanged;
    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);  // Do not destroy LobbyTest when changing scene


        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }
    private void Update()
    {
        //HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds but to use when running on more devices, each one with different ip
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdate();
    }
    private void Start()
    {
        playerName = "Jack_Sparrow" + UnityEngine.Random.Range(1, 100);
        Authenticate(playerName);
        //await UnityServices.InitializeAsync();
        //AuthenticationService.Instance.SignedIn += () =>
        //{
        //    Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId + " as: " + playerName);
        //    //RefreshLobbyList();
        //};
        //await AuthenticationService.Instance.SignInAnonymouslyAsync();
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
            RefreshLobbyList();
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

    private async void HandleLobbyPollForUpdate()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0)
            {
                float lobbyUpdateTimerMax = 1.1f; // to update once per second
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                if (!IsPlayerInLobby())
                {
                    // Player has left this lobby
                    Debug.Log("You left the Lobby!");
                    OnLeftLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                    joinedLobby = null;
                }
            }
        }
    }

    [Command]
    public async void CreateLobby()
    {
        try
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions()
            {
                Player = GetPlayer(),
                // if true -> not visible with ListLobbies
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, "CasualRace")/*, DataObject.IndexOptions.S1*/}
                }
            };

            // this for customized lobby creation options
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("First lobby", maxPlayers, createLobbyOptions);

            hostLobby = lobby;
            joinedLobby = hostLobby;
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartHost();

            Debug.Log("You are the Host of the Lobby: " + lobby.Name + " Max Players: " + lobby.MaxPlayers + " Lobby ID: " + lobby.Id + " Lobby Code: " + lobby.LobbyCode);
            PrintPlayers(joinedLobby);            
        }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }

    [Command]
    public async void QuickJoinLobby()
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            joinedLobby = lobby;
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
            Debug.Log(" You have just joined the lobby " + joinedLobby.Name + " as a client");

            OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
            NetworkManager.Singleton.StartClient();

            //PrintPlayers(joinedLobby); not working because polling is every 1s
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
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
            //QueryLobbiesOptions queryLobbiesOptions = new()
            //{
            //    Count = 25,
            //    Filters = new List<QueryFilter>
            //    {
            //        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
            //        //new QueryFilter(QueryFilter.FieldOptions.S1, "CasualRace",QueryFilter.OpOptions.EQ)

            //    },
            //    Order = new List<QueryOrder>
            //    {
            //        new QueryOrder(false, QueryOrder.FieldOptions.Created)
            //    }
            //};
            // below to filter the lobby
            //QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            //QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            //OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });

            //Debug.Log("Lobbies found: " + lobbyListQueryResponse.Results.Count);
            //foreach (Lobby lobby in lobbyListQueryResponse.Results)
            //{
            //    PrintPlayers(lobby);
            //    //Debug.Log("Lobby name: " + lobby.Name + " " + "Max players: " + lobby.MaxPlayers /*+ " " + lobby.Data["GameMode"].Value*/);
            //}
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {lobbyList = queryResponse.Results });
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
    public async void RefreshLobbyList()
    {
        try
        {
            //QueryLobbiesOptions options = new QueryLobbiesOptions();
            //options.Count = 25;

            //// Filter for open lobbies only
            //options.Filters = new List<QueryFilter> {
            //    new QueryFilter(
            //        field: QueryFilter.FieldOptions.AvailableSlots,
            //        op: QueryFilter.OpOptions.GT,
            //        value: "0")
            //};

            //// Order by newest lobbies first
            //options.Order = new List<QueryOrder> {
            //    new QueryOrder(
            //        asc: false,
            //        field: QueryOrder.FieldOptions.Created)
            //};

            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
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
    
    [Command]
    public async void LeaveLobby()
    {
        if (IsPlayerInLobby()) {
        try {   //Don't need customized host migration
                //if (!IsLobbyHost())
                //{
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                    joinedLobby = null;
                    OnLeftLobby?.Invoke(this, EventArgs.Empty);
                //} else {
                //    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                //    joinedLobby = null;
                //    OnLeftLobby?.Invoke(this, EventArgs.Empty); 
                //    MigrateLobbyHost(); 
                //} 
            }
            catch (LobbyServiceException e) { Debug.Log(e); }
        }
    }

    [Command]
    private bool IsPlayerInLobby()
    {
        if (joinedLobby != null && joinedLobby.Players != null) 
        {
            try {
                foreach (Player player in joinedLobby.Players)
                {
                    if (player.Id == AuthenticationService.Instance.PlayerId)
                    {
                        // This player is in this lobby
                        return true;
                    }
                }
            } catch (LobbyServiceException e) { Debug.Log(e); }
        } return false;
        
    }

    [Command]
    public async void DeleteLobby()
    {
        if (IsPlayerInLobby() && IsLobbyHost())
        try 
        {      
          await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        } catch (LobbyServiceException e) { Debug.Log(e); }
    }

    [Command]
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
            Loader.LoadNetwork(Loader.Scene.Race_Net_Test);
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
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
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
}