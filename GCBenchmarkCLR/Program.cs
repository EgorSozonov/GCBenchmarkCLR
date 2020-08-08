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
            Console.WriteLine("Used time = " + getDateDiff(timeStart, timeEnd, TimeUnit.SECONDS) + " s");
        }


        public static int runWithGC(int height, DateTime tStart) {
            var withGC = new WithGC(height);


            //val memory: Long = runtime.totalMemory() - runtime.freeMemory()
            Console.WriteLine("Used memory = " + (memory / 1024L / 1024L) + " MB");
            Console.WriteLine("Time for alloc = " + getDateDiff(tStart, Date.from(Instant.now()), TimeUnit.SECONDS) + " s");

            return withGC.processTree();
        }


        public static int runWithRegions(int height, DateTime tStart) {
            var withRegion = new WithRegions(height);

            var runtime = Runtime.getRuntime();
            var memory: Long = runtime.totalMemory() - runtime.freeMemory();
            Console.WriteLine("Used memory = " + (memory / 1024L / 1024L) + " MB");
            Console.WriteLine("Time for alloc = " + getDateDiff(tStart, Date.from(Instant.now()), TimeUnit.SECONDS) + " s");

            return withRegion.processTree();


        }
    }
