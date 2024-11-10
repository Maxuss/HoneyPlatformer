using UnityEngine;

namespace DonDomain.BG.Impl
{
    public class RedBackground: DonBackground
    {
        public Color color1;
        public Color color2;
        public Color color3;
        [ColorUsage(false, true)]
        public Color glow;

        public override void Change(BackgroundTweener bg)
        {
            bg.DoColor1(color1);
            bg.DoColor2(color2);
            bg.DoColor3(color3);
            bg.DoHighlights(glow);
            bg.DoSpinEasing(0.11f);
            bg.DoPixelSizeFactor(700);
            bg.DoSpinSpeed(45);
            bg.DoSpinAmount(1f);
            bg.DoContrast(4);
        }
    }
}