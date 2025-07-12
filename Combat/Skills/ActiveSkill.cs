using System.Windows;
using System.Windows.Media;
using GodmistWPF.Dialogs;
using GodmistWPF.Utilities;
using BattleEventData = GodmistWPF.Combat.Modifiers.PassiveEffects.BattleEventData;
using BattleUser = GodmistWPF.Combat.Battles.BattleUser;
using Character = GodmistWPF.Characters.Character;
using NameAliasHelper = GodmistWPF.Utilities.NameAliasHelper;
using ResourceType = GodmistWPF.Enums.ResourceType;
using SkillTarget = GodmistWPF.Enums.SkillTarget;

namespace GodmistWPF.Combat.Skills;

/// <summary>
/// Reprezentuje umiejętność aktywną, którą postać może użyć w walce.
/// </summary>
/// <remarks>
/// Klasa zarządza kosztami, efektami i logiką użycia umiejętności.
/// Obsługuje różne typy efektów, koszty zasobów i mechanikę trafień.
/// </remarks>
public class ActiveSkill
{
    /// <summary>
    /// Pobiera lokalizowaną nazwę umiejętności.
    /// </summary>
    public string Name => NameAliasHelper.GetName(Alias);
    /// <summary>
    /// Pobiera lub ustawia wewnętrzny identyfikator umiejętności.
    /// </summary>
    public string Alias { get; set; }
    /// <summary>
    /// Pobiera lub ustawia koszt zasobów potrzebny do użycia umiejętności.
    /// </summary>
    public int ResourceCost { get; set; }
    /// <summary>
    /// Pobiera lub ustawia koszt punktów akcji wyrażony jako ułamek maksymalnej puli AP.
    /// </summary>
    public double ActionCost { get; set; }
    /// <summary>
    /// Pobiera lub ustawia wartość określającą, czy umiejętność zawsze trafia.
    /// </summary>
    public bool AlwaysHits { get; set; }
    /// <summary>
    /// Pobiera lub ustawia dokładność umiejętności, która wpływa na szansę trafienia.
    /// </summary>
    public int Accuracy { get; set; }
    /// <summary>
    /// Pobiera lub ustawia liczbę trafień zadawanych przez umiejętność.
    /// </summary>
    public int Hits { get; set; }
    /// <summary>
    /// Pobiera lub ustawia listę efektów aktywowanych przez umiejętność.
    /// </summary>
    public List<IActiveSkillEffect> Effects { get; set; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="ActiveSkill"/>.
    /// </summary>
    public ActiveSkill()
    {
        
    }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="ActiveSkill"/> z określonymi parametrami.
    /// </summary>
    /// <param name="alias">Identyfikator umiejętności.</param>
    /// <param name="resourceCost">Koszt zasobów.</param>
    /// <param name="actionCost">Koszt punktów akcji (jako ułamek maksymalnej puli).</param>
    /// <param name="alwaysHits">Czy umiejętność zawsze trafia.</param>
    /// <param name="accuracy">Dokładność umiejętności.</param>
    /// <param name="effects">Lista efektów umiejętności.</param>
    /// <param name="hits">Liczba trafień (domyślnie 1).</param>
    public ActiveSkill(string alias, int resourceCost, double actionCost, bool alwaysHits, int accuracy,
        List<IActiveSkillEffect> effects, int hits = 1)
    {
        Alias = alias;
        ResourceCost = resourceCost;
        ActionCost = actionCost;
        AlwaysHits = alwaysHits;
        Accuracy = accuracy;
        Effects = effects;
        Hits = hits;
    }

    /// <summary>
    /// Wykonuje umiejętność na określonym przeciwniku.
    /// </summary>
    /// <param name="caster">Użytkownik umiejętności.</param>
    /// <param name="enemy">Przeciwnik, na którym wykonywana jest umiejętność.</param>
    /// <remarks>
    /// Sprawdza dostępne zasoby, zużywa punkty akcji i stosuje efekty umiejętności.
    /// Loguje informacje o użyciu umiejętności w interfejsie użytkownika.
    /// </remarks>
    public void Use(BattleUser caster, BattleUser enemy)
    {
        var resourceCost = (int)UtilityMethods.CalculateModValue(ResourceCost, caster.User.PassiveEffects.GetModifiers("ResourceCost"));
        
        // Check if caster has enough resources and action points
        if ((!(caster.User.CurrentResource >= resourceCost) && 
            (caster.User.ResourceType != ResourceType.Fury || 
             !(Math.Abs(caster.User.MaximalResource - caster.User.CurrentResource) < 0.001)) && resourceCost > 0) || 
            caster.CurrentActionPoints < caster.MaxActionPoints.BaseValue * ActionCost)
        {
            // Log failed skill usage due to insufficient resources/AP
            Application.Current.Dispatcher.Invoke(() => 
            {
                if (Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault() is { } dialog)
                {
                    dialog.AddToBattleLog($"{caster.User.Name} doesn't have enough resources to use {Name}!", Brushes.Yellow);
                }
            });
            return;
        }
        
        // Log skill usage
        Application.Current.Dispatcher.Invoke(() => 
        {
            if (Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault() is { } dialog)
            {
                dialog.AddToBattleLog($"{caster.User.Name} uses {Name}!", Brushes.White);
            }
        });
        
        var toHit = AlwaysHits ? (true, 1) : CheckHit(caster.User, enemy.User);
        caster.User.UseResource(resourceCost);
        caster.UseActionPoints(caster.MaxActionPoints.BaseValue * ActionCost);
        
        // Apply self effects
        foreach (var effect in Effects.Where(x => x.Target == SkillTarget.Self)) 
            effect.Execute(caster.User, enemy.User, Alias);
            
        if (Effects.All(x => x.Target != SkillTarget.Enemy)) 
            return;
            
        // Apply enemy effects
        for (var i = 0; i < Hits; i++)
        {
            if (!toHit.Item1)
            {
                // Log missed attack
                Application.Current.Dispatcher.Invoke(() => 
                {
                    if (Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault() is { } dialog)
                    {
                        dialog.AddToBattleLog($"{enemy.User.Name} dodged the attack!", Brushes.Yellow);
                    }
                });
                continue;
            }
            
            // Log successful hit
            Application.Current.Dispatcher.Invoke(() => 
            {
                if (Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault() is { } dialog)
                {
                    dialog.AddToBattleLog($"{caster.User.Name}'s {Name} hits {enemy.User.Name}!", Brushes.White);
                }
            });
            
            // Apply effects
            foreach (var effect in Effects.Where(x => x.Target == SkillTarget.Enemy))
            {
                effect.Execute(caster.User, enemy.User, Name);
                caster.User.PassiveEffects.HandleBattleEvent(new BattleEventData("OnHit", caster, enemy));
            }
        }
    }

    /// <summary>
    /// Generates a description for the skill based on its effects and properties.
    /// </summary>
    public string GenerateDescription(ResourceType resourceType, double maxActionPoints)
    {
        if (Effects == null || !Effects.Any())
            return "No effects";

        var description = new System.Text.StringBuilder();
        if (ResourceCost > 0)
        {
            description.AppendLine($"Cost: {ResourceCost} {resourceType} | {ActionCost:P0} ({ActionCost * maxActionPoints:F0}) AP ");
        }
        else
        {
            description.AppendLine($"Cost: {ActionCost:P0} ({ActionCost * maxActionPoints:F0}) AP");
        }
        // Add hit information
        if (AlwaysHits)
        {
            description.AppendLine("Always hits");
        }
        else if (Accuracy > 0)
        {
            description.AppendLine($"Accuracy: {Accuracy}");
        }
        
        // Add hits information
        if (Hits > 1)
        {
            description.AppendLine($"Hits {Hits} times");
        }
        
        // Add effect descriptions
        var effectDescriptions = Effects
            .Select(e => GetEffectDescription(e))
            .Where(d => !string.IsNullOrEmpty(d));
            
        description.Append(string.Join("\n", effectDescriptions));
        
        return description.ToString().Trim();
    }
    
    /// <summary>
    /// Gets a human-readable description of a skill effect.
    /// </summary>
    private string GetEffectDescription(IActiveSkillEffect effect)
    {
        // This is a simple implementation - you might want to expand this
        // to handle different effect types more specifically
        return effect.ToString().Split('.').Last();
    }

    /// <summary>
    /// Sprawdza, czy umiejętność trafia w cel.
    /// </summary>
    /// <param name="caster">Postać używająca umiejętności.</param>
    /// <param name="target">Cel umiejętności.</param>
    /// <returns>Krotka zawierająca informację o trafieniu i procentową szansę na trafienie.</returns>
    private (bool, double) CheckHit(Character caster, Character target)
    {
        var accuracy = (caster.Accuracy + Accuracy) / 2;
        var hitChance = accuracy * accuracy / (accuracy + target.Dodge);
        hitChance = Math.Min(UtilityMethods.CalculateModValue(hitChance, 
            caster.PassiveEffects.GetModifiers("HitChanceMod")), 100);
        return (Random.Shared.NextDouble() * 100 < hitChance, hitChance / 100);
    }
}