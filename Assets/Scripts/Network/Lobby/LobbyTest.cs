using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using QFSW.QC;

public class LobbyTest : MonoBehaviour
{
    private Lobby hostLobby;
    private float heartbeatTimer;
    private string playerName;
    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0) {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }
    private async void Start()
    {
        playerName= "Jack_Sparrow" + Random.Range(1, 100);
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
    }

    [Command]
    private async void CreateLobby()
    {
        try
        {
            string LobbyName = "MyLobby";
            int maxPlayers = 4;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions()
            {
                // if true -> not visible with ListLobbies
                IsPrivate = false,
                Player = new Player
                {
                    //Id = AuthenticationService.Instance.PlayerId,
                    Data = new Dictionary<string, PlayerDataObject>
                    {   //visible to each lobby member: private -> only host, public -> public
                        { "PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName ) }
                    }
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(LobbyName, maxPlayers, createLobbyOptions);

            hostLobby = lobby;

            Debug.Log("Created Lobby: " + lobby.Name + " Max Players: " + lobby.MaxPlayers + " Lobby ID: " + lobby.Id + " Lobby Code: " + lobby.LobbyCode);
            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }
    [Command]
    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new() 
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }, 
                Order = new List<QueryOrder> 
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created) 
                }  
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies found:" + " " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results) { 
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers); 
            }
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]
    private async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            //QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
                      
            await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode);
            Debug.Log("Joined lobby with code: " + lobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in lobby named: " + lobby.Name);
        foreach (Player player in lobby.Players)
        {
            Debug.Log("Player ID: " + player.Id + " " + player.Data["PlayerName"].Value);
        }
    }
}
