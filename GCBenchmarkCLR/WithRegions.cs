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
        int indCurrRegion;
        Node[] currRegion;
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

            indCurrRegion = 0;
            currRegion = regions[0];
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
                ++indCurrRegion;
                indFree = 0;
                if (indCurrRegion == regions.Count) {
                    regions.Add(new Node[SIZE_REGION]);
                }
                currRegion = regions[indCurrRegion];
            }

            var nd = currRegion[indFree];
            //currRegion[indFree].left = -1;
            //currRegion[indFree].right = -1;
            //currRegion[indFree].a = _a;
            //currRegion[indFree].b = _b;
            //currRegion[indFree].c = _c;
            //currRegion[indFree].d = _d;
            //nd.left = -1;
            //nd.right = -1;
            //nd.a = _a;
            //nd.b = _b;
            //nd.c = _c;
            //nd.d = _d;
            //nd.left = -1;
            foo(ref currRegion[indFree], _a, _b, _c, _d);
            ++indFree;
            return indCurrRegion * SIZE_REGION + indFree - 1;
        }

        public static void foo(ref Node nd, int _a, int _b, int _c, int _d) {
            nd.left = -1;
            nd.right = -1;
            nd.a = _a;
            nd.b = _b;
            nd.c = _c;
            nd.d = _d;
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

        public sealed class Loc {
            public Node[] arr;
            public int ind;
        }
    }
}
