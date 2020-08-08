using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCBenchmarkCLR {
    public class WithGC {
        public int height;
        public int sum = 0;
        public Tree theTree = null;

        public WithGC(int _height) {
            height = _height;
        }

        Tree createTree(int[] payload) {
            if (height <= 0) return null;

            var stack = new Stack<Tree>();
            var wholeTree = createLeftTree(height, payload, stack);
            while (stack.Any()) {
                var bottomElement = stack.Peek();
                if (bottomElement.right != null || stack.Count() == height) {
                    stack.Pop();
                    while (stack.Any() && stack.Peek().right != null) stack.Pop();
                }
                if (stack.Any() && stack.Count() < height) {
                    bottomElement = stack.Peek();
                    bottomElement.right = createLeftTree(height - stack.Count(), payload, stack);
                }
            }
            return wholeTree;
        }


        // Populate the tree. Allocates lots of objects for the GC to waste time on.
        public Tree createLeftTree(int height, int[] payload, Stack<Tree> stack) {
            if (height == 0) return null;

            var newArr = new int[4];
            Array.Copy(payload, newArr, 4);
            var wholeTree = new Tree { payload = newArr };
            var currTree = wholeTree;
            stack.Push(wholeTree);
            for (int i = 1; i < height; ++i) {
                newArr = new int[4];
                Array.Copy(payload, newArr, 4);
                var newTree = new Tree { payload = newArr };
                currTree.left = newTree;
                currTree = newTree;
                stack.Push(currTree);
            }
            return wholeTree;
        }


        public int processTree() {
            if (theTree == null) {
                Console.WriteLine("Oh noes, the tree is null!");
                return -1;
            } else {
                var stack = new Stack<Tree>();
                processLeftTree(theTree, stack);
                while (stack.Any()) {
                    var bottomElem = stack.Pop().right;
                    if (bottomElem != null) processLeftTree(bottomElem, stack);
                }
            }
            return sum;
        }


        public void processLeftTree(Tree tree, Stack<Tree> stack) {
            Tree currElem = tree;
            if (currElem != null) {
                stack.Push(currElem);
                for (int i = 0; i < currElem.payload.Length; ++i) {
                    sum += currElem.payload[i];
                }
                while (currElem?.left != null) {
                    currElem = currElem.left;
                    if (currElem != null) {
                        for (int i = 0; i < currElem.payload.Length; ++i) {
                            sum += currElem.payload[i];
                        }
                        stack.Push(currElem);
                    }
                }
            }
        }


        public class Tree {
            public Tree left = null;
            public Tree right = null;
            public int[] payload;
        }
    }
}
