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
        public const int SIZE_REGION = ELTS_IN_REGION;
        List<Node[]> regions;
        int currRegion;
        int indFree;
        public int sum;

        public WithRegions(int _height, DateTime tStart) {
            height = _height;
            var numRegions = (int)(Math.Pow(2.0, (double)this.height) - 1) / ELTS_IN_REGION + 1;
            regions = new List<Node[]>(numRegions);
            for (int i = 0; i < numRegions; ++i) {
                regions.Add(new Node[SIZE_REGION]);
            }
            Console.WriteLine($"Time for region alloc = {(DateTime.Now - tStart).TotalMilliseconds} ms");

            currRegion = 0;
            indFree = 0;

            createTree(1, 2, -1, -1);
            sum = 0;
        }

        public void createTree(int _a, int _b, int _c, int _d) {
            if (height <= 0) return;
            var stack = new Stack<Loc>();
            var wholeTree = createLeftTree(height, _a, _b, _c, _d, stack);
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
                    bottomElement.arr[bottomElement.ind].right = createLeftTree(height - stack.Count(), _a, _b, _c, _d, stack);
                }
            }
        }

        public int createLeftTree(int height, int _a, int _b, int _c, int _d, Stack<Loc> stack) {
            if (height == 0) return -1;

            var wholeTree = allocateNode(_a, _b, _c, _d);
            Loc currTree = toLoc(wholeTree);
            stack.Push(currTree);
            for (int i = 1; i < height; ++i) {
                var newTree = allocateNode(_a, _b, _c, _d);
                currTree.arr[currTree.ind].left = newTree;
                currTree = toLoc(newTree);
                stack.Push(currTree);
            }
            return wholeTree;
        }


        public int processTree() {
            if (indFree == 0) {
                Console.WriteLine("Oh noes, the tree is null!");
                return -1;
            } else {
                var stack = new Stack<Loc>();
                processLeftTree(toLoc(0), stack);
                while (stack.Any()) {
                    var bottomElem = stack.Pop();
                    var indRight = bottomElem.arr[bottomElem.ind].right;
                    if (indRight > -1) processLeftTree(toLoc(indRight), stack);
                }
            }
            return sum;
        }

        public void processLeftTree(Loc root, Stack<Loc> stack) {
            stack.Push(root);

            sum += root.arr[root.ind].a;
            sum += root.arr[root.ind].b;
            sum += root.arr[root.ind].c;
            sum += root.arr[root.ind].d;

            var currLeft = root.arr[root.ind].left;
            while (currLeft > -1) {
                var currNode = toLoc(currLeft);

                sum += currNode.arr[root.ind].a;
                sum += currNode.arr[root.ind].b;
                sum += currNode.arr[root.ind].c;
                sum += currNode.arr[root.ind].d;
                stack.Push(currNode);
                currLeft = currNode.arr[currNode.ind].left;
            }
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
            var result = currRegion * SIZE_REGION + indFree;
            region[indFree].left = -1;
            region[indFree].right = -1;
            region[indFree].a = _a;
            region[indFree].b = _b;
            region[indFree].c = _c;
            region[indFree].d = _d;
            ++indFree;
            return result;
        }

        public Loc toLoc(int ind) {
            var numRegion = ind / SIZE_REGION;
            var offset = ind % SIZE_REGION;
            return new Loc { arr = regions[numRegion], ind = offset };
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
