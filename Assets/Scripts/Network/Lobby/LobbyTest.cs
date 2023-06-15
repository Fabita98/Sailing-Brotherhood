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


public class LobbyTest : NetworkBehaviour
{
    public static LobbyTest Instance { get; private set; }
    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private string playerName;
    public const int maxPlayers = 4;

    public event EventHandler OnReadyChanged;


    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
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

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }

    private async void Start()
    {
        playerName = "Jack_Sparrow" + UnityEngine.Random.Range(1, 100);
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId + " as: " + playerName);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdate();
    }

    [Command]
    public async void CreateLobby()
    {
        try
        {
            string LobbyName = "MyLobby";
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions()
            {
                // if true -> not visible with ListLobbies
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "CasualRace") /*, DataObject.IndexOptions.S1*/}
                }
            };

            // this for customized lobby creation options
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(LobbyName, maxPlayers, createLobbyOptions);

            hostLobby = lobby;

            Debug.Log("Created Lobby: " + lobby.Name + " Max Players: " + lobby.MaxPlayers + " Lobby ID: " + lobby.Id + " Lobby Code: " + lobby.LobbyCode);
            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e) { Debug.Log(e); }
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


            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log("Lobby name: " + lobby.Name + " " + "Max players: " +  lobby.MaxPlayers /*+ " " + lobby.Data["GameMode"].Value*/);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
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
    [Command]
    public async void QuickJoinLobby()
    {
        try
        {
            Lobby joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            Debug.Log("Joined lobby: " + joinedLobby.Name);
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
                    {   //visible to each lobby member: private -> only host, public -> public
                        { "PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName ) }
                    }
        };
    }
    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in lobby named: " + lobby.Name /*+ " " + lobby.Data["GameMode"].Value*/);
        foreach (Player player in lobby.Players)
        {
            Debug.Log("Player ID: " + player.Id + " " + player.Data["PlayerName"].Value);
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
    [Command]
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
    [Command]
    private async void LeaveLobby()
    {
        try
        {
          await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        } catch (LobbyServiceException e) { Debug.Log(e); }
    }
    
    private async void KickPlayer()
    {
        try
        {   // kicks the 2nd player: the first player who is gonna to be the host
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
        }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }
    
    private async void MigrateLobbyHost()
    {
        // Customized host selection after host has left
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id,
            });
        }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }
    [Command]
    public async void DeleteLobby()
    {
        try 
        {      
          await LobbyService.Instance.DeleteLobbyAsync(hostLobby.Id);
        } catch (LobbyServiceException e) { Debug.Log(e); }
    }
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
            LobbyTest.Instance.DeleteLobby();
            // Race_Net_Test andrà sostituito con la scena finale networkata
            Loader.LoadNetwork(Loader.Scene.Pirate_Test_NETWORK);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }
    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
}