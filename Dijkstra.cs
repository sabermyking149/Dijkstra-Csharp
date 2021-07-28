using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dijkstra1
{
    class Program
    {
        static void Main(string[] args)
        {
            //获取配置文件顶点数
            int VertexNum = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["VertexNum"]);
            //Vertex配置文件保存顶点信息，格式为 "(顶点名,顶点id)|(顶点名,顶点id)|...|"
            string VertexDataString = System.Configuration.ConfigurationManager.AppSettings["VertexData"];

            AdjacencyList a = new AdjacencyList(VertexNum);
            
            string[] strArray = VertexDataString.Split('|');
            foreach (string k in strArray)
            {
                string x = k.Trim('(').Trim(')'); //去掉左右小括号
                //分割顶点的名称和id
                int pos = x.IndexOf(',');
                if (pos < 0)
                    break;
                AdjacencyList.VertexData v = new AdjacencyList.VertexData(x.Substring(0, pos), Convert.ToInt32(x.Substring(pos + 1)));
             
                //Console.WriteLine("{0}{1}",v.GetVertexName(),v.GetVertexID());
                //添加顶点
                a.AddVertex(v);
               
            }

            //由配置文件获取加权有向图边的关系，格式暂时为 "(顶点名1,顶点名2,权值,是否是单向)|(顶点名1,顶点名2,权值,是否是单向)|...|"
            string EdgeString = System.Configuration.ConfigurationManager.AppSettings["Edge"];
            strArray = EdgeString.Split('|');
            foreach (string k in strArray)
            {
                string x = k.Trim('(').Trim(')');
                int pos = 0;
                
                pos = x.IndexOf(',');
                if (pos < 0)
                    break;
                AdjacencyList.VertexData VertexData1 = a.Find(x.Substring(0,pos)).data;
                x = x.Remove(0, pos + 1);
               
                pos = x.IndexOf(',');
                AdjacencyList.VertexData VertexData2 = a.Find(x.Substring(0, pos)).data;
                x = x.Remove(0, pos + 1);

                pos = x.IndexOf(',');
                double weight = Convert.ToDouble(x.Substring(0, pos));
                x = x.Remove(0, pos + 1);
                
                bool isDirected; 
                if (x == "T")
                {
                    isDirected = true;
                }
                else if (x == "F")
                {
                    isDirected = false;
                }
                else
                    throw new ArgumentException("无效的方向标识！");

                //添加边
                a.AddEdge(VertexData1, VertexData2, weight, isDirected);
            }

            //Console.WriteLine(ConString);

            //添加顶点
            //AdjacencyList.VertexData A = new AdjacencyList.VertexData("A", 1);
            //AdjacencyList.VertexData B = new AdjacencyList.VertexData("B", 2);
            //AdjacencyList.VertexData C = new AdjacencyList.VertexData("C", 3);
            //AdjacencyList.VertexData D = new AdjacencyList.VertexData("D", 4);
            //AdjacencyList.VertexData E = new AdjacencyList.VertexData("E", 5);
            //AdjacencyList.VertexData F = new AdjacencyList.VertexData("F", 6);

            //a.AddVertex(A);
            //a.AddVertex(B);
            //a.AddVertex(C);
            //a.AddVertex(D);
            //a.AddVertex(E);
            //a.AddVertex(F);
            //添加边
            //a.AddEdge(A, B, 3, true);
            //a.AddEdge(A, C, 10, true);
            //a.AddEdge(C, F, 5, true);
            //a.AddEdge(B, D, 2, true);
            //a.AddEdge(D, E, 4, true);
            //a.AddEdge(E, C, 2, true);
            //a.AddEdge(E, F, 7, true);

            Console.WriteLine(a.ToString());
            Console.ReadKey();

            string StartVertexName = System.Configuration.ConfigurationManager.AppSettings["StartVertexName"];
            string EndVertexName = System.Configuration.ConfigurationManager.AppSettings["EndVertexName"];
            RouteDijkstra.DijkstraShortestPath(a, VertexNum, a.Find(StartVertexName).data, a.Find(EndVertexName).data);
        }
    }
  
    public class AdjacencyList
    {
        public List<Vertex> items; //图的顶点集合
        public AdjacencyList() : this(10) { } //构造方法
        public AdjacencyList(int capacity) //指定容量的构造方法
        {
            items = new List<Vertex>(capacity);
        }
        public void AddVertex(VertexData item) //添加一个顶点
        {   //不允许插入重复值
            if (Contains(item))
            {
                throw new ArgumentException("插入了重复顶点！");
            }
            items.Add(new Vertex(item));
        }
        public void AddEdge(VertexData from, VertexData to, double weight, bool isDirected) //添加边,可选择有向还是无向
        {
            Vertex fromVer = Find(from); //找到起始顶点
            if (fromVer == null)
            {
                throw new ArgumentException("头顶点并不存在！");
            }

            Vertex toVer = Find(to); //找到结束顶点
            if (toVer == null)
            {
                throw new ArgumentException("尾顶点并不存在！");
            }

            //无向边的两个顶点都需记录边信息
            if (isDirected == false)    //无向
            {
                AddDirectedEdge(fromVer, toVer, weight);
                AddDirectedEdge(toVer, fromVer, weight);
            }
            else
                AddDirectedEdge(fromVer, toVer, weight);
        }
        public bool Contains(VertexData item) //查找图中是否包含某项
        {
            foreach (Vertex v in items)
            {
                if (v.data.GetVertexName().ToString() == item.GetVertexName().ToString() ||   //名称id都不能有一致的
                    v.data.GetVertexID() == item.GetVertexID())
                {
                    return true;
                }
            }
            return false;
        }
        public Vertex Find(VertexData item) //查找指定项并返回
        {
            foreach (Vertex v in items)
            {
                if (v.data.GetVertexName().ToString() == item.GetVertexName().ToString() &&
                    v.data.GetVertexID() == item.GetVertexID())
                {
                    return v;
                }
            }
            return null;
        }
        public Vertex Find(int VertexID) //重载Find，只根据VertexID返回Vertex
        {
            foreach (Vertex v in items)
            {
                if (v.data.GetVertexID() == VertexID)
                {
                    return v;
                }
            }
            return null;
        }
        public Vertex Find(string VertexName) //重载Find，只根据VertexName返回Vertex
        {
            foreach (Vertex v in items)
            {
                if (v.data.GetVertexName() == VertexName)
                {
                    return v;
                }
            }
            return null;
        }
        public bool IsAllVertexVisited(List<Vertex> items) //是否每一个顶点都被访问过
        {
            foreach (Vertex item in items)
            {
                if (item.visited == false)
                    return false;
            }

            return true;
        }
        //添加有向边
        private void AddDirectedEdge(Vertex fromVer, Vertex toVer, double weight)
        {
            if (fromVer.firstEdge == null) //无邻接点时
            {
                fromVer.firstEdge = new Node(toVer);
                fromVer.firstEdge.weight = weight;
            }
            else
            {
                Node tmp, node = fromVer.firstEdge;
                do
                {   //检查是否添加了重复边
                    if (node.adjvex.data.Equals(toVer.data))
                    {
                        throw new ArgumentException("添加了重复的边！");
                    }
                    tmp = node;

                    node = node.next;
                } while (node != null);
                tmp.next = new Node(toVer); //添加到链表未尾
                tmp.next.weight = weight;
            }
        }
        public override string ToString() //仅用于测试
        {   //打印每个节点和它的邻接点
            string s = string.Empty;
            foreach (Vertex v in items)
            {
                s += v.data.GetVertexName().ToString() + v.data.GetVertexID().ToString() + ":";
                if (v.firstEdge != null)
                {
                    Node tmp = v.firstEdge;
                    while (tmp != null)
                    {
                        s += v.data.GetVertexName().ToString() + v.data.GetVertexID().ToString() +
                            " -> " + tmp.adjvex.data.GetVertexName().ToString() + tmp.adjvex.data.GetVertexID().ToString() +
                            " = " + tmp.weight.ToString() + " ";
                        tmp = tmp.next;
                    }
                }
                s += "\r\n";
            }
            return s;
        }

        //嵌套类，表示链表中的表结点
        public class Node
        {
            public Vertex adjvex; //邻接点域
            public Node next; //下一个邻接点指针域
            public double weight; //权值
            public Node(Vertex value)
            {
                adjvex = value;
                next = null;
            }
        }
        //嵌套类，表示存放于数组中的表头结点
        public class Vertex
        {
            public VertexData data; //顶点数据域，包括顶点名称和id号
            public Node firstEdge; //邻接点链表头指针
            public bool visited; //访问标志,遍历时使用
            public Vertex(VertexData value) //构造方法
            {
                data = value;
                visited = false;
            }
        }
        //嵌套类，表示表头结点对应顶点的data信息
        public class VertexData
        {
            string VertexName;
            int VertexID;

            public VertexData()
            {
                VertexName = "null";
                VertexID = 0;
            }
            public VertexData(string name, int id)
            {
                VertexName = name;
                VertexID = id;
            }

            public string GetVertexName() { return VertexName; }
            public int GetVertexID() { return VertexID; }
            public void SetVertexName(string name) { VertexName = name; }
            public void SetVertexID(int id) { VertexID = id; }
        }
    }

    public class RouteDijkstra
    {
        //string path = string.Empty;

        public static void DijkstraShortestPath(AdjacencyList adj, int capacity, AdjacencyList.VertexData startVerData, AdjacencyList.VertexData endVerData)
        {
            double[] dist = new double[1000];
            double inf = Convert.ToDouble(int.MaxValue);
            double min;
            int n;
            int u = 0;
            //string s = string.Empty;
            

            //搜寻开始结点、终止结点名对应的结点
            AdjacencyList.Vertex startVer = null;
            AdjacencyList.Vertex endVer = null;

            startVer = adj.Find(startVerData);
            endVer = adj.Find(endVerData);
       
            //Console.WriteLine("{0}{1}",startVer.data.GetVertexName().ToString(),startVer.data.GetVertexID().ToString());
            //Console.WriteLine("{0}{1}",endVer.data.GetVertexName().ToString(),endVer.data.GetVertexID().ToString());

            //校验adj顶点数是否和capacity一致
            n = adj.items.Count();
            if (n != capacity)
            {
                throw new ArgumentException("图顶点数与传入顶点数不一致！");
            }

            int[] last = new int[n];//存储最短路径，每个结点的上一个结点

            //初始化dist数组
            for (int i = 0; i < n; i++)
                dist[i] = inf;

            //初始化路径表
            for (int i = 0; i < n; i++)
            {
                last[i] = startVerData.GetVertexID() - 1;  //vertexData id从1开始，作为数组下标要减去1
            }
            last[startVerData.GetVertexID() - 1] = -1;

            dist[startVer.data.GetVertexID() - 1] = 0; 
            startVer.visited = true;                   //将开始结点设为已访问

            AdjacencyList.Node tmpNode = startVer.firstEdge;
            if (tmpNode != null)
            {
                while (tmpNode != null)
                {
                    dist[tmpNode.adjvex.data.GetVertexID() - 1] = tmpNode.weight;
                    tmpNode = tmpNode.next;
                }
            }
            //第一个点(初始点已装载完毕)
            //遍历结点，计算最短路径
            AdjacencyList.Vertex tmpVertex = null;
            for (int i = 0; i < n - 1; i++)
            {
                min = inf;
                
                //找到离StartVer结点最近的点
                foreach (AdjacencyList.Vertex v in adj.items)
                {
                    if (v.visited == false)
                    {
                        int j = v.data.GetVertexID() - 1;
                        if (dist[j] < min && dist[j] != 0)
                        {
                            min = dist[j];
                            tmpVertex = v;
                            u = j;
                        }
                    }
                }

                if (tmpVertex != null)
                {
                    tmpVertex.visited = true;
                    if (tmpVertex.firstEdge != null)
                    {
                        do
                        {
                            int vv = tmpVertex.firstEdge.adjvex.data.GetVertexID() - 1;
                            if (tmpVertex.firstEdge.weight != 0 && tmpVertex.firstEdge.weight < inf && tmpVertex.firstEdge.adjvex.visited == false)
                            {
                                if (min + tmpVertex.firstEdge.weight < dist[vv])
                                {
                                    dist[vv] = min + tmpVertex.firstEdge.weight;
                                    last[vv] = u;
                                }
                            }
                            tmpVertex.firstEdge = tmpVertex.firstEdge.next;
                        } while (tmpVertex.firstEdge != null);
                    }
                }             
            }

            foreach (AdjacencyList.Vertex a in adj.items)
                Console.WriteLine(a.visited);

            for (int i = 0; i < capacity; i++)
                Console.WriteLine("{0},{1}",dist[i],last[i]);
            

            PrintPath(last, dist, adj, startVerData.GetVertexID());
            Console.ReadKey();
        }
        
        public static void PrintPath(int[] last,double[] dist,AdjacencyList adj,int startVertexID)
        {
            double inf = Convert.ToDouble(int.MaxValue);
            int n = adj.items.Count();
            for (int i = 0;i < n;i++)
            {
                if (i != startVertexID - 1)
                {
                    if (last[i] == -1 || dist[i] == 0)
                        Console.WriteLine("[{0} ==> {1}] : {2}",adj.Find(startVertexID).data.GetVertexName(),
                            adj.Find(i + 1).data.GetVertexName(), "No Path!");
                    else
                    {
                        Console.Write("[{0} ==> {1}] : {2}\t", adj.Find(startVertexID).data.GetVertexName(),
                             adj.Find(i + 1).data.GetVertexName(), dist[i]);
                        PrintPathLoop(last, adj,i + 1);
                        Console.WriteLine();
                    }
                }
            }
        }

        public static void PrintPathLoop(int[] last, AdjacencyList adj,int endVertexID)
        {
            if (last[endVertexID - 1] != -1)
            {
                PrintPathLoop(last, adj,last[endVertexID - 1] + 1);
                Console.Write("->");
            }
            Console.Write(adj.Find(endVertexID).data.GetVertexName());
        }
    }
}
