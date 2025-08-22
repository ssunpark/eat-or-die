using System;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : BehaviourSingleton<CurrencyManager>
{
    private Dictionary<ECurrencyType, int> _currencies = new();
    private readonly int _gold = 2000;
    public event Action<ECurrencyType, int> OnCurrencyChanged;

    private void Awake()
    {
        Debug.Log("이니셜 호출");
        Initialize(new Dictionary<ECurrencyType, int>
        {
            { ECurrencyType.Gold, _gold }
        });
    }

    // 재화 딕셔너리를 초기화한다.
    public void Initialize(Dictionary<ECurrencyType, int> initialValues)
    {
        _currencies = new Dictionary<ECurrencyType, int>(initialValues);
    }

    // 특정 재화의 현재 보유량을 가져온다. 없을 경우 0 반환.
    public int Get(ECurrencyType type)
    {
        return _currencies.TryGetValue(type, out var value) ? value : 0;
    }

    // 해당 재화를 충분히 가지고 있는지 확인한다.
    public bool HasEnough(ECurrencyType type, int required)
    {
        return Get(type) >= required;
    }

    // 특정 재화를 차감한다. 보유량이 부족할 경우 false 반환. 성공 시 이벤트 발생.
    public bool TrySpend(ECurrencyType type, int amount)
    {
        if (!HasEnough(type, amount))
        {
            return false;
        }
        _currencies[type] -= amount;
        OnCurrencyChanged?.Invoke(type, _currencies[type]);
        return true;
    }

    // 특정 재화를 추가한다. 없을 경우 자동으로 0으로 초기화 후 더함. 이벤트 발생.
    public void Add(ECurrencyType type, int amount)
    {
        if (!_currencies.ContainsKey(type))
        {
            _currencies[type] = 0;
        }
        _currencies[type] += amount;
        OnCurrencyChanged?.Invoke(type, _currencies[type]);
    }

    // 현재 보유 중인 모든 재화 정보를 복사해서 반환한다. 저장 용도 등으로 활용 가능.
    public Dictionary<ECurrencyType, int> GetAllCurrencies()
    {
        return new Dictionary<ECurrencyType, int>(_currencies);
    }
}