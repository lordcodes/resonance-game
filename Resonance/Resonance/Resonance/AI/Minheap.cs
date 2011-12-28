using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance
{
    class Minheap
    {
        private List<Node> list;
        private int count;

        public Minheap()
        {
            list = new List<Node>();
            count = 0;
        }

        /// <summary>
        /// Get the number of elements on the heap
        /// </summary>
        public int Count
        {
            get
            {
                return count;
                //return list.Count;
            }
        }

        public bool contains(int x, int z)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].X == x && list[i].Z == z) return true;
            }
            return false;
        }

        public int accessElementGCost(int x, int z)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].X == x && list[i].Z == z)
                {
                    return list[i].G;
                }
            }
            return -1;
        }

        public void setElementCosts(int x, int z, int g)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].X == x && list[i].Z == z)
                {
                    list[i].F = g + list[i].H;
                    list[i].G = g;
                    heapUp(i);
                    heapDown(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Extracts the root from the heap
        /// </summary>
        /// <returns>null if error, else the root node</returns>
        public Node extractRoot()
        {
            if (count == 0) return null;
            else
            {
                Node rootNode = list[0];
                switchElements(0, count - 1);
                count--;
                heapDown(0);

                return rootNode;
            }
            /*Node rootNode = list[0];
            list.RemoveAt(0);
            return rootNode;*/
        }

        /// <summary>
        /// Peek at the root of the heap
        /// </summary>
        /// <returns>null if error, else the root node</returns>
        public Node accessRoot()
        {
            if (count == 0) return null;
            else return list[0];
        }

        /// <summary>
        /// Add a node to the heap
        /// </summary>
        /// <param name="node">The node to add</param>
        public void add(Node node)
        {
            if (count >= list.Count) list.Add(node);
            else list[count] = node;
            count++;
            heapUp(count - 1);
            /*if (list.Count == 0) list.Add(node);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].F > node.F) list.Insert(i, node);
            }*/
        }

        //PRIVATE METHODS

        /// <summary>
        /// Re-sort the heap
        /// </summary>
        private void heapify()
        {
            for (int i = parentIndex(count - 1); i >= 0; i--)
            {
                heapDown(i);
            }
        }

        private void heapUp(int index)
        {
            Node elem = list[index];
            while (true)
            {
                int parentInd = parentIndex(index);
                if (parentInd < 0 || list[parentInd].F < elem.F) break;
                switchElements(index, parentInd);
                index = parentInd;
                
                break;
            }
        }

        private void heapDown(int index)
        {
            while (true)
            {
                int left = lChildIndex(index);
                if (left < 0) break;
                int right = rChildIndex(index);

                int child;
                if (right < 0) child = left;
                else
                {
                    if (left >= list.Count || right >= list.Count) break;
                    if (list[left].F < list[right].F) child = left;
                    else child = right;
                }
                if (list[child].F > list[index].F) break;
                switchElements(index, child);
                index = child;
            }
        }


        /// <summary>
        /// Returns the index of the parent
        /// </summary>
        /// <param name="index">the index you are at</param>
        /// <returns>int - index of parent</returns>
        private int parentIndex(int index)
        {
            if (count == 0) return -1;
            else return (index - 1) / 2;
        }

        /// <summary>
        /// Returns the index of the left child
        /// </summary>
        /// <param name="index">the index you are at</param>
        /// <returns>int - index of left child</returns>
        private int lChildIndex(int index)
        {
            if (count == 0) return -1;
            else return 2 * index + 1;
        }

        /// <summary>
        /// Returns the index of the right child
        /// </summary>
        /// <param name="index">the index you are at</param>
        /// <returns>int - index of right child</returns>
        private int rChildIndex(int index)
        {
            if (count == 0) return -1;
            else return 2 * index + 2;
        }

        /// <summary>
        /// Swap elements around in the heap
        /// </summary>
        /// <param name="first">first element</param>
        /// <param name="second">second element</param>
        private void switchElements(int first, int second)
        {
            Node temp = list[first];
            list[first] = list[second];
            list[second] = temp;
        }
    }
}
