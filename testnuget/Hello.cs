using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.ShortestPath;
using QuickGraph.Serialization;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Collections;
using QuickGraph.Algorithms.Search;

class Hello
{
    public DijkstraShortestPathAlgorithm<node, Edge<node>> dijkstra;
    public List<string> path;
    public VertexPredecessorRecorderObserver<node, Edge<node>> predecessorObserver;
    public AdjacencyGraph<node, Edge<node>> myGraph;
    public Dictionary<Edge<node>, float> edgeCost;
    public Func<Edge<node>, float> weighs;
    public List<node> myNode;
    public List<Edge<node>> listEdgeShortestGraph;
    public List<Edge<node>> listEdgeNotInShortestGraph;
    public int[] exitnode = { 50, 101, 169 };
    public List<Road> myRoad;
    public int soCanh = 0;
    static void Main()
    {
        Hello x = new Hello();
        List<IEnumerable<Edge<node>>> resultList = new List<IEnumerable<Edge<node>>>();
        x.SetRoad();
        x.setGraph();
        foreach(node mnode in x.myNode)
        {
            if (mnode.thuTu == 50 || mnode.thuTu == 101 || mnode.thuTu == 169) continue;
            resultList.Add(x.MinExitPath(mnode));
        }

        x.setUplist();

        Console.WriteLine("sau day la cac canh o trong graph shortest_path");

        foreach(Edge<node> edge in x.listEdgeShortestGraph)
        {
            Console.Write("(" + edge.Source.thuTu + ", " + edge.GetOtherVertex(edge.Source).thuTu+"), ");
        }

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("sau day la cac canh o trong graph notinshortest_path");

        foreach (Edge<node> edge in x.listEdgeNotInShortestGraph)
        {
            Console.Write("(" + edge.Source.thuTu + ", " + edge.GetOtherVertex(edge.Source).thuTu + "), ");
        }


        Console.WriteLine("tong so canh trog graph la: ");
        Console.WriteLine(x.listEdgeNotInShortestGraph.Count + x.listEdgeShortestGraph.Count);
        Console.WriteLine(x.soCanh);

        Console.ReadKey();
    }


    public void SetRoad()
    {   
        myNode = new List<node>();
        myRoad = new List<Road>();
        string[] lines = System.IO.File.ReadAllLines("C:\\Users\\ThaiTuan\\Documents\\simulation\\Assets\\graph.txt");
        foreach (string line in lines)
        {
            string[] words = line.Split(';');
            int thutu = int.Parse(words[0]);
            int x = int.Parse(words[1]);
            int y = int.Parse(words[2]);
            int index = 3;
            while (index < words.Length - 1)
            {
                int dinhKe = int.Parse(words[index].Split('#')[1]);
                soCanh += 1;
                int leng = int.Parse(words[index + 1]);
                int wid = int.Parse(words[index + 2]);
                float tr = float.Parse(words[index + 3]);
                Road mRoad = new Road(thutu, dinhKe, leng, wid, tr);
                myRoad.Add(mRoad);
                index = index + 4;
            }
            node mNode = new node(x, y, thutu);
            myNode.Add(mNode);
        }
    }

    public void setGraph()
    {
        listEdgeNotInShortestGraph = new List<Edge<node>>();
        myGraph = new AdjacencyGraph<node, Edge<node>>();
        edgeCost = new Dictionary<Edge<node>, float>(myRoad.Count);
        foreach (node node in myNode)
        {
            myGraph.AddVertex(node);
        }
        foreach (Road road in myRoad)
        {
            node n1 = myNode[road.node1 - 1];
            node n2 = myNode[road.node2 - 1];
            Edge<node> edge = new Edge<node>(n1, n2);
            myGraph.AddEdge(edge);
            edgeCost.Add(edge, road.length);
            if(contains(listEdgeNotInShortestGraph, edge)[0] != 1)
            {
                listEdgeNotInShortestGraph.Add(edge);
            }
            
        }

        listEdgeShortestGraph = new List<Edge<node>>();
}

    public void Dijkstra(node mNode)
            
    {
        dijkstra = new DijkstraShortestPathAlgorithm<node, Edge<node>>(myGraph, e => edgeCost[e]);
        predecessorObserver = new VertexPredecessorRecorderObserver<node, Edge<node>>();
        using (predecessorObserver.Attach(dijkstra))
            dijkstra.Compute(mNode);
    }

    IEnumerable<Edge<node>> MinExitPath(node mNode)
    {
        Dijkstra(mNode);

        List<node> list = new List<node>();
        foreach (int index in exitnode) {
            list.Add(myNode[index - 1]);
        }

        Double min = Double.MaxValue;
        node mexitNode = new node(0, 0, 0);
        foreach(node node in list)
        {
            Double mMin = 0;
            dijkstra.TryGetDistance(node, out mMin);
            if (mMin < min){
                min = mMin;
                mexitNode = node;
            }
        }

        IEnumerable<Edge<node>> result;
        predecessorObserver.VertexPredecessors.TryGetPath(mexitNode, out result);
        Console.Write(min + "[ ");
        foreach (Edge<node> tuan in result)
        {
            Console.Write(tuan.Source.thuTu + ", ");
            if (contains(listEdgeShortestGraph, tuan)[0] ==1) continue;
            listEdgeShortestGraph.Add(tuan);
            
        }

        Console.Write(mexitNode.thuTu);
        Console.Write("]");
        Console.WriteLine();
        return result;
    }

    int[] contains(List<Edge<node>> list, Edge<node> edge)
    {
        int index = 0;
        foreach (Edge<node> edgeList in list)
        {
            if ((edgeList.Source.thuTu == edge.Source.thuTu) && edgeList.GetOtherVertex(edgeList.Source).thuTu == edge.GetOtherVertex(edge.Source).thuTu) {
                int[] b = { 1, index };
                return b;
            }
            if ((edgeList.Source.thuTu == edge.GetOtherVertex(edge.Source).thuTu) && edgeList.GetOtherVertex(edgeList.Source).thuTu == edge.Source.thuTu)
            {
                int[] b = { 1, index };
                return b;
            }
            index += 1;
        }
        int[] a = { 0, 0 };
        return a;
    }

    void remove(List<Edge<node>> list, Edge<node> edge)
    {
        list.Remove(edge);
        Edge<node> newEdge = new Edge<node>(edge.GetOtherVertex(edge.Source), edge.Source);
        list.Remove(newEdge);
    }

    void setUplist()
    {
        foreach(Edge<node> edge in listEdgeShortestGraph)
        {
            int[] result = contains(listEdgeNotInShortestGraph, edge);
            if (result[0] == 1)
            {
                listEdgeNotInShortestGraph.RemoveAt(result[1]);
            }
        }

        foreach (Edge<node> edge in listEdgeShortestGraph)
        {
            if (contains(listEdgeNotInShortestGraph, edge)[0] ==1)
            {
                Console.Write("contain roi ne   ");
            }
        }
    }
}