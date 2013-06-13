using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csharp_conways_game_of_life
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Write("Cells count: ");

            var cellsCount = int.Parse(Console.ReadLine());
            var history = Universe.FromNothing(cellsCount).BigBang();

            Console.ReadKey();
        }
    }

    public class Universe : HashSet<Tuple<int,int>>
    {
        public static int HorizontalHorizont = 80;
        public static int VerticalHorizont = 25;

        public Universe(IEnumerable<Tuple<int, int>> cells) : base(cells)
        {
        }

        public static Universe FromNothing(int count)
        {
            var random = new Random();
            return new Universe(Enumerable.Range(0, count).Select(_ => Tuple.Create(random.Next(0, HorizontalHorizont), random.Next(0, VerticalHorizont))));
        }

        public Universe Evolve()
        {
            var set = this
                .SelectMany(t => Enumerable.Range(0, 9).Select(i => Tuple.Create(t.Item1 + i / 3 - 1, t.Item2 + i % 3 - 1)))
                .Distinct()
                .Select(t => Tuple.Create(
                                t.Item1,
                                t.Item2,
                                this.Contains(t),
                                this.Count(c => Math.Abs(t.Item1 - c.Item1) <= 1 && Math.Abs(t.Item2 - c.Item2) <= 1) - (this.Contains(t) ? 1 : 0)))
                .Where(state => state.Item3
                    ? state.Item4 == 2 || state.Item4 == 3
                    : state.Item4 == 3)
                .Where(state => state.Item1 >= 0 && state.Item2 >= 0 && state.Item1 < HorizontalHorizont && state.Item2 < VerticalHorizont)
                .Select(state => Tuple.Create(state.Item1, state.Item2));
            return new Universe(set);
        }
    }

    public static class UniverseExtensions
    {
        public static TimeSpan GenerationLifespan = TimeSpan.FromMilliseconds(200);
        
        public static Universe Print(this Universe universe)
        {
            Console.Clear();
            universe.ToList().ForEach(t =>
                                          {
                                              Console.SetCursorPosition(t.Item1, t.Item2);
                                              Console.Write('■');
                                          });
            return universe;
        }

        public static async Task BigBang(this Universe universe)
        {
            await Task.Delay(GenerationLifespan);
            await universe.Print().Evolve().BigBang();
        }
    }
}
