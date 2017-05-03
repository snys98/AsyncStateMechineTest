using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncStateMechineTest
{
    public class Node
    {
        private static List<Node> _nodes = new List<Node>();
        private static int _id = 1;
        public Node()
        {
            this.Id = _id++;
            _nodes.Add(this);
        }

        public int Id { get; }

        public Node Left { get; set; }
        public Node Right { get; set; }

        public bool IsLeaf => this.Left == null && this.Right == null;
        public bool IsTraveled { get; set; } = false;

        public override string ToString()
        {
            return $"{Left?.Id ?? 0}|{this.Id}|{Right?.Id ?? 0}";
        }

        //public string ToGraphic()
        //{
        //    if (IsLeaf)
        //    {
        //        return $"{{{this.Id.ToString()}}}";
        //    }
        //    return $"{{{this.Id.ToString()}}}";
        //}
    }
    class Program
    {
        static void Main(string[] args)
        {
            #region MyRegion
            var node = new Node()
            {
                Left = new Node()
                {
                    Left = new Node()
                    {
                        Left = new Node()
                        {
                            Left = new Node(),
                            Right = new Node()
                        },
                        Right = new Node()
                    },
                    Right = new Node()
                },
                Right = new Node()
                {
                    Left = new Node()
                    {
                        Left = new Node(),
                        Right = new Node()
                    },
                    Right = new Node()
                    {
                        Left = new Node()
                        {
                            Left = new Node()
                            {
                                Left = new Node(),
                                Right = new Node()
                            },
                            Right = new Node()
                            {
                                Left = new Node(),
                                Right = new Node()
                            }
                        },
                        Right = new Node()
                        {
                            Left = new Node()
                            {
                                Left = new Node()
                                {
                                    Left = new Node(),
                                    Right = new Node()
                                    {
                                        Left = new Node(),
                                        Right = new Node()
                                    }
                                },
                                Right = new Node()
                            },
                            Right = new Node()
                        }
                    }
                }
            }; 
            #endregion
            var task = Travle(node);
            while (true)
            {
                if (task.IsCompleted)
                {
                    Console.WriteLine("线程id:"+ Thread.CurrentThread.ManagedThreadId +"|线程上下文标识:"+ Thread.CurrentThread.ExecutionContext.GetHashCode() + ":" + task.Result);
                    break;
                }
            }
            Console.ReadKey();
        }


        public static async Task<int> Travle(Node node)
        {
            node.IsTraveled = true;
            if (!node.IsLeaf)
            {
                Console.WriteLine("线程id:" + Thread.CurrentThread.ManagedThreadId + "|线程上下文标识:" + Thread.CurrentThread.ExecutionContext.GetHashCode() + ":" + await Travle(node.Left));
                Console.WriteLine("线程id:" + Thread.CurrentThread.ManagedThreadId + "|线程上下文标识:" + Thread.CurrentThread.ExecutionContext.GetHashCode() + ":" + await Travle(node.Right));
            }
            return node.Id;
            //   StateMechine stateMechine = new StateMechine();
            //stateMechine.node = node;
            //stateMechine.builder = AsyncTaskMethodBuilder<int>.Create();
            //stateMechine.state = -1;
            //AsyncTaskMethodBuilder<int> next_builder = stateMechine.builder;
            //   next_builder.Start<StateMechine>(ref stateMechine);
            //return stateMechine.builder.Task;
        }

    }

    public class StateMechine:IAsyncStateMachine
    {
        public Node node { get; set; }

        public AsyncTaskMethodBuilder<int> builder { get; set; }
        public int state { get; set; }
        public void MoveNext()
        {
            return;
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            return;
        }
    }
}
