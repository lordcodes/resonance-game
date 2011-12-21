using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance
{
    class Minheap
    {
        private Node head;

        public Minheap() { }

        public void add(Node node)
        {
            if (head == null) head = node;
            else if (head.After == null && node.Cost <= head.Cost)
            {
                node.NextElement = head;
                head = node;
            }
            else
            {
                Node newHead = head;
                while (newHead.NextElement != null && newHead.NextElement.Cost < node.Cost) newHead = newHead.NextElement;
                node.NextElement = newHead.NextElement;
                newHead.NextElement = node;
            }
        }

        public bool hasNext()
        {
            if (head != null) return true;
            else return false;
        }

        public Node extractHead()
        {
            Node value = head;
            head = head.NextElement;
            return value;
        }
    }
}
