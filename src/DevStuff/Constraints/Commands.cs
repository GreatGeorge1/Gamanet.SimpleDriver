using System.Collections.Generic;

namespace DevStuff.Constraints
{
    public enum Commands
    {
        None = 0,
        Text,
        Sound
    }
    public static class Data
    {
        public static Dictionary<Commands, byte> GetCommands()
        {
            return new Dictionary<Commands, byte> {
                {Commands.Text, 84},
                {Commands.Sound, 83}
            };
        }
    }
}