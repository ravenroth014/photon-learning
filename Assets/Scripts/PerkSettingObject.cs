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


public struct PerkData : INetworkStruct
{
    public int PerkTree1;

    public PerkData(int perkTree1)
    {
        PerkTree1 = perkTree1;
    }
}

[Flags]
public enum PerkTreeType : byte
{
    Starter,
    MultiAttack,
    AttackDamage,
    FireRate,
    TechnicProjectile,
    ChargeAttack,
    Bomb,
    Dash,
    Taunt,
    Pyro,
    Cryo,
    Electro,
    Aero,
    Bio,
    Health,
    Shield,
    Speed,
    Bounty,
    Companion
}

[Flags]
public enum PerkStarter : byte
{
    PERK_HUNT_001,
    PERK_HUNT_002,
    PERK_HUNT_003,
    PERK_HUNT_004,
    PERK_HUNT_005,
    PERK_HUNT_006,
    PERK_HUNT_007,
    PERK_HUNT_008
}

[Flags]
public enum PerkMultiAttack : byte
{
    PERK_MULT_001,
    PERK_MULT_002,
    PERK_MULT_003,
    PERK_MULT_004
}

public enum PerkAttackDamage : byte
{
    PERK_ATK_001,
    PERK_ATK_002,
    PERK_ATK_003,
    PERK_ATK_004,
    PERK_ATK_005,
    PERK_ATK_006
}

public enum PerkFireRate : byte
{
    PERK_FIRE_001,
    PERK_FIRE_002,
    PERK_FIRE_003,
    PERK_FIRE_004
}

public enum PerkTechnicProjectile : byte
{
    PERK_PROJ_001,
    PERK_PROJ_002,
    PERK_PROJ_003,
    PERK_PROJ_004
}

public enum PerkChargeAttack : byte
{
    PERK_CHAR_001,
    PERK_CHAR_002,
    PERK_CHAR_003,
    PERK_CHAR_004
}

public enum PerkBomb : byte
{
    PERK_BOMB_001,
    PERK_BOMB_002,
    PERK_BOMB_003,
    PERK_BOMB_004,
    PERK_BOMB_005,
    PERK_BOMB_006
}

public enum PerkDash : byte
{
    PERK_DASH_001,
    PERK_DASH_002,
    PERK_DASH_003,
    PERK_DASH_004,
    PERK_DASH_005,
    PERK_DASH_006
}

public enum PerkTaunt : byte
{
    PERK_TAUN_001,
    PERK_TAUN_002,
    PERK_TAUN_003,
    PERK_TAUN_004,
    PERK_TAUN_005
}

public enum PerkPyro : byte
{
    PERK_PYRO_001,
    PERK_PYRO_002,
    PERK_PYRO_003,
    PERK_PYRO_004,
    PERK_PYRO_005,
    PERK_PYRO_006,
    PERK_PYRO_007,
    PERK_PYRO_008
}

public enum PerkCryo : byte
{
    PERK_CRYO_001,
    PERK_CRYO_002,
    PERK_CRYO_003,
    PERK_CRYO_004,
    PERK_CRYO_005,
    PERK_CRYO_006,
    PERK_CRYO_007,
    PERK_CRYO_008
}

public enum PerkElectro : byte
{
    PERK_ELEC_001,
    PERK_ELEC_002,
    PERK_ELEC_003,
    PERK_ELEC_004,
    PERK_ELEC_005,
    PERK_ELEC_006,
    PERK_ELEC_007
}

public enum PerkAero : byte
{
    PERK_AREO_001,
    PERK_AREO_002,
    PERK_AREO_003,
    PERK_AREO_004,
    PERK_AREO_005,
    PERK_AREO_006
}

public enum PerkBio : byte
{
    PERK_BIO_001,
    PERK_BIO_002,
    PERK_BIO_003,
    PERK_BIO_004,
    PERK_BIO_005
}

public enum PerkHealth : byte
{
    PERK_HEALTH_001,
    PERK_HEALTH_002,
    PERK_HEALTH_003,
    PERK_HEALTH_004,
    PERK_HEALTH_005,
    PERK_HEALTH_006,
    PERK_HEALTH_007
}

public enum PerkShield : byte
{
    PERK_SHIELD_001,
    PERK_SHIELD_002,
    PERK_SHIELD_003,
    PERK_SHIELD_004,
    PERK_SHIELD_005
}

public enum PerkSpeed : byte
{
    PERK_SPEED_001,
    PERK_SPEED_002,
    PERK_SPEED_003,
    PERK_SPEED_004,
    PERK_SPEED_005
}

public enum PerkBounty : byte
{
    PERK_BOUNTY_001,
    PERK_BOUNTY_002,
    PERK_BOUNTY_003,
    PERK_BOUNTY_004
}

public enum PerkCompanion : byte
{
    PERK_COMPANION_001,
    PERK_COMPANION_002,
    PERK_COMPANION_003,
    PERK_COMPANION_004
}