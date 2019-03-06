using System.Collections.Generic;

namespace con2.game
{
    public static class Players
    {
        private static Dictionary<int, PlayerManager> _Dic = null;
        
        public static Dictionary<int, PlayerManager> Dic
            => _Dic ?? (_Dic = new Dictionary<int, PlayerManager>());
    
        public static PlayerManager GetPlayerByID(int id) 
            => _Dic[id];
    }
}