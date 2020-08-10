using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCBenchmarkCLR {
    public class WithRegions {
        public int height;
        public const int ELTS_IN_REGION = 600000;
        public const int SIZE_PAYLOAD = 4;
        public const int SIZE_REGION = (SIZE_PAYLOAD + 2) * ELTS_IN_REGION;
        List<Payload[]> regions;
        int currRegion;
        int indFree;
        public int sum;

        public WithRegions(int _height) {
            height = _height;
            var numRegions = (Math.pow(2.0, this.height.toDouble()) - 1).toInt() / ELTS_IN_REGION + 1;
            regions = new List<Payload[]>(numRegions);
            for (int i = 0; i < numRegions; ++i) {
                //regions.add(IntArray(SIZE_REGION) { _ -> 0})
                regions.Add(new Payload[SIZE_REGION]);
            }
            Console.WriteLine("Time for region alloc = " + getDateDiff(tStart, Date.from(Instant.now()), TimeUnit.MILLISECONDS) + " ms")

            currRegion = 0;
            indFree = 0;

            createTree(intArrayOf(1, 2, -1, -1));
            sum = 0;
        }

        public int processTree() {
            return -1;
        }

        public struct Payload {
        int a;
        int b;
        int c;
        int d;
    }
    }
}
