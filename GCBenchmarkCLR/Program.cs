using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCBenchmarkCLR {
    class Program {
        static void Main(string[] args) {
        }

        public static void run(int height, string designator, Func<int, DateTime, int> coreFun) {
            Console.WriteLine("Processing tree with $designator...");
            var timeStart = DateTime.Now;

            var result = coreFun(height, timeStart);

            var timeEnd = DateTime.Now;

            Console.WriteLine("Finished with result = " + result);
            Console.WriteLine("Used time = " + (timeEnd - timeStart).TotalMilliseconds + " ms");
        }


        public static int runWithGC(int height, DateTime tStart) {
            var withGC = new WithGC(height);


            var memory = GC.GetTotalMemory(false);
            Console.WriteLine($"Used memory = {(memory / 1024L / 1024L)} MB");
            Console.WriteLine($"Time for alloc = {(DateTime.Now - tStart).TotalMilliseconds} s");

            return withGC.processTree();
        }


        public static int runWithRegions(int height, DateTime tStart) {
            var withRegion = new WithRegions(height);

            var runtime = Runtime.getRuntime();
            long memory = runtime.totalMemory() - runtime.freeMemory();
            Console.WriteLine("Used memory = " + (memory / 1024L / 1024L) + " MB");
            Console.WriteLine("Time for alloc = " + getDateDiff(tStart, Date.from(Instant.now()), TimeUnit.SECONDS) + " s");

            return withRegion.processTree();


        }
    }
