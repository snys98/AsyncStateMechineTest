using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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

            Console.WriteLine($@"
1.state machine implemention of async&await
2.state machine implemention of custom IAsyncStateMachine");
            var selection = Console.ReadKey().Key;
            Console.WriteLine();
            Task<int> task;
            switch (selection)
            {
                case ConsoleKey.D1:
                    task = Travle(node);
                    while (true)
                    {
                        if (task.IsCompleted)
                        {
                            Console.WriteLine("线程id:" + Thread.CurrentThread.ManagedThreadId + "|线程上下文标识:" + Thread.CurrentThread.ExecutionContext.GetHashCode() + ":" + task.Result);
                            break;
                        }
                    }
                    break;
                case ConsoleKey.D2:
                    task = StateMechineTravle(node);
                    while (true)
                    {
                        if (task.IsCompleted)
                        {
                            Console.WriteLine("线程id:" + Thread.CurrentThread.ManagedThreadId + "|线程上下文标识:" + Thread.CurrentThread.ExecutionContext.GetHashCode() + ":" + task.Result);
                            break;
                        }
                    }
                    break;
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

        }

        public static Task<int> StateMechineTravle(Node node)
        {
            StateMechine stateMechine = new StateMechine();
            stateMechine.node = node;
            stateMechine.builder = AsyncTaskMethodBuilder<int>.Create();
            stateMechine.state = -1;
            AsyncTaskMethodBuilder<int> next_builder = stateMechine.builder;
            next_builder.Start<StateMechine>(ref stateMechine);
            return stateMechine.builder.Task;
        }
    }

    public struct StateMechine : IAsyncStateMachine
    {
        public int state;

        public AsyncTaskMethodBuilder<int> builder;

        public Node node;

        private object wrap1;

        private object wrap2;

        private TaskAwaiter<int> awaiter;

        void IAsyncStateMachine.MoveNext()
        {
            int num = this.state;
            int id;
            try
            {
                TaskAwaiter<int> taskAwaiter;
                if (num != 0)
                {
                    if (num == 1)
                    {
                        taskAwaiter = this.awaiter;
                        this.awaiter = default(TaskAwaiter<int>);
                        this.state = -1;
                        goto IL_1AB;
                    }
                    this.node.IsTraveled = true;
                    if (this.node.IsLeaf)
                    {
                        goto IL_20C;
                    }
                    this.wrap1 = Thread.CurrentThread.ManagedThreadId;
                    this.wrap2 = Thread.CurrentThread.ExecutionContext.GetHashCode();
                    taskAwaiter = Program.Travle(this.node.Left).GetAwaiter();
                    if (!taskAwaiter.IsCompleted)
                    {
                        this.state = 0;
                        this.awaiter = taskAwaiter;
                        this.builder.AwaitUnsafeOnCompleted<TaskAwaiter<int>, StateMechine>(ref taskAwaiter, ref this);
                        return;
                    }
                }
                else
                {
                    taskAwaiter = this.awaiter;
                    this.awaiter = default(TaskAwaiter<int>);
                    this.state = -1;
                }
                int arg_CC_0 = taskAwaiter.GetResult();
                taskAwaiter = default(TaskAwaiter<int>);
                object obj = arg_CC_0;
                Console.WriteLine(string.Concat(new object[]
                {
                    "线程id:",
                    this.wrap1,
                    "|线程上下文标识:",
                    this.wrap2,
                    ":",
                    obj
                }));
                this.wrap1 = null;
                this.wrap2 = null;
                this.wrap2 = Thread.CurrentThread.ManagedThreadId;
                this.wrap1 = Thread.CurrentThread.ExecutionContext.GetHashCode();
                taskAwaiter = Program.Travle(this.node.Right).GetAwaiter();
                if (!taskAwaiter.IsCompleted)
                {
                    this.state = 1;
                    this.awaiter = taskAwaiter;
                    this.builder.AwaitUnsafeOnCompleted<TaskAwaiter<int>, StateMechine>(ref taskAwaiter, ref this);
                    return;
                }
                IL_1AB:
                int arg_1BA_0 = taskAwaiter.GetResult();
                taskAwaiter = default(TaskAwaiter<int>);
                obj = arg_1BA_0;
                Console.WriteLine(string.Concat(new object[]
                {
                    "线程id:",
                    this.wrap2,
                    "|线程上下文标识:",
                    this.wrap1,
                    ":",
                    obj
                }));
                this.wrap2 = null;
                this.wrap1 = null;
                IL_20C:
                id = this.node.Id;
            }
            catch (Exception exception)
            {
                this.state = -2;
                this.builder.SetException(exception);
                return;
            }
            this.state = -2;
            this.builder.SetResult(id);
        }

        [DebuggerHidden]
        void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
        {
            this.builder.SetStateMachine(stateMachine);
        }
    }
}
