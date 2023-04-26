using System;
using System.Collections.Generic;

public static class ShortestPathFinder
{
    public static List<Tuple<int, int>> FindShortestPath(bool[,] graph, Tuple<int, int> start, Tuple<int, int> end)
    {
        if (graph == null ||
            start.Item1 < 0 || start.Item1 >= graph.GetLength(0) || end.Item1 < 0 || end.Item1 >= graph.GetLength(0) ||
            start.Item2 < 0 || start.Item2 >= graph.GetLength(1) || end.Item2 < 0 || end.Item2 >= graph.GetLength(1)
            )
        {
            return null;
        }

        int width = graph.GetLength(0);
        int height = graph.GetLength(1);

        int[,] distances = new int[width, height];
        bool[,] visited = new bool[width, height];
        Tuple<int, int>[,] previous = new Tuple<int, int>[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                distances[i, j] = int.MaxValue;
                visited[i, j] = false;
                previous[i, j] = null;
            }
        }

        distances[start.Item1, start.Item2] = 0;

        Tuple<int, int> current = null;
        while (!visited[end.Item1, end.Item2])
        {
            current = null;
            int shortestDistance = int.MaxValue;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Tuple<int, int> point = Tuple.Create(i, j);
                    if (!visited[i, j] && distances[i, j] < shortestDistance)
                    {
                        current = point;
                        shortestDistance = distances[i, j];
                    }
                }
            }

            if (current == null)
            {
                break;
            }

            visited[current.Item1, current.Item2] = true;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    int x = current.Item1 + i;
                    int y = current.Item2 + j;
                    if (x >= 0 && x < width && y >= 0 && y < height && graph[x, y])
                    {
                        int distance = distances[current.Item1, current.Item2] + 1;
                        if (distance < distances[x, y])
                        {
                            distances[x, y] = distance;
                            previous[x, y] = current;
                        }
                    }
                }
            }
        }

        if (previous[end.Item1, end.Item2] == null)
        {
            return null;
        }

        List<Tuple<int, int>> path = new List<Tuple<int, int>>();
        Tuple<int, int> node = end;
        while (node != null)
        {
            path.Add(node);
            node = previous[node.Item1, node.Item2];
        }
        path.Reverse();

        return path;
    }

    public static int ShortestPath(bool[,] grid, int startX, int startY, int endX, int endY)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Initialize distance array with maximum values and source node with 0 distance
        int[,] distance = new int[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                distance[i, j] = int.MaxValue;
            }
        }
        distance[startX, startY] = 0;

        // Create a priority queue to store nodes with minimum distance
        PriorityQueue<Node> pq = new PriorityQueue<Node>();
        pq.Enqueue(new Node(startX, startY, 0));

        while (pq.Count > 0)
        {
            // Get the node with minimum distance
            Node currentNode = pq.Dequeue();

            // Check if we have reached the destination
            if (currentNode.x == endX && currentNode.y == endY)
            {
                return distance[endX, endY];
            }

            // Visit neighbors of current node
            VisitNeighbors(grid, distance, pq, currentNode);
        }

        // If we can't reach the destination, return -1
        return -1;
    }

    private static void VisitNeighbors(bool[,] grid, int[,] distance, PriorityQueue<Node> pq, Node currentNode)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Define neighbor positions
        int[,] neighborPos = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

        // Visit each neighbor
        for (int i = 0; i < neighborPos.GetLength(0); i++)
        {
            int neighborX = currentNode.x + neighborPos[i, 0];
            int neighborY = currentNode.y + neighborPos[i, 1];

            // Check if neighbor is within bounds and is a valid path
            if (neighborX >= 0 && neighborX < rows && neighborY >= 0 && neighborY < cols && grid[neighborX, neighborY])
            {
                // Calculate distance to neighbor
                int newDistance = distance[currentNode.x, currentNode.y] + 1;

                // If new distance is less than current distance to neighbor, update distance and add to priority queue
                if (newDistance < distance[neighborX, neighborY])
                {
                    distance[neighborX, neighborY] = newDistance;
                    pq.Enqueue(new Node(neighborX, neighborY, newDistance));
                }
            }
        }
    }

    private class Node : IComparable<Node>
    {
        public int x;
        public int y;
        public int distance;

        public Node(int x, int y, int distance)
        {
            this.x = x;
            this.y = y;
            this.distance = distance;
        }

        public int CompareTo(Node other)
        {
            return distance.CompareTo(other.distance);
        }
    }

    private class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> data;

        public int Count { get { return data.Count; } }

        public PriorityQueue()
        {
            data = new List<T>();
        }

        public void Enqueue(T item)
        {
            data.Add(item);
            int index = data.Count - 1;
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (data[index].CompareTo(data[parentIndex]) >= 0)
                {
                    break;
                }
                T temp = data[index];
                data[index] = data[parentIndex];
                data[parentIndex] = temp;
                index = parentIndex;
            }
        }

        public T Dequeue()
        {
            int lastIndex = data.Count - 1;
            T firstItem = data[0];
            data[0] = data[lastIndex];
            data.RemoveAt(lastIndex);

            --lastIndex;
            int parentIndex = 0;
            while (true)
            {
                int childIndex = parentIndex * 2 + 1;
                if (childIndex > lastIndex)
                {
                    break;
                }
                int rightChild = childIndex + 1;
                if (rightChild <= lastIndex && data[rightChild].CompareTo(data[childIndex]) < 0)
                {
                    childIndex = rightChild;
                }
                if (data[parentIndex].CompareTo(data[childIndex]) <= 0)
                {
                    break;
                }
                T temp = data[parentIndex];
                data[parentIndex] = data[childIndex];
                data[childIndex] = temp;
                parentIndex = childIndex;
            }
            return firstItem;
        }

        public T Peek()
        {
            T firstItem = data[0];
            return firstItem;
        }

        public bool Contains(T item)
        {
            return data.Contains(item);
        }

        public List<T> ToList()
        {
            return new List<T>(data);
        }
    }
}

