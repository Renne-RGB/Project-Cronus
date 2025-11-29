using UnityEngine;

namespace Assets.Code
{
    public class HitInfo
    {
        public HitInfo(Vector2 basicPoint, bool isAssistPoint)
        {
            this.isAssistPoint = isAssistPoint;
            this.basicPoint = basicPoint;
        }
        public Vector2 toPointDirection { get; set; }
        public bool isAssistPoint { get; }
        public Vector2 basicPoint { get; }
        public Vector2 leftPoint { get; set; }
        public Vector2 rightPoint { get; set; }
        public Vector2 toLeftPointDirection { get; set; }
        public Vector2 toRightPointDirection { get; set; }
        public bool leftHit { get; set; }
        public bool rightHit { get; set; }
        public Vector2 leftHitPoint { get; set; }
        public Vector2 rightHitPoint { get; set; }
        public Vector2 leftHitNormal { get; set; }
        public Vector2 rightHitNormal { get; set; }
    }
}