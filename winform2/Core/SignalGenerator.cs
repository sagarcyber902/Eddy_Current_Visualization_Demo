using System;
using System.Collections.Generic;
using System.Text;
using winform2.Model;

namespace winform2.Core
{
    public class SignalGenerator
    {
        private readonly Random rand = new();
        private float currentX, currentY;

        public Sample Generate()
        {
            int roll = rand.Next(1000);

            if (roll < 999)
            {
                currentX += (float)(rand.NextDouble() * 4 - 2);
                currentY += (float)(rand.NextDouble() * 4 - 2);

                currentX *= 0.95f;
                currentY *= 0.95f;
            }
            else
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float mag = (float)(rand.NextDouble() * 120 + 60);

                currentX += MathF.Cos(angle) * mag;
                currentY += MathF.Sin(angle) * mag;
            }

            currentX = Math.Clamp(currentX, -200, 200);
            currentY = Math.Clamp(currentY, -200, 200);

            double z = Math.Sqrt(currentX * currentX + currentY * currentY);

            return new Sample { X = currentX, Y = currentY, Z = z };
        }

        public void Reset()
        {
            currentX = currentY = 0;
        }
    }
}
