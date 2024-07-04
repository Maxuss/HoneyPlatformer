using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Level;
using Program;
using Program.Channel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Objects.Executors
{
    public class Teleporter: MonoBehaviour, IActionContainer, IChannelReceiver
    {
        [SerializeField]
        [ColorUsage(false, true)]
        private Color[] voidColors;
        [SerializeField]
        [ColorUsage(false, true)]
        private Color[] azureColors;
        [SerializeField]
        [ColorUsage(false, true)]
        private Color[] pyroColors;
        [SerializeField]
        [ColorUsage(false, true)]
        private Color[] scarletColors;
        [SerializeField]
        [ColorUsage(false, true)]
        private Color[] electroColors;
        [SerializeField]
        [ColorUsage(false, true)]
        private Color[] platinumColors;

        [SerializeField] private ParticleSystem tpParticles;
        [SerializeField]
        private AudioClip tpSound;
        [SerializeField]
        private AudioClip tpChargeSound;
        [SerializeField]
        private LayerMask teleportMask;

        [SerializeField]
        private Transform tpPos;
        
        [SerializeField]
        private TeleporterChannel channel;
        private Renderer _renderer;

        [SerializeField]
        private BoxCollider2D collider;
        
        [Space(50)]
        [SerializeField]
        private TeleporterChannel[] blacklistedChannels;
        
        private bool _state;
        private static readonly int TpColor1 = Shader.PropertyToID("_TpColor1");
        private static readonly int TpColor2 = Shader.PropertyToID("_TpColor2");

        
        public string Name => "Телепорт";
        public string Description => "Используется для (практически) мгновенного перемещения объектов по разным каналам";

        public ActionInfo[] SupportedActions => new ActionInfo[]
        {
            new()
            {
                ActionName = "Телепортировать",
                ActionDescription = "Производит телепортацию объект в другой телепорт канала. Если в канале более двух телепортов, перемещение не удастся",
                ValueType = ActionValueType.Enum,
                EnumType = typeof(TeleporterChannel),
                ParameterName = "Канал",
                BlacklistedEnumTypes = blacklistedChannels.Cast<int>().ToList()
            },
        };
        public ProgrammableType Type { get; }
        public ActionData SelectedAction { get; set; }

        private TeleporterChannel[] _blacklistedChannels;
        
        
         
        public void Begin(ActionData action)
        {
            channel = _blacklistedChannels[(int) action.StoredValue!];
            var colors = Channel2Colors();
            _renderer.material.SetColor(TpColor1, colors.Item1);
            _renderer.material.SetColor(TpColor2, colors.Item2);
        }

        public void ReceiveBool(Transform source, bool b)
        {
            if (b)
            {
                StartCoroutine(TeleportCoroutine());
            }
        }

        private IEnumerator TeleportCoroutine()
        {
            SfxManager.Instance.Play(tpChargeSound);
            tpParticles.Play();
            yield return new WaitForSeconds(1.5f);
            var destination = SceneManager.GetActiveScene().GetRootGameObjects()
                .Where(it => it.CompareTag("Teleporter") && it.gameObject != gameObject && it.GetComponent<Teleporter>().channel == channel).ToList();
            if (destination.Count != 1)
            {
                // failed to teleport
                tpParticles.Stop();
                yield break;
            }

            var dest = destination[0].GetComponent<Teleporter>().tpPos;

            var objects = new List<Collider2D>();
            Physics2D.OverlapBox(transform.position, collider.size, 0f, new ContactFilter2D(), objects);
            foreach (var toTeleport in objects.Where(it => (teleportMask & (1 << it.gameObject.layer)) != 0))
            {
                var tf = toTeleport.transform;
                var deltaFromCenter = transform.position - tf.position;
                var final = dest.position + deltaFromCenter;
                tf.position = final;
            }
            SfxManager.Instance.Play(tpSound);
            tpParticles.Stop();
        }

        public void ReceiveFloat(Transform source, float v)
        {
        }
        
        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            var colors = Channel2Colors();
            _renderer.material.SetColor(TpColor1, colors.Item1);
            _renderer.material.SetColor(TpColor2, colors.Item2);
            tpParticles.Stop();
            _blacklistedChannels = Enum.GetValues(typeof(TeleporterChannel)).Cast<TeleporterChannel>()
                .Where(it => !blacklistedChannels.Contains(it)).ToArray();

            var act = SelectedAction;
            act.StoredValue = (int)channel;
            SelectedAction = act;
        }

        private (Color, Color) Channel2Colors()
        {
            return channel switch
            {
                TeleporterChannel.Void => (voidColors[0], voidColors[1]),
                TeleporterChannel.Azure => (azureColors[0], azureColors[1]),
                TeleporterChannel.Pyro => (pyroColors[0], pyroColors[1]),
                TeleporterChannel.Scarlet => (scarletColors[0], scarletColors[1]),
                TeleporterChannel.Electro => (electroColors[0], electroColors[1]),
                TeleporterChannel.Platinum => (platinumColors[0], platinumColors[1]),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    [Serializable]
    public enum TeleporterChannel
    {
        Void,
        Azure,
        Pyro,
        Scarlet,
        Electro,
        Platinum,
    }
}