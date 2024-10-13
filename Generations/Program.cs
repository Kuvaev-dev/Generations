using System;
using System.Collections.Generic;

namespace Generations
{
    // Задача
    // Нехай є військо противника, розташоване нерівномірно на певній території.Є 10 бойових     дронів. При вибуху в деякому радіусі не залишається нічого живого. Знайти такі координати для кожного дрону так, щоб після вибухів у противника залишалось якомога менше військ. Радіус дії вводиться з клавіатури.Противник і його кількість генеруються випадково. На екрані - початкова картинка і картинка після вибуху.
    internal class Program
    {
        private static Random _rnd = new Random();
        const int GenerationSize = 100;
        const int GenerationNumbers = 200;
        const double MutationProbability = 0.2;
        const int NumberOfDrones = 10;
        static double Radius;

        static List<(double, double)> enemies = new List<(double, double)>();

        static void Main(string[] args)
        {
            Console.Write("Введіть радіус дії дронів: ");
            Radius = Convert.ToDouble(Console.ReadLine());

            int enemyCount = _rnd.Next(30, 100);
            GenerateEnemies(enemyCount);

            List<KeyValuePair<int[], double>> generation = GenerateRandom();
            SortGeneration(generation);

            for (int getNum = 0; getNum < GenerationNumbers; getNum++)
            {
                generation = GenerateNewGeneration(generation, true);
                SortGeneration(generation);

                Console.WriteLine($"Краща особа: {generation[0].Value}");
                Console.WriteLine($"Generation: {getNum}");
            }

            int[] bestGenome = generation[0].Key;
            Console.WriteLine("Координати дронів:");
            for (int i = 0; i < NumberOfDrones; i++)
            {
                Console.WriteLine($"Дрон {i + 1}: x = {GetX(bestGenome[i])}, y = {GetY(bestGenome[i])}");
            }

            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }

        private static void GenerateEnemies(int count)
        {
            enemies.Clear();
            for (int i = 0; i < count; i++)
            {
                double x = _rnd.NextDouble() * 100;
                double y = _rnd.NextDouble() * 100;
                enemies.Add((x, y));
            }
        }

        private static List<KeyValuePair<int[], double>> GenerateRandom()
        {
            List<KeyValuePair<int[], double>> result = new List<KeyValuePair<int[], double>>();
            for (int i = 0; i < GenerationSize; i++)
            {
                int[] genome = new int[NumberOfDrones];
                for (int j = 0; j < NumberOfDrones; j++)
                {
                    genome[j] = _rnd.Next();
                }
                result.Add(new KeyValuePair<int[], double>(genome, Fitness(genome)));
            }
            return result;
        }

        private static void SortGeneration(List<KeyValuePair<int[], double>> generation)
        {
            generation.Sort((x, y) => y.Value.CompareTo(x.Value));
            if (generation.Count > GenerationSize)
                generation.RemoveRange(GenerationSize, generation.Count - GenerationSize);
        }

        private static double GetY(int genome)
        {
            int y = genome & 0xffff;
            return y * 100.0 / 0x10000;
        }

        private static double GetX(int genome)
        {
            int x = (genome >> 16) & 0xffff;
            return x * 100.0 / 0x10000;
        }

        private static double Fitness(int[] genome)
        {
            HashSet<int> destroyedEnemies = new HashSet<int>();

            for (int i = 0; i < NumberOfDrones; i++)
            {
                double droneX = GetX(genome[i]);
                double droneY = GetY(genome[i]);

                for (int j = 0; j < enemies.Count; j++)
                {
                    double enemyX = enemies[j].Item1;
                    double enemyY = enemies[j].Item2;

                    if (Math.Sqrt(Math.Pow(droneX - enemyX, 2) + Math.Pow(droneY - enemyY, 2)) <= Radius)
                    {
                        destroyedEnemies.Add(j);
                    }
                }
            }

            return destroyedEnemies.Count;
        }

        private static List<KeyValuePair<int[], double>> GenerateNewGeneration(List<KeyValuePair<int[], double>> parents, bool useElitism)
        {
            List<KeyValuePair<int[], double>> result = new List<KeyValuePair<int[], double>>();

            if (useElitism)
                result.Add(parents[0]);

            while (result.Count < GenerationSize)
            {
                int parent1 = _rnd.Next(GenerationSize);
                int parent2 = _rnd.Next(GenerationSize);

                int[] child = new int[NumberOfDrones];
                for (int i = 0; i < NumberOfDrones; i++)
                {
                    int mask = ~0 << _rnd.Next(32);
                    child[i] = parents[parent1].Key[i] & mask | parents[parent2].Key[i] & ~mask;

                    if (_rnd.NextDouble() < MutationProbability)
                        child[i] ^= 1 << _rnd.Next(32);
                }

                result.Add(new KeyValuePair<int[], double>(child, Fitness(child)));
            }

            return result;
        }
    }
}
