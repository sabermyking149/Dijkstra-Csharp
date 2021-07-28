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
            AdjacencyList<string> a = new AdjacencyList<string>();
            //添加顶点
            string A = "A";
            string B = "B";
            string C = "C";
            string D = "D";

            a.AddVertex(A);
            a.AddVertex(B);
            a.AddVertex(C);
            a.AddVertex(D);
            //添加边
            a.AddEdge(A, B, 0.25, true);
            a.AddEdge(A, C, 0.50, true);
            a.AddEdge(A, D, 0.75, true);
            a.AddEdge(B, D, 1.00, false);
            Console.WriteLine(a.ToString());
            Console.ReadKey();

            dijkstra1.AdjacencyList<string>.Dijkstra.dijkstra(a.items, 10, A, B);

        }
    }
    

    public class AdjacencyList<T>
    {
        public List<Vertex<T>> items; //图的顶点集合
        public AdjacencyList() : this(10) { } //构造方法
        public AdjacencyList(int capacity) //指定容量的构造方法
        {
            items = new List<Vertex<T>>(capacity);
        }
        public void AddVertex(T item) //添加一个顶点
        {   //不允许插入重复值
            if (Contains(item))
            {
                throw new ArgumentException("插入了重复顶点！");
            }
            items.Add(new Vertex<T>(item));
        }
        public void AddEdge(T from, T to, double weight, bool isDirected) //添加边,可选择有向还是无向
        {
            Vertex<T> fromVer = Find(from); //找到起始顶点
            if (fromVer == null)
            {
                throw new ArgumentException("头顶点并不存在！");
            }

            Vertex<T> toVer = Find(to); //找到结束顶点
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
        public bool Contains(T item) //查找图中是否包含某项
        {
            foreach (Vertex<T> v in items)
            {
                if (v.data.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }
        private Vertex<T> Find(T item) //查找指定项并返回
        {
            foreach (Vertex<T> v in items)
            {
                if (v.data.Equals(item))
                {
                    return v;
                }
            }
            return null;
        }
        //添加有向边
        private void AddDirectedEdge(Vertex<T> fromVer, Vertex<T> toVer, double weight)
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
                    //node.weight = weight;  这两个都不对
                    //tmp.weight = weight;
                    node = node.next;
                } while (node != null);
                tmp.next = new Node(toVer); //添加到链表未尾
                tmp.next.weight = weight;
            }
        }
        public override string ToString() //仅用于测试
        {   //打印每个节点和它的邻接点
            string s = string.Empty;
            foreach (Vertex<T> v in items)
            {
                s += v.data.ToString() + ":";
                if (v.firstEdge != null)
                {
                    Node tmp = v.firstEdge;
                    while (tmp != null)
                    {
                        s += v.data.ToString() + " -> " + tmp.adjvex.data.ToString() + " = " + tmp.weight.ToString() + " ";
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
            public Vertex<T> adjvex; //邻接点域
            public Node next; //下一个邻接点指针域
            public double weight;
            public Node(Vertex<T> value)
            {
                adjvex = value;
                next = null;
            }
        }
        //嵌套类，表示存放于数组中的表头结点
        public class Vertex<TValue>
        {
            public TValue data; //数据
            public Node firstEdge; //邻接点链表头指针
            public Boolean visited; //访问标志,遍历时使用
            public Vertex(TValue value) //构造方法
            {
                data = value;
            }
        }

        public class Dijkstra
        {
            string path;
            public static void dijkstra(List<Vertex<T>> items, int capacity, T startVerName, T endVerName)
            {
                string s = string.Empty;
                foreach (Vertex<T> v in items)
                {
                    s += v.data.ToString() + ":";
                    if (v.firstEdge != null)
                    {
                        Node tmp = v.firstEdge;
                        while (tmp != null)
                        {
                            s += v.data.ToString() + " -> " + tmp.adjvex.data.ToString() + " = " + tmp.weight.ToString() + " ";
                            tmp = tmp.next;
                        }
                    }
                    s += "\r\n";
                }

                Console.WriteLine(s);


                //搜寻开始结点、终止结点名对应的结点
                Vertex<T> startVer = null;
                Vertex<T> endVer = null;
                foreach (Vertex<T> tmp in items)
                {
                    if (tmp.data.Equals(startVerName))
                    {
                        startVer = tmp;
                        break;
                    }
                }

                foreach (Vertex<T> tmp in items)
                {
                    if (tmp.data.Equals(endVerName))
                    {
                        endVer = tmp;
                        break;
                    }
                }

                Console.WriteLine(startVer.data.ToString());
                Console.WriteLine(endVer.data.ToString());

                Console.ReadKey();

                List<Node> dist = new List<Node>(capacity);

                foreach (Node tmp in dist)
                {
                    tmp.weight = 1024;
                    tmp.adjvex.visited = false;
                }

                

            }
        }

    }
}
