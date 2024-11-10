using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Igroprom
{
    public class BotHandler: MonoBehaviour
    {
        public static BotHandler Instance { get; private set; }

        private ChatId _chat = new ChatId(-1002303725806);
        private TelegramBotClient _bot;
        private float idleCooldown = 10;
        
        private void Awake()
        {
            DontDestroyOnLoad(this);

            Instance = this;
            
            StartCoroutine(AwakeBot());
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
                return;

            idleCooldown -= Time.deltaTime;

            if (idleCooldown > 0)
                return;

            idleCooldown = 120;
            
            ScreenCapture.CaptureScreenshot("idle.png");
            // TODO: fix!!!
            Task.Run(async () =>
            {
                var file = System.IO.File.OpenRead("C:\\Users\\rm\\Projects\\HoneyPlatformer\\idle.png");
                await _bot.SendPhoto(_chat, InputFile.FromStream(file, "idle.png"),
                    "Игрок находится на одном уровне уже более двух минут!");
                System.IO.File.Delete("C:\\Users\\rm\\Projects\\HoneyPlatformer\\idle.png");
            });
        }

        public void SendBotMessage(string message)
        {
            Task.Run(async () => await SendMessageInternal(message));
        }

        private async Task SendMessageInternal(string message)
        {
            await _bot.SendMessage(_chat, message, parseMode: ParseMode.MarkdownV2);
        }

        private IEnumerator AwakeBot()
        {
            _bot = new TelegramBotClient("7927088014:AAEaZufaecrupKYXD3hL5vJlqNwvfIQTVVc");
            SendBotMessage("Бот запущен!");
            yield break;
        }
    }
}