using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Heap<T> where T : IHeapItem<T>
    {
        private T[] items;
        private int currentItemCount;

        /// <summary>
        /// Creates the heap.
        /// </summary>
        /// <param name="heapSize">The heap's size</param>
        public Heap(int heapSize)
        {
            items = new T[heapSize];
        }

        public int Count
        {
            get
            {
                return currentItemCount;
            }
        }

        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            ++currentItemCount;
        }

        public T RemoveFirst()
        {
            T first = items[0];
            --currentItemCount;
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return first;
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }

        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        private void SortDown(T item)
        {
            while (true)
            {
                int leftChild = (item.HeapIndex * 2) + 1;
                int rightChild = (item.HeapIndex * 2) + 2;

                int swapIndex = 0;

                if (leftChild < currentItemCount)
                {
                    swapIndex = leftChild;
                    if (rightChild < currentItemCount)
                    {
                        if (items[leftChild].CompareTo(items[rightChild]) < 0)
                        {
                            swapIndex = rightChild;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void Swap(T item1, T item2)
        {
            items[item1.HeapIndex] = item2;
            items[item2.HeapIndex] = item1;

            int temp = item1.HeapIndex;
            item1.HeapIndex = item2.HeapIndex;
            item2.HeapIndex = temp;
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}
