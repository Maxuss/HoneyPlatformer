using System;
using System.Linq;
using Level;
using Objects.Executors;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Igroprom
{
    public class WatcherDoor: ExitDoor
    {
        public float beginTime;
        
        protected override void OnExit()
        {
            BotHandler.Instance.SendBotMessage($"Игрок завершил уровень `{SceneManager.GetSceneAt(0).name}` за `{Mathf.RoundToInt(Time.realtimeSinceStartup - beginTime)}` секунд");
        }

        protected override void OnExitDone()
        {
            var level = SceneManager.GetSceneByBuildIndex(nextLevel);
            var door = level.GetRootGameObjects().FirstOrDefault(it => it.TryGetComponent<WatcherDoor>(out var cmp));
            if (door != null)
                door.GetComponent<WatcherDoor>().beginTime = Time.realtimeSinceStartup;
        }
    }
}