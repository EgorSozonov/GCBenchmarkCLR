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
        List<Node[]> regions;
        int currRegion;
        int indFree;
        public int sum;

        public WithRegions(int _height, DateTime tStart) {
            height = _height;
            var numRegions = (int)(Math.Pow(2.0, (double)this.height) - 1) / ELTS_IN_REGION + 1;
            regions = new List<Node[]>(numRegions);
            for (int i = 0; i < numRegions; ++i) {
                //regions.add(IntArray(SIZE_REGION) { _ -> 0})
                regions.Add(new Node[SIZE_REGION]);
            }
            Console.WriteLine($"Time for region alloc = {(DateTime.Now - tStart).TotalMilliseconds} ms");

            currRegion = 0;
            indFree = 0;

            createTree(new Node { a = 1, b = 2, c = -1, d = -1 });
            sum = 0;
        }

        public void createTree(Node payload) {
            if (height <= 0) return;
            var stack = new Stack<Loc>();
            var wholeTree = createLeftTree(height, payload, stack);
            while (stack.Any()) {
                var bottomElement = stack.Peek();
                if (bottomElement.arr[bottomElement.ind].right > -1 || stack.Count() == height) {
                    stack.Pop();
                    while (stack.Any()) {
                        bottomElement = stack.Peek();
                        if (bottomElement.arr[bottomElement.ind].right == -1) break;
                        stack.Pop();
                    }
                }
                if (stack.Any()) {
                    bottomElement = stack.Peek();
                    bottomElement.arr[bottomElement.ind + 1] = createLeftTree(height - stack.Count(), payload, stack);
                }
            }
        }

        public int processTree() {
            return -1;
        }

        public int allocateNode(int _a, int _b, int _c, int _d) {
            if (indFree == SIZE_REGION) {
                ++currRegion;
                indFree = 0;
                if (currRegion == regions.Count) {
                    regions.Add(new Node[SIZE_REGION]);
                }
            }

            var region = regions[currRegion];
            //val result = Loc(region, indFree)
            var result = currRegion * SIZE_REGION + indFree;
            region[indFree].left = -1;
            region[indFree].right = -1;
            region[indFree].a = _a;
            region[indFree].b = _b;
            region[indFree].c = _c;
            region[indFree].d = _d;
            return result;
        }

        public struct Node {
            public int left;
            public int right;
            public int a;
            public int b;
            public int c;
            public int d;
        }

        public class Loc {
            public Node[] arr;
            public int ind;
        }
    }
}
