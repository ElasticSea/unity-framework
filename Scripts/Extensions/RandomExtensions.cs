using System;

namespace ElasticSea.Framework.Scripts.Extensions
{
    public static class RandomExtensions
    {
        public static bool Probability(this Random random, float probability) => random.NextDouble() <= probability;
        public static bool FlipCoin(this Random random) => random.NextDouble() >= 0.5;
        public static int RollDice(this Random random, int sides) => random.Next(1, sides);
    }
}