using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourBot
{
    /// <summary>
    /// Heap without changeKey, makes no demands on objects it stores
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class AnonymousMinHeap<T>
    {
        MinHeap<MinHeapItem<T>> Heap;
        public AnonymousMinHeap()
        {
            Heap = new MinHeap<MinHeapItem<T>>();
        }

        public void Add(T newItem, float key)
        {
            Heap.Add(new MinHeapItem<T>(newItem,key));
        }

        public T ExtractMin()
        {
            MinHeapItem<T> output = Heap.ExtractMin();
            if (output != default(MinHeapItem<T>))
                return output.GetItem();
            else
                return default(T);
        }

        public int Count
        {
            get { return Heap.Count; }
        }

        private class MinHeapItem<Y> : IHeapItem
        {
            Y Subject;
            float Key;
            int Index;

            public MinHeapItem(Y subject, float key)
            {
                Subject = subject;
                Key = key;
            }

            public float GetKey()
            {
                return Key;
            }

            public void SetKey(float key)
            {
                Key = key;
            }

            public int GetIndex()
            {
                return Index;
            }

            public void SetIndex(int index)
            {
                Index = index;
            }

            public Y GetItem()
            {
                return Subject;
            }

        }
    }

    /// <summary>
    /// heap with changekey, stored objects must implement interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class MinHeap<T> where T : IHeapItem
    {
        List<T> Storage;

        public MinHeap()
        {
            Storage = new List<T>();
        }

        public void Add(T newItem)
        {
            Storage.Add(newItem);
            newItem.SetIndex(Storage.Count-1);
            BubbleUp(newItem);
        }

        public void ChangeKey(T item, float newKey)
        {
            if (newKey < item.GetKey())
            {
                BubbleUp(item);
            }
            else
            {
                if (newKey > item.GetKey())
                {
                    MinHeapify(item);
                }
            }
        }

        public T ExtractMin()
        {
            if (Storage.Count == 0)
                return default(T);
            T result = Storage[0];

            Storage[0] = Storage[Storage.Count - 1];
            Storage.RemoveAt(Storage.Count - 1);

            if (!IsEmpty)
            {
                Storage[0].SetIndex(0);
                MinHeapify(Storage[0]);
            }

            return result;
        }

        private void BubbleUp(T current)
        {
            T parent;

            while (current.GetIndex() != 0)
            {
                int parentindex = GetParent(current.GetIndex());
                parent = Storage[GetParent(current.GetIndex())];

                if (parent.GetKey() > current.GetKey())
                {
                    Exchange(parent, current);
                }
                else
                {
                    break;
                }
            }
        }

        private void Exchange (T parent, T child)
        {
            int parentIndex = parent.GetIndex();
            int childIndex = child.GetIndex();

            child.SetIndex(parentIndex);
            parent.SetIndex(childIndex);

            Storage[childIndex] = parent;
            Storage[parentIndex] = child;
        }

        private void MinHeapify(T current)
        {
            T smallest = default(T);
            T left = default(T);
            T right = default(T);

            int leftIndex = GetLeft(current.GetIndex());
            int rightIndex = GetRight(current.GetIndex());

            if (leftIndex < Storage.Count)
            {
                left = Storage[leftIndex];
                if (left.GetKey() < current.GetKey())
                {
                    smallest = left;
                }
                else
                {
                    smallest = current;
                }

            }
            else
            {
                smallest = current;
            }

            if (rightIndex < Storage.Count)
            {
                right = Storage[rightIndex];
                if (right.GetKey() < smallest.GetKey())
                {
                    smallest = right;
                }
            }

            if (smallest.GetIndex() != current.GetIndex())
            {
                Exchange(current, smallest);
                MinHeapify(current);
            }

        }

        private int GetParent(int index)
        {
            return (index-1) / 2;
        }

        private int GetLeft(int index)
        {
            return (2 * index) + 1;
        }

        private int GetRight(int index)
        {
            return (2 * index) + 2;
        }

        public bool IsEmpty
        {
            get { return Storage.Count <= 0; }
        }

        public int Count
        {
            get { return Storage.Count; }
        }
    }

    public interface IHeapItem
    {
        float GetKey();
        int GetIndex();
        void SetKey(float key);
        void SetIndex(int index);
    }
}
