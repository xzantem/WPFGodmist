using DungeonFieldType = GodmistWPF.Enums.Dungeons.DungeonFieldType;

namespace GodmistWPF.Dungeons
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa reprezentująca pojedyncze pole w lochu.
    /// </summary>
    public abstract class DungeonField(DungeonFieldType fieldType)
    {
        /// <summary>
        /// Pobiera lub ustawia typ pola lochu.
        /// </summary>
        public DungeonFieldType FieldType {get;private set;} = fieldType;
        /// <summary>
        /// Pobiera lub ustawia wartość określającą, czy pole zostało odkryte przez gracza.
        /// </summary>
        public bool Revealed {get; private set;}

        /// <summary>
        /// Oznacza pole jako odkryte.
        /// </summary>
        public void Reveal() {
            Revealed = true;
        }

        /// <summary>
        /// Czyści pole, ustawiając jego typ na pusty.
        /// </summary>
        public void Clear()
        {
            FieldType = DungeonFieldType.Empty;
        }
    }
}