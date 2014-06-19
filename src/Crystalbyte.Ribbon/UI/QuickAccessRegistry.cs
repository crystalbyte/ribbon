using System.Collections.Generic;
using System.Linq;

namespace Crystalbyte.UI {
    public static class QuickAccessRegistry {

        private static readonly List<IQuickAccessConform> Commands 
            = new List<IQuickAccessConform>();
        
        public static void Register(IQuickAccessConform command) {
            Commands.Add(command);
        }

        public static IQuickAccessConform Find(string key) {
            return Commands.FirstOrDefault(x => x.Key == key);
        }
    }
}
