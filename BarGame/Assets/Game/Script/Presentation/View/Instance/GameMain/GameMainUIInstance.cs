using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Presentation.View
{
    [System.Serializable]
    public sealed class GameMainUIInstance
    //UIのインスタンスを管理する
    {
        public Button pauseButton = null!;
        public Button resumeButton = null!;
        public GameObject pauseWindow = null!;
    }
}
