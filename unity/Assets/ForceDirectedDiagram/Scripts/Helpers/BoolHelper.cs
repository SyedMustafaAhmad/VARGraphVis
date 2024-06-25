namespace ForceDirectedDiagram.Scripts.Helpers
{
    public static class BoolHelper
    {
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        public static bool ToBool(this int value)
        {
            return value == 1;
        }
    }
}