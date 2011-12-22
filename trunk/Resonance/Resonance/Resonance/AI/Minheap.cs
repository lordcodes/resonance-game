﻿using System;
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

        public int Count
        {
            get
            {
                return count;
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

                return rootNode;
            }
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
                
                break;
            }
        }

        private void heapDown(int index)
        {
            while (true)
            {
                break;
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
