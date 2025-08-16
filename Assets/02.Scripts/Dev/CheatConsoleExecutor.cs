using System;
using System.Linq;
using Fusion;
using UnityEngine;

public class CheatConsoleExecutor : NetworkBehaviour
{
    private Action<string> _log;

    public void TryExecute(string line, Action<string> logger)
    {
        _log = logger;
        RPC_RequestExecute(line);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestExecute(string line, RpcInfo info = default)
    {
        string result = ExecuteOnServer(line, info);
        RPC_Reply(result);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_Reply(string message)
    {
        _log?.Invoke(message);
    }

    private string ExecuteOnServer(string line, RpcInfo info)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(line)) return "Empty command.";
            var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string cmd = tokens[0].ToLowerInvariant();

            switch (cmd)
            {
                case "trait": return Cmd_Trait(tokens, info);
                case "giveitem": return Cmd_GiveItem(tokens);
                case "tp": return Cmd_Tp(tokens, info);
                case "hp": return Cmd_HpMp(tokens, info, isHp: true);
                case "mp": return Cmd_HpMp(tokens, info, isHp: false);
                case "enemy": return Cmd_Enemy(tokens, info);
                case "spawner": return Cmd_Spawner(tokens, info);
                default: return $"Unknown command '{cmd}'.";
            }
        }
        catch (Exception ex)
        {
            return "[Cheat] ERROR: " + ex.Message;
        }
    }

    // trait [lv|exp] [TraitEnum] [Value]
    //  - lv  : 해당 트레잇 레벨을 '정확히' 설정 (ForceSetLevel)
    //  - exp : 해당 트레잇 경험치를 '증가'(가산) 처리
    private string Cmd_Trait(string[] t, RpcInfo info)
    {
        if (t.Length < 4) return "Usage: trait [lv|exp] [TraitEnum] [Value]";

        string sub = t[1].ToLowerInvariant();
        if (!Enum.TryParse<ETraitType>(t[2], true, out var traitType))
            return $"Unknown TraitEnum '{t[2]}'";

        if (!float.TryParse(t[3], out float fval))
            return $"Invalid Value '{t[3]}'";

        var player = GetRequestPlayer(info);
        if (player == null) return "Player not found.";

        var data = player.TraitDataList?.FirstOrDefault(d => d.TraitType == traitType);
        if (data == null) return $"TraitData not found for {traitType}.";

        switch (sub)
        {
            case "lv":
            case "level":
                int level = Mathf.Max(0, Mathf.RoundToInt(fval));
                player.Trait.ForceSetLevel(traitType, level, data); // 레벨 강제 세팅 
                return $"Set {traitType} level = {level}";

            case "exp":
                int amt = Mathf.RoundToInt(fval);
                if (amt == 0) return "exp delta is 0.";
                player.Trait.AddExp(traitType, amt, data);          // 경험치 가산 처리 
                return $"Added {amt} exp to {traitType}";
            

            default:
                return "Usage: trait [lv|exp] [TraitEnum] [Value]";
        }
    }

    // giveitem [itemId] [quantity] [durability]
    // durability 생략 시 1
    private string Cmd_GiveItem(string[] t)
    {
        if (t.Length < 2) return "Usage: giveitem [itemId] [quantity] [durability]";
        if (!int.TryParse(t[1], out int itemId)) return "Invalid itemId";

        int quantity = 1;
        if (t.Length >= 3) int.TryParse(t[2], out quantity);
        quantity = Mathf.Max(1, quantity);

        int durability = 1;
        if (t.Length >= 4) int.TryParse(t[3], out durability);
        durability = Mathf.Max(1, durability);

        // 프로젝트 인벤토리 API 직접 호출(서버에서 처리)
        var item = ItemManager.Instance.GetItem(itemId);
        var inst = new ItemInstance(item, quantity, durability);
        UnifiedInventoryManager.Instance.AddItem(inst);
        return $"Gave item {itemId} x{quantity} (durability {durability})";
    }

    // tp [x] [y] [z]  (요청 보낸 본인 이동)
    private string Cmd_Tp(string[] t, RpcInfo info)
    {
        if (t.Length < 4) return "Usage: tp [x] [y] [z]";
        if (!float.TryParse(t[1], out float x)) return "Invalid x";
        if (!float.TryParse(t[2], out float y)) return "Invalid y";
        if (!float.TryParse(t[3], out float z)) return "Invalid z";

        var player = GetRequestPlayer(info);
        if (player == null) return "Player not found.";

        Vector3 pos = new Vector3(x, y, z);

        player.Teleport(pos);
        return $"Teleported to ({x}, {y}, {z})";
    }

    // hp|mp [value]  (HP=Hunger, MP=Mana)
    private string Cmd_HpMp(string[] t, RpcInfo info, bool isHp)
    {
        if (t.Length < 2) return isHp ? "Usage: hp [value]" : "Usage: mp [value]";
        if (!float.TryParse(t[1], out float v)) return $"Invalid value '{t[1]}'";

        var player = GetRequestPlayer(info);
        if (player == null) return "Player not found.";

        if (isHp)
        {
            player.Resource.SetHunger(v); // HP 세팅(=Hunger) 
            return $"HP set to {player.Resource.CurrentHunger}/{player.Resource.MaxHunger}";
        }
        else
        {
            player.Resource.SetMana(v);   // MP 세팅(=Mana) 
            return $"MP set to {player.Resource.CurrentMana}/{player.Resource.MaxMana}";
        }
    }

    private Player GetRequestPlayer(RpcInfo info)
    {
        // 1) RPC 보낸 주체
        var who = info.Source;

        // 2) 폴백: 이 Behaviour가 속한 NetworkObject의 입력권자
        if (who.IsNone && Object != null)
            who = Object.InputAuthority;

        if (who.IsNone)
            return null;

        if (PlayerInfoManager.PlayerControllers != null &&
            PlayerInfoManager.PlayerControllers.TryGetValue(who, out var p) && p)
            return p;

        var po = Runner?.GetPlayerObject(who);
        return po ? po.GetComponent<Player>() : null;
    }

    private string Cmd_Enemy(string[] t, RpcInfo info)
    {
        if (t.Length < 2) return "Usage: enemy [spawn|spawnhere|killall] ...";

        string sub = t[1].ToLowerInvariant();
        switch (sub)
        {
            case "spawn":
                {
                    int count = (t.Length >= 3 && int.TryParse(t[2], out var c)) ? Mathf.Max(1, c) : 1;
                    var sp = GetNearestSpawner(info);
                    if (sp == null) return "No EnemySpawner found.";
                    sp.SpawnBurst(count);
                    return $"Enemy spawned at nearest spawner x{count}";
                }
            case "spawnhere":
                {
                    int count = (t.Length >= 3 && int.TryParse(t[2], out var c)) ? Mathf.Max(1, c) : 1;
                    var player = GetRequestPlayer(info);
                    if (player == null) return "Player not found.";
                    var pos = player.SimpleKCC ? player.SimpleKCC.Transform.position : player.transform.position;
                    var sp = GetNearestSpawner(info);
                    if (sp == null) return "No EnemySpawner found.";
                    sp.SpawnAt(pos, count);
                    return $"Enemy spawned at player x{count}";
                }
            case "killall":
                {
                    var enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                    int n = 0;
                    foreach (var e in enemies)
                    {
                        if (e.TryGetComponent<NetworkObject>(out var no))
                        {
                            if (Runner.IsServer) Runner.Despawn(no);
                            n++;
                        }
                        else
                        {
                            Destroy(e.gameObject);
                        }
                    }
                    return $"Killed {n} enemies.";
                }
            default:
                return "Usage: enemy [spawn [count] | spawnhere [count] | killall]";
        }
    }

    private string Cmd_Spawner(string[] t, RpcInfo info)
    {
        if (t.Length < 2) return "Usage: spawner [start|stop|setinterval] ...";
        var sp = GetNearestSpawner(info);
        if (sp == null) return "No EnemySpawner found.";

        string sub = t[1].ToLowerInvariant();
        switch (sub)
        {
            case "start":
                {
                    if (t.Length < 4) return "Usage: spawner start [interval] [total]";
                    if (!float.TryParse(t[2], out var interval)) return "Invalid interval";
                    if (!int.TryParse(t[3], out var total)) return "Invalid total";
                    sp.StartAuto(interval, total);
                    return $"Spawner auto ON: every {interval}s, total {total}";
                }
            case "stop":
                sp.StopAuto();
                return "Spawner auto OFF";
            case "setinterval":
                {
                    if (t.Length < 3) return "Usage: spawner setinterval [seconds]";
                    if (!float.TryParse(t[2], out var sec)) return "Invalid seconds";
                    sp.StartAuto(sec, 0); // 카운트 0이면 타이머만 세팅, 자동은 꺼짐
                    sp.StopAuto();
                    return $"Spawner interval set to {sec}s";
                }
            default:
                return "Usage: spawner [start|stop|setinterval]";
        }
    }

    private EnemySpawner GetNearestSpawner(RpcInfo info)
    {
        if (EnemySpawner.Instances.Count == 0) return null;
        var player = GetRequestPlayer(info);
        if (player == null) return EnemySpawner.Instances[0];
        var p = player.SimpleKCC ? player.SimpleKCC.Transform.position : player.transform.position;
        EnemySpawner best = null;
        float bestDist = float.MaxValue;
        foreach (var s in EnemySpawner.Instances)
        {
            float d = Vector3.SqrMagnitude(s.transform.position - p);
            if (d < bestDist) { bestDist = d; best = s; }
        }
        return best;
    }
}
