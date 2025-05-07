using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senior_Project
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private readonly List<T> heap;

        public PriorityQueue()
        {
            heap = new List<T>();
        }

       
        public void Enqueue(T item)
        {
            heap.Add(item);
            HeapifyUp(heap.Count - 1);
        }

      
        public T Dequeue()
        {
            if (heap.Count == 0) throw new InvalidOperationException("Priority queue is empty.");

            T root = heap[0];
            heap[0] = heap[^1];
            heap.RemoveAt(heap.Count - 1);
            HeapifyDown(0);

            return root;
        }

       
        public T Peek()
        {
            if (heap.Count == 0) throw new InvalidOperationException("Priority queue is empty.");
            return heap[0];
        }

        public int Count => heap.Count;

        private void HeapifyUp(int index)
        {
            while (index > 0)
            { int parent = (index - 1) / 2;
                if (heap[index].CompareTo(heap[parent]) >= 0) break;
                Swap(index, parent);
                index = parent;}
        }
        private void HeapifyDown(int index)
        {
            int lastIndex = heap.Count - 1;
            while (index < heap.Count)
            {int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int smallest = index;
                if (leftChild <= lastIndex && heap[leftChild].CompareTo(heap[smallest]) < 0)
                    smallest = leftChild;
                if (rightChild <= lastIndex && heap[rightChild].CompareTo(heap[smallest]) < 0)
                    smallest = rightChild;
                if (smallest == index) break;
                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            (heap[i], heap[j]) = (heap[j], heap[i]);
        }
    }
}