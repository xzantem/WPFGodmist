using DungeonFieldType = GodmistWPF.Enums.Dungeons.DungeonFieldType;

namespace GodmistWPF.Dungeons
{
    public abstract class DungeonField(DungeonFieldType fieldType)
    {
        public DungeonFieldType FieldType {get;private set;} = fieldType;
        public bool Revealed {get; private set;}

        public void Reveal() {
            Revealed = true;
        }

        public void Clear()
        {
            FieldType = DungeonFieldType.Empty;
        }
    }
}