using System.Collections.Generic;
using UnityEngine;

namespace Dyzalonius.Sugondese.Entities
{
    public class BallsDisplay : MonoBehaviour
    {
        [SerializeField]
        private List<SpriteRenderer> ballRenderers = new List<SpriteRenderer>();

        private void Start()
        {
            ballRenderers.ForEach(x => x.color = Color.clear);
        }

        public void RearrangeBalls(List<BallType> balls)
        {
            for (int i = 0; i < ballRenderers.Count; i++)
            {
                // Get ballColor
                Color ballColor = Color.clear;
                if (i < balls.Count)
                {
                    BallType ballType = balls[i];

                    switch (ballType)
                    {
                        case BallType.Normal:
                            ballColor = Color.red;
                            break;
                    }
                }

                // Set ballColor
                ballRenderers[i].color = ballColor;
            }
        }
    }
}