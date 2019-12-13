
using System;

namespace Voice
{
    class Program
    {
        static void Main(string[] args)
        {
            var command = new ReadCommand(args);
        #if DEBUG
            Console.ReadKey();
        #endif
        }
    }
}
