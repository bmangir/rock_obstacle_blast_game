using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Data
{
    [System.Serializable]
    public class LevelData
    {
        public int level_number;
        public int grid_width;
        public int grid_height;
        public int move_count;
        public List<string> grid;
    }
}