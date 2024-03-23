using System.Collections;
using System.Linq;
using NPC;
using Program;
using Program.Channel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Objects.Executors
{
    public class ShieldGenerator: MonoBehaviour, IChannelReceiver, IActionContainer
    {
        [SerializeField]
        private GameObject leftShield;
        [SerializeField]
        private GameObject rightShield;

        private bool _active;

        private void Start()
        {
            leftShield.SetActive(false);
            rightShield.SetActive(false);
        }

        private void StartShields()
        {
            // TODO: IMPORTANT!!! activation sound

            leftShield.SetActive(true);
            rightShield.SetActive(true);

            StartCoroutine(RepelBees());
        }

        private IEnumerator RepelBees()
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("REPELLING");
            var bees = SceneManager
                .GetActiveScene().GetRootGameObjects();
            foreach(var b in bees)
            {
                Debug.Log($"B: {b}");
            }
            
            foreach (var bee in bees
                         .Where(each => each.CompareTag("Bee"))
                         .Select(each => each.GetComponent<BeeController>()))
            {
                bee.Escape();
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        public void ReceiveBool(Transform source, bool b)
        {
            if (b && !_active)
            {
                _active = true;
                StartShields();
            }
        }

        public void ReceiveFloat(Transform source, float v)
        {
        }

        public string Name => "Генератор щитов";
        public string Description => "Создает поле отпугивающее кибер ос.";

        public ActionInfo[] SupportedActions { get; } = new[]
        {
            new ActionInfo
            {
                ActionName = "Поднять щиты",
                ActionDescription = "Материализует щиты. Работает все время после первого получения сигнала."
            }
        };
        public ProgrammableType Type => ProgrammableType.Executor;
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            // noop
        }
    }
}