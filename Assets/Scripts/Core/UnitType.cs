namespace RPG.Core {
    public class UnitType
    {
        private static readonly string[] types = {"Player", "Ally", "Enemy"};

        public static string GetType(Type type) {
            int index = (int)type;
            if (index > types.Length) {
                return "";
            }
            return types[index % types.Length];
        }

        public enum Type {
            PLAYER = 0,
            ALLY = 1,
            ENEMY = 2
        }
    }
}
