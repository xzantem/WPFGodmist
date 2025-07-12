using DungeonFieldType = GodmistWPF.Enums.Dungeons.DungeonFieldType;

namespace GodmistWPF.Dungeons
{
    /// <summary>
    /// Klasa reprezentująca pokój w lochu, będący specjalnym typem pola, które może zawierać różne elementy interaktywne.
    /// </summary>
    public class DungeonRoom : DungeonField {
        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="DungeonRoom"/> z określonym typem pola.
        /// </summary>
        /// <param name="fieldType">Typ pola pokoju w lochu.</param>
        public DungeonRoom(DungeonFieldType fieldType) : base(fieldType) {}
        
    }
}