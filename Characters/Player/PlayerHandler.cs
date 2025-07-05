using GodmistWPF.Enums;

namespace GodmistWPF.Characters.Player
{
    public static class PlayerHandler {
        public static PlayerCharacter player;

        public static double HonorDiscountModifier => player.HonorLevel switch
        {
            HonorLevel.Exile => 2,
            HonorLevel.Useless => 1.75,
            HonorLevel.Shameful => 1.5,
            HonorLevel.Uncertain => 1.25,
            HonorLevel.Recruit => 1,
            HonorLevel.Mercenary => 0.95,
            HonorLevel.Fighter => 0.9,
            HonorLevel.Knight => 0.85,
            HonorLevel.Leader => 0.8,
            _ => throw new ArgumentOutOfRangeException()
        };
        public static double HonorExperienceModifier => player.HonorLevel switch
        {
            HonorLevel.Exile => 1,
            HonorLevel.Useless => 1,
            HonorLevel.Shameful => 1,
            HonorLevel.Uncertain => 1,
            HonorLevel.Recruit => 1,
            HonorLevel.Mercenary => 1,
            HonorLevel.Fighter => 1.1,
            HonorLevel.Knight => 1.2,
            HonorLevel.Leader => 1.5,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}