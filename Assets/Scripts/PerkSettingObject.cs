using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[CreateAssetMenu(fileName = "PerkSetting", menuName = "Perks/Perk Setting")]
public class PerkSettingObject : ScriptableObject
{
    [SerializeField] private List<PerkSetting> _perkSettingList;
    public IReadOnlyList<PerkSetting> PerkSettingList => _perkSettingList;
}

[Serializable]
public struct PerkSetting : INetworkStruct
{
    public int Attack;
    public int MaxHp;
    public int Defense;
}
