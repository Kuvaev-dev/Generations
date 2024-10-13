using System;
using System.Collections.Generic;

namespace Generations
{
    internal class Program
    {
        private static Random _rnd = new Random();
        const int GenerationSize = 10000;
        const int GenerationNumbers = 200;
        const double MutationProability = 0.2; 

        static void Main(string[] args)
        {
            // Ініціалізація нульового покоління
            List<KeyValuePair<int, double>> generation = GenerateRandom();
            SortGeneration(generation);

            // Основний цикл генерації
            for (int getNum = 0; getNum < GenerationNumbers; getNum++)
            {
                generation = GenerateNewGeneration(generation, true);
                SortGeneration(generation);

                Console.WriteLine($"Краща особа: {Weight(generation[0].Key)}");
                Console.WriteLine($"Generation: {getNum}");
            }

            Console.WriteLine($"x = {GetX(generation[0].Key)}");
            Console.WriteLine($"y = {GetY(generation[0].Key)}");
            Console.WriteLine("Genome = {0:X}", generation[0].Key);

            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }

        // Генерація першого випадкового покоління
        private static List<KeyValuePair<int, double>> GenerateRandom()
        {
            List<KeyValuePair<int, double>> result = new List<KeyValuePair<int, double>>();
            for (int i = 0; i < GenerationSize * 2; i++)
            {
                int genome = _rnd.Next();
                result.Add(new KeyValuePair<int, double>(genome, Weight(genome)));
            }
            return result;
        }

        // Початкова відбраковка
        private static void SortGeneration(List<KeyValuePair<int, double>> generation)
        {
            generation.Sort((x, y) => y.Value.CompareTo(x.Value));
            if (generation.Count > GenerationSize)
                generation.RemoveRange(GenerationSize, generation.Count - GenerationSize);
        }

        // Обчислення фітнес-функції
        private static double GetY(int genome)
        {
            int y = genome & 0xffff;
            return y * (1.28 + 1.28) / (0x10000 - 1.0) - 1.28;
        }

        private static double GetX(int genome)
        {
            int x = (genome >> 16) & 0xffff;
            return x * (1.28 + 1.28) / (0x10000 - 1.0) - 1.28;
        }

        private static double Sqr(double x)
        {
            return x * x;
        }

        private static double Weight(int genome)
        {
            double x = GetX(genome);
            double y = GetY(genome);
            return 100 / (100 * Sqr(Sqr(x) - y) + Sqr(1 - x) + 1);
        }

        // Процедура генерації нового покоління
        private static List<KeyValuePair<int, double>> GenerateNewGeneration(List<KeyValuePair<int, double>> parents, bool useElitism)
        {
            List<KeyValuePair<int, double>> result = new List<KeyValuePair<int, double>>();

            // Реалізація єлітизму
            if (useElitism)
                result.Add(parents[0]);

            while (result.Count < GenerationSize * 2)
            {
                // Турнірна вибірка попередників
                int parent1a = _rnd.Next(GenerationSize);
                int parent1b = _rnd.Next(GenerationSize);
                int parent2a = _rnd.Next(GenerationSize);
                int parent2b = _rnd.Next(GenerationSize);
                int parent1 = Math.Min(parent1a, parent1b);
                int parent2 = Math.Min(parent2a, parent2b);

                // Генерація з кросовером
                int mask = ~0 << _rnd.Next(32);
                int child = parents[parent1].Key & mask | parents[parent2].Key & ~mask;

                // Мутація
                if (_rnd.NextDouble() > MutationProability)
                    child ^= 1 << _rnd.Next(32);
                result.Add(new KeyValuePair<int, double> (child, Weight(child)));
            }

            return result;
        }
    }
}
