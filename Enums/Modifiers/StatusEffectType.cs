namespace GodmistWPF.Enums.Modifiers;

public enum StatusEffectType
{
    Buff, // Gives positive effect to user
    Debuff, // Gives negative effect to user
    Bleed, // Causes user to take DoT (Damage over Time)
    Poison, // Causes user to take DoT (Damage over Time)
    Burn, // Causes user to take DoT (Damage over Time)
    Stun, // User is unable to take turns
    Freeze, // User is unable to take turns, and is slowed once the effect ends
    Frostbite, // Speed is reduced to 10
    Sleep, // User is stunned but regenerates health over time
    Invisible, // User cannot be targeted by skills
    Paralysis, // User is unable to act, and cannot be targeted by skills
    Provocation, // User's skill target must be the person who caused the provocation
    Shield, // Blocks a certain amount of damage in place of health
    Regeneration, // Regens a certain amount of health/resource per turn
}