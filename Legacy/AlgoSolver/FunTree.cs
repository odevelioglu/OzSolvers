using System;
using System.Collections.Generic;

namespace OzAlgo
{
    public class FunNode
    {
        public FunNode ParentNode;
        public List<FunNode> Childs = new List<FunNode>();
        
        public List<int> Args = new List<int>();
        public List<FunNode> SubCalls = new List<FunNode>();
        public bool IsSubCallRoot;
        
        public IFuncBase Function;
        
        public bool HasChild
        {
            get
            {
                return Childs.Count > 0;
            }
        }

        public bool IsRoot { get { return ParentNode == null; } }

        public void AddChild(FunNode child)
        {
            child.ParentNode = this;
            Childs.Add(child);
        }

        public void AddSubCall(FunNode call)
        {
            call.ParentNode = this;
            SubCalls.Add(call);
        }

        public override string ToString()
        {
            if (IsRoot)
                return "{}";

            var funcName = "f";
            if (Function != null)
                funcName = Function.FuncName;

            var ret = " ";
            foreach (var args in FunTree.VisitToTop(this))
            {
                ret += funcName + "(" + string.Join(",", args) + "); ";
            }

            return ret;
        }

        public int Depth
        {
            get
            {
                var curr = this;
                int ret = 0;

                do
                {
                    ret++;
                    curr = curr.ParentNode;
                } while (curr != null);

                return ret;
            }
        }
    }

    public class FunTree
    {
        // VisitToBottom right most
        public static void VisitToBottom(FunNode root, Action<FunNode> action)
        {
            if (root == null)
                return;

            action(root);

            foreach (var child in root.Childs)
            {
                VisitToBottom(child, action);
            }
        }

        public static int LeafCount(FunNode root)
        {
            int i = 0;

            VisitToBottom(root, node =>
            {
                if (!node.HasChild)
                    i++;
            });

            return i;
        }

        public static List<List<int>> VisitToTop(FunNode node)
        {
            var revList = new List<List<int>>();
            FunTree.VisitToTop(node, funNode =>
            {
                if (!funNode.IsRoot)
                    revList.Add(funNode.Args);
            });

            revList.Reverse();

            return revList;
        }

        public static List<FunNode> GetNodes(FunNode node)
        {
            var revList = new List<FunNode>();
            FunTree.VisitToTop(node, funNode =>
            {
                if (!funNode.IsRoot)
                    revList.Add(funNode);
            });

            revList.Reverse();

            return revList;
        }

        public static void VisitToTop(FunNode node, Action<FunNode> action)
        {
            if (node == null)
                return;

            action(node);

            VisitToTop(node.ParentNode, action);
        }

        public static FunNode GetRoot(FunNode node)
        {
            FunNode root = null;
            FunTree.VisitToTop(node, funNode =>
            {
                if (funNode.IsRoot)
                    root = funNode;
            });

            return root;
        }

        public static List<FunNode> GetLeafList(FunNode root)
        {
            var ret = new List<FunNode>();

            VisitToBottom(root, node =>
            {
                if (!node.HasChild)
                    ret.Add(node);
            });

            return ret;
        }
    }
}
