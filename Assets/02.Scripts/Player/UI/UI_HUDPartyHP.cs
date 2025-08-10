using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
public class UI_HUDPartyHP: MonoBehaviour
{
    [SerializeField] private UI_PartySlot[] _partySlots;

    private float _checkTimer;
    private const float CheckInterval = 0.5f;


    private Dictionary<PlayerRef, UI_PartySlot> _slotMap = new();
    private Dictionary<PlayerRef, ResourceManager> _resourceMap = new();

    private void Start()
    {
        PlayerInfoManager.Instance.OnPlayerUnregistered += HandlePlayerUnregistered;
    }

    private void Update()
    {
        _checkTimer += Time.deltaTime;
        if (_checkTimer < CheckInterval || PlayerInfoManager.Instance.Object == null || !PlayerInfoManager.Instance.Object.IsValid) return;
        _checkTimer = 0f;
        CheckAndUpdatePlayers();
    }

    private void CheckAndUpdatePlayers()
    {
        var players = PlayerInfoManager.Instance.Players;

        for (int i = 0; i < players.Length; i++)
        {
            var playerInfo = players[i];

            if (!playerInfo.Ref.IsRealPlayer)
            {
                continue;
            }

            if (!_slotMap.TryGetValue(playerInfo.Ref, out var slot))
            {
                AssignSlotToPlayer(playerInfo);
            }
            else
            {
                if (slot.GetNickname() != playerInfo.Nickname.ToString())
                {
                    slot.SetNickName(playerInfo.Nickname.ToString());
                }
            }
        }
    }

    private void AssignSlotToPlayer(PlayerInfo info)
    {
        var emptySlot = _partySlots.FirstOrDefault(slot => slot.BoundPlayer == PlayerRef.None);
        if (emptySlot == null)
        {
            Debug.LogWarning("파티 슬롯이 부족합니다");
            return;
        }

        emptySlot.Bind(info.Ref, info.Nickname.ToString());
        _slotMap[info.Ref] = emptySlot;

        var player = PlayerInfoManager.Instance.GetPlayerFromNetworkId(info.NetworkId);
        if (player == null)
        {
            Debug.LogWarning($"Player not found for NetworkId: {info.NetworkId}");
            return;
        }

        if (player.Resource !=null)
        {
            player.Resource.OnHungerChanged += emptySlot.SetSliderValue;
            _resourceMap[info.Ref] = player.Resource;
        }
    }

    private void HandlePlayerUnregistered(PlayerRef playerRef, int index)
    {
        Debug.Log($"Player unregistered: {playerRef} at index {index}");
        if (_resourceMap.TryGetValue(playerRef, out var res))
        {
            res.OnHungerChanged -= _slotMap[playerRef].SetSliderValue;
            _resourceMap.Remove(playerRef);
            Debug.Log($"Unsubscribed from hunger changes for player: {playerRef}");
        }

        if (_slotMap.TryGetValue(playerRef, out var slot))
        {
            slot.ResetSlot();
            _slotMap.Remove(playerRef);

            Debug.Log($"Reset slot for player: {playerRef}");
        }
    }

    private void OnDestroy()
    {
        if (PlayerInfoManager.Instance != null)
            PlayerInfoManager.Instance.OnPlayerUnregistered -= HandlePlayerUnregistered;

        foreach (var pair in _resourceMap)
        {
            if (_slotMap.TryGetValue(pair.Key, out var slot))
                pair.Value.OnHungerChanged -= slot.SetSliderValue;
        }
        foreach (var slot in _slotMap.Values)
        {
            slot.ResetSlot();
        }
        _slotMap.Clear();
        _resourceMap.Clear();
    }
}