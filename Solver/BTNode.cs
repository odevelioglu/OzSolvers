using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver
{
    public class BTNode<T>
    {
        protected T element;

        public BTNode<T> left;
        public BTNode<T> right;

        public BTNode(T elem)
        {
            element = elem;
            left = right = null;
        }

        public BTNode(T elem, BTNode<T> left, BTNode<T> right)
        {
            element = elem;

            this.left = this.right = null;

            if ((left != null && left.getElement() != null)) this.left = new BTNode<T>(left);
            if ((right != null && right.getElement() != null)) this.right = new BTNode<T>(right);
        }

        // copy
        public BTNode(BTNode<T> node)
            : this(node.getElement(), node.getLeft(), node.getRight())
        {

        }

        //

        public T getElement()
        {
            return element;
        }

        public BTNode<T> getLeft()
        {
            return left;
        }

        public BTNode<T> getRight()
        {
            return right;
        }

        public void setElement(T val)
        {
            element = val;
        }

        public void setLeft(BTNode<T> val)
        {
            left = val;
        }

        public void setRight(BTNode<T> val)
        {
            right = val;
        }

        public void setValueAt(int index, T val)
        {
            if (index == 0) element = val;
            else
            {
                if ((left != null && left.getElement() != null))
                {
                    left.setValueAt(index - 1, val);

                    if ((right != null && right.getElement() != null)) right.setValueAt(index - 1 - left.count(), val);
                }
                else if ((right != null && right.getElement() != null))
                {
                    right.setValueAt(index - 1, val);
                }
            }
        }

        public void setTreeAt(int index, BTNode<T> val)
        {
            if (index == 0)
            {
                element = val.getElement();

                if (val.getLeft() != null) left = new BTNode<T>(val.getLeft());
                else left = null;

                if (val.getRight() != null) right = new BTNode<T>(val.getRight());
                else right = null;
            }
            else
            {
                if ((left != null && left.getElement() != null))
                {
                    left.setTreeAt(index - 1, val);

                    if ((right != null && right.getElement() != null)) right.setTreeAt(index - 1 - left.count(), val);
                }
                else if ((right != null && right.getElement() != null))
                {
                    right.setTreeAt(index - 1, val);
                }
            }
        }

        //
        public int count()
        {
            int result = 1;

            result += ((left != null && left.getElement() != null) ? left.count() : 0);
            result += ((right != null && right.getElement() != null) ? right.count() : 0);

            return result;
        }

        // Root Left Right
        public void preOrder(List<T> arr)
        {
            arr.Add(element);

            if ((left != null && left.getElement() != null)) left.preOrder(arr);

            if ((right != null && right.getElement() != null)) right.preOrder(arr);
        }

        public void preOrderTree(List<BTNode<T>> arr)
        {
            arr.Add(new BTNode<T>(this));

            if ((left != null && left.getElement() != null)) left.preOrderTree(arr);

            if ((right != null && right.getElement() != null)) right.preOrderTree(arr);
        }

        // Left Root Right
        public void inOrder(List<T> arr)
        {
            if ((left != null && left.getElement() != null)) left.inOrder(arr);

            arr.Add(element);

            if ((right != null && right.getElement() != null)) right.inOrder(arr);
        }

        public void inOrderTree(List<BTNode<T>> arr)
        {
            if ((left != null && left.getElement() != null)) left.inOrderTree(arr);

            arr.Add(new BTNode<T>(this));

            if ((right != null && right.getElement() != null)) right.inOrderTree(arr);
        }

        // Left Right Root
        public void postOrder(List<T> arr)
        {
            if ((left != null && left.getElement() != null)) left.postOrder(arr);

            if ((right != null && right.getElement() != null)) right.postOrder(arr);

            arr.Add(element);
        }

        public void postOrderTree(List<BTNode<T>> arr)
        {
            if ((left != null && left.getElement() != null)) left.postOrderTree(arr);

            if ((right != null && right.getElement() != null)) right.postOrderTree(arr);

            arr.Add(new BTNode<T>(this));
        }

    }
}
