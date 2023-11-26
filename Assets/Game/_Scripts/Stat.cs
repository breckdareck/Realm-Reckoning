using System;

namespace Game._Scripts
{
    
    [Serializable]
    public enum GeneralStat
    {
        Level,
        Experience,
        StarRating,
        Strength,
        Agility,
        Magik,
        AdherenceToCommand,
        Leadership,
        Health,
        Speed,
        Potency,
        Resilience,
        Bloodlust,
        CriticalDamage,
        DefenseNegation,
        PhysicalOffense,
        PhysicalCriticalChance,
        ArmorPierce,
        PhysicalAccuracy,
        Armor,
        PhysicalDodge,
        PhysicalCriticalAvoidance,
        MagikOffense,
        MagikCriticalChance,
        MagikArmorPierce,
        MagikAccuracy,
        MagikArmor,
        MagikDodge,
        MagikCriticalAvoidance,
    }
    

// Level-up bonuses
    public enum LevelUpBonus
    {
        StrengthPerLevel,
        AgilityPerLevel,
        MagikPerLevel,
        ArmorPerLevel,
        MagikArmorPerLevel,
    }
}