using UnityEngine;

namespace DonDomain.BG.Impl
{
    public class EvilBackground: DonBackground
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
            bg.DoSpinEasing(0.19f);
            bg.DoPixelSizeFactor(600);
            bg.DoSpinSpeed(50);
            bg.DoSpinAmount(2f);
            bg.DoContrast(8);
        }
    }
}