using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepDiveTechnicals
{
    public static class HardProblems
    {
        /// <summary>
        /// Problem : 17.2
        /// Description : Write a method to shuffle a deck of cards. It must be a perfect shuffle-in other words, each
        /// of the 52! permutations of the deck has to be equally likely.Assume that you are given a random
        /// number generator which is perfect.
        /// <Approach>
        /// We can follow the approach build Shuffle(n) by Shuffle(n-1). So I build my base case by recursing until
        /// I reach the start of the list with my cards. Then as my index is increasing I am swaping randomly with 
        /// my random generator my current incoming card (pointed by my index) with a random other card in the range
        /// [0,index]. If I want i can keep only the last produced shuffle of the whole deck. Otherwise I can add to a List<int[]>
        /// all my different shuffles during recursion.
        /// <Time>O(n) foreach card</Time>
        /// <Space>If I remove my allShuffles List<int[]> and just return the final record then my space is O(logn) for
        /// the memory stack used by recursion</Space>
        /// </Approach>
        /// []
        /// [1]
        /// [1,2] -> [3,1,2] -> [3,4,2,1] -> [3,4,5,1,2]
        /// </summary>
        public static int[] Shuffle()
        {
            int[] cards = new int[10] //52 cards
            {1,2,3,4,5,6,7,8,9,10};
            //  {1,2,3,4,..,52} where 1=1heart 13=1diamond etc...
            return ShuffleHelper(cards,9);
        }
        public static List<int[]> allShuffles = new List<int[]>();
        public static int[] ShuffleHelper(int[] cards, int index)
        {
            if (index == 0) return cards; //baseCase

            cards = ShuffleHelper(cards, index - 1);

            int indexToSwap = RandomGeneratorBetweenArray(0, index);

            int temp = cards[indexToSwap];
            cards[indexToSwap] = cards[index];
            cards[index] = temp;
            var tempCards = new int[index + 1];
            for (int i=0;i<=index;i++)
            {
                tempCards[i] = cards[i];
            }
            allShuffles.Add(tempCards);
            return cards;
        }
        public static int RandomGeneratorBetweenArray(int lowerIndex, int higherIndex)
        {
            Random number = new Random();
            return number.Next(lowerIndex, higherIndex - 1);
        }

        /// <summary>
        /// Problem : 17.3
        /// Description : Write a method to randomly generate a set of m integers from an array of size n. Each
        /// element must have equal probability of being chosen.
        /*
         * (1,3,5) array (1,2,3,5,6,7,8)
         * my initial array has length of 7. Each of my records has a 1/7 probability of being chosen
         */
        public static void RandomSet()
        {
            List<int> initialList = new List<int> { 1, 3, 5, 2, 6, 7, 8 };
            decimal probability = (decimal)(1.0m / initialList.Count) * 100;

            List<int> probabiListic = new List<int>();
            foreach (var item in initialList)
            {
                Random num = new Random();
                int currentProb = num.Next(0, 100);

                if (currentProb <= Convert.ToInt32(probability))
                    probabiListic.Add(item);
            }
            Console.WriteLine("Stop");
        }

        /// <summary>
        /// Problem : 17.4
        /// Description :An array A contains all the integers from O to n, except for one number which
        /// is missing.In this problem, we cannot access an entire integer in A with a single operation.The
        /// elements of A are represented in binary, and the only operation we can use to access them is "fetch
        /// the jth bit of A[i];' which takes constant time. Write code to find the missing integer. Can you do it
        /// in O(n) time?
        public static void MissingNumber()
        {

        }

        /// <summary>
        /// Problem : 17.4
        /// Description :An array A contains all the integers from O to n, except for one number which
        /// is missing. Write code to find the missing integer.
        /// Approach : Go through the input list and add each number to calculate the Sum
        /// then you're missing number is going to be the diff between your sum and the actual sum form 0 to n
        /// sum(0,n) = (n*(n+1))/2
        public static void MissingNumberNonBinary()
        {
            int n = 10;
            List<int> intList = new List<int>
            {
                1,2,3,4,6,7,8,9,10
            };
            int sum = 0;
            foreach (var item in intList)
            {
                sum += item;
            }
            int missingNumber = (n*(n+1)/2) - sum;
        }

        /// <summary>
        /// Problem : 17.4
        /// Description :Given an array filled with letters and numbers, find the longest subarray with
        /// an equal number of letters and numbers.
        /// <Time>O(N)</Time>
        /// Approach :
        /// Keep a dictionary (indextoFrequency) with the number of Numbers and Letters for each index in the array.
        /// Then iterate over it and count the deltas between number of letters and numbers for each index.
        /// Every time you calculate a delta keep it as a key in another dicitonary (differenceBetweenPositionsOccurencies)
        /// and keep the first time the this delta occured and every time keep the last index whenever you update it
        /// Every time we reproduce a delta that we have reproduced before in the iteration that means that we have created
        /// a subarray of equal numbers and letters. Calc the diff of the (endIndex-startIndex) for current diff and compare it 
        /// to the longestRout integer. Store the longestRoute (and the start and end index if desired).
        /// </summary>
        public static void LettersAndNumber()
        {
            var indexToFrequency = new Dictionary<int, Tuple<int, int>>();
            //this dict keeps the index of the list and the number of letters and numbers at this point
            List<string> input = new List<string>
            {
                "letter", "number", "letter", "letter", "letter", "number", "number",
                "number", "letter", "number", "letter", "letter", "number", "number",
                "letter", "letter", "letter", "letter", "letter", "letter"
            };
            // 1L 0N, 1L 1N, 2L 1N, 3L 1N, 4L 1N, 4L 2N, 4L 3N, 4L 4N, 5L 5N, 6L 5N, 7L 5N, 7L 6N, 7L 7N,  
            int letterSum = 0;
            int numberSum = 0;
            int index = 0;
            foreach (var item in input)
            {
                if (item == "letter")
                    letterSum++;
                else
                    numberSum++;
                indexToFrequency.Add(index, new Tuple<int, int>(letterSum, numberSum));
                index++;
            }
            var differenceBetweenPositionsOccurencies = new Dictionary<int,Tuple<int,int>>(); //difference,<startindex,endindex>
            int longestRoute = 0;
            foreach (var item in indexToFrequency)
            {
                int delta = Math.Abs(item.Value.Item1 - item.Value.Item2);
                if (!differenceBetweenPositionsOccurencies.ContainsKey(delta))
                {
                    differenceBetweenPositionsOccurencies.Add(delta, new Tuple<int, int>(item.Key,item.Key));
                }
                else
                {
                    differenceBetweenPositionsOccurencies[delta]= new Tuple<int,int>(differenceBetweenPositionsOccurencies[delta].Item1, item.Key);
                    longestRoute = 
                        (differenceBetweenPositionsOccurencies[delta].Item2 - differenceBetweenPositionsOccurencies[delta].Item1) > longestRoute
                        ? (differenceBetweenPositionsOccurencies[delta].Item2 - differenceBetweenPositionsOccurencies[delta].Item1) 
                        : longestRoute;
                }
            }

        }

        /// <summary>
        /// Problem : 17.7
        /// Description : Each year, the government releases a list of the 10,000 most common baby names
        /// and their frequencies(the number of babies with that name). The only problem with this is that
        /// some names have multiple spellings.For example, "John" and ''.Jon" are essentially the same name
        /// but would be listed separately in the list. Given two lists, one of names/frequencies and the other
        /// of pairs of equivalent names, write an algorithm to print a new list of the true frequency of each
        /// name.Note that if John and Jon are synonyms, and Jon and Johnny are synonyms, then John and
        /// Johnny are synonyms. (It is both transitive and symmetric.) In the final list, any name can be used
        /// as the "real" name.
        /// EXAMPLE
        /// Input:
        /// Names: John(15), Jon(12), Chris(13), Kris(4), Christopher(19)
        /// Synonyms: (Jon, John), (John, Johnny), (Chris, Kris), (Chris, Christopher)
        /// Output: John(27), Kris(36)
        /// Approach : Iterate over the synonyms and create a bidirectional braph for each node. By following this technique
        /// when you finish with the dictionary you will have distinct components of all the correlated names. For extra 
        /// explanation of the distinct components see the description above "components" local dictionary
        /// Then foreach distinct component, perform a BFS and foreach node check its frequency in the given NameFrequency dictionary.
        /// before you traverse the next graph component print the max occurency for the "real name" which is randomly the key of the 
        /// masterNode as we explained before.
        /// <Time>O(N+P) where N is the number of the pairs in synonyms dictionary and P is the number of every node in every created graph. max P = 2N</Time>
        /// <Space>O(N+P) as well because dict Components keeps the number of different components which in worst case is N (each pair directs to a different graph)
        /// and dictionary graphNodeCreated is storing every node in every graph which is P = 2N </Space>
        /// </summary>
        public static void BabyNames()
        {
            List<Tuple<string,string>> synonyms = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Jon","John" ),
                new Tuple<string, string>("John","Johnny" ),
                new Tuple<string, string>("Chris","Kris" ),
                new Tuple<string, string>("Chris","Christopher")
            };
            //keep the Key ("Name") and the reference of the GraphNode
            var graphNodeCreated = new Dictionary<string,GraphNode>();
            //The first pair which key and value has not already been created keep it as the master component and the reference to the node
            //example for meaning "Master Component" :
            /*
             *       Jon                 Chris
             *  Johnny  John          Kris     Christopher
             *  
             *  Jon and Chris are the the first keys of key,value pairs that have not  been added to the dictionary.
             *  They are gonna represent the headNodes so we can have a start point to traverse on distinct components.
             *  Left and Right graphs are two individual components
             *  
             */
            var components = new Dictionary<string, GraphNode>();

            Dictionary<string, int> NameFrequency = new Dictionary<string, int>
            { 
                {"Jon",10 },
                {"John",12 },
                {"Johnny",30},
                {"Chris",20 },
                {"Christopher",7 },
                { "Kris",12} 
            };

            foreach (var item in synonyms)
            {
                
                if (!graphNodeCreated.ContainsKey(item.Item1))
                {
                    var node = new GraphNode(item.Item1);
                    graphNodeCreated.Add(item.Item1,node);
                    if (!graphNodeCreated.ContainsKey(item.Item2))
                    {
                        var neighbor = new GraphNode(item.Item2);
                        node.adjacents.Add(neighbor);
                        neighbor.adjacents.Add(node);
                        graphNodeCreated.Add(item.Item2, neighbor);
                        //if its the first pair that does not exist, keep it as a new component
                        components.Add(item.Item1, node);
                    }
                }
                else
                {
                    if (!graphNodeCreated.ContainsKey(item.Item2))
                    {
                        var node = new GraphNode(item.Item2);
                        graphNodeCreated.Add(item.Item2,node);
                        var neighbor = graphNodeCreated[item.Item1];
                        node.adjacents.Add(neighbor);
                        neighbor.adjacents.Add(node);
                    }
                }
            }

            //components performs as a list of headNodes. Iterate over each graph and check the input frequencies
            foreach (var item in components)
            {
                int componentSum = 0;
                var headNode = item.Value;
                var queue = new Queue<GraphNode>();
                queue.Enqueue(headNode);
                while (queue.Count>0)
                {
                    var tempNode = queue.Dequeue();
                    if (!tempNode.visited)
                        componentSum += NameFrequency[tempNode.data];
                    tempNode.visited = true;
                    foreach (var neighboR in tempNode.adjacents)
                    {
                        if (!neighboR.visited)
                            queue.Enqueue(neighboR);
                    }
                }
                Console.WriteLine($"The main name is {item.Key} and its frequency is {componentSum}");
            }
            
        }
        /// <summary>
        /// Problem : 17.12
        /// Description : Consider a simple data structure called BiNode, which has pointers to two other nodes. The
        /// data structure BiNode could be used to represent both a binary tree(where node1 is the left node
        /// and node2 is the right node) or a doubly linked list(where node1 is the previous node and node2
        /// is the next node). Implement a method to convert a binary search tree(implemented with BiNode)
        /// into a doubly linked list.The values should be kept in order and the operation should be performed
        /// in place(that is, on the original data structure).
        /// Approach : Build the linked list by traversing with inorder in the left subtree and in the right subtree and merge the 2 sub-linkedlists
        /// you can use the BiNode class to create each node and from leftSubtree you're gonna retreive the "head" of the left sub-linkedList and from
        /// rightSubtree you're gonna retrieve the "tail" of the right sub-linkedList
        /// <Time>O(N) since you must touch each node</Time>
        /// </summary>
        public static void BiNodeFunc()
        {
            //tree creation
            BiNode root = new BiNode(10);
            root.left = new BiNode(2);
            root.left.left = new BiNode(1);
            root.left.right = new BiNode(7);
            root.right = new BiNode(15);
            root.right.left = new BiNode(12);
            root.right.right = new BiNode(30);
            //end creation

            var halfLeftlinkedList = BiNodeHelperLeftView(root.left);
            var halfRightlinkedList = BiNodeHelperRightView(root.right);
            root.left = halfLeftlinkedList;
            halfLeftlinkedList.right = root;

            root.right = halfRightlinkedList;
            halfRightlinkedList.left = root;

            //printning mid to left
            var node1 = root;
            while(node1!= null)
            {
                Console.WriteLine(node1.data);
                node1 = node1.left;
            }
            //printing mid to right
            var node2 = root.right;
            while(node2 != null)
            {
                Console.WriteLine(node2.data);
                node2 = node2.right;
            }
            //end printing
        }

        public static BiNode BiNodeHelperLeftView(BiNode node)
        {
            if (node == null)
                return null;
            var previousNode = BiNodeHelperLeftView(node.left);
            var linkedListNode = new BiNode(node.data);
            var nextNode = BiNodeHelperLeftView(node.right);

            if (previousNode != null)
            {
                linkedListNode.left = previousNode;
                previousNode.right = linkedListNode;
            }

            if (nextNode != null)
            {
                linkedListNode.right = nextNode;
                nextNode.left = linkedListNode;
            }

            return (linkedListNode.right!=null) ? linkedListNode.right : linkedListNode;
        }

        public static BiNode BiNodeHelperRightView(BiNode node)
        {
            if (node == null)
                return null;
            var previousNode = BiNodeHelperRightView(node.left);
            var linkedListNode = new BiNode(node.data);
            var nextNode = BiNodeHelperRightView(node.right);
            
            if (previousNode != null)
            { 
                linkedListNode.left = previousNode;
                previousNode.right = linkedListNode;
            }
            if (nextNode != null)
            {
                linkedListNode.right = nextNode;
                nextNode.left = linkedListNode;
            }

            return (linkedListNode.left != null) ? linkedListNode.left : linkedListNode;
        }

        /// <summary>
        /// Problem : 17.13
        /// Description : Oh, no! You have accidentally removed all spaces, punctuation, and capitalization in a
        /// lengthy document.A sentence like "I reset the c omputer. It still didn't boot!"
        /// became "iresetthec omputeri tstilldidntboot''. You'll deal with the punctuation and capitalization
        /// later; right now you need to re-insert the spaces.Most of the words are in a dictionary but
        /// a few are not.Given a dictionary(a list of strings) and the document(a string), design an algorithm
        /// to unconcatenate the document in a way that minimizes the number of unrecognized characters.
        /// EXAMPLE
        /// Input jesslookedjustliketimherbrother
        /// Output: jess looked just like tim her brother(7 unrecognized characters)
        /// Approach : 
        /// Iterate over the document. If your current character is not contained in the trie just continue, add it to your output stringBuild and increment the running index.
        /// Otherwise if it is contained check if in the HashMap that you keep the character and the list of the corresponding nodes if there exists a node which is a starting word (startsWith = true)
        /// Foreach of these prefixes perform the HelperFunction which does a BFS on the Trie and in parallel moves the index of the document. If you finish with Trie traverse and you find a 
        /// letter which is final word (isFinal = true) then return the incremented index for the last letter of the valid sentence. 
        /// If your returned index > -1 it means that you had a Trie Hit and you found a valid world. Prepend a space " " to your strinbuilder
        /// and until your running index reaches your returned index keep Appending letters from your input Doc into you stringbuilder.
        /// After that Append a final space " " and keep iterating over the remaining chars of the doc. 
        /// </summary>
        public static void ReSpace()
        {
            //Trie Seed
            var root = new TrieNode("*");
            root.children.Add(new TrieNode("l", true, false));

            root.children[0].children.Add(new TrieNode("o", false, false));
            root.children[0].children[0].children.Add(new TrieNode("o", false, false));
            root.children[0].children[0].children[0].children.Add(new TrieNode("k", false, true));

            root.children[0].children.Add(new TrieNode("i", false, false));
            root.children[0].children[1].children.Add(new TrieNode("k", false, false));
            root.children[0].children[1].children[0].children.Add(new TrieNode("e", false, true));

            root.children.Add(new TrieNode("h", true, false));
            root.children[1].children.Add(new TrieNode("e", false, false));
            root.children[1].children[0].children.Add(new TrieNode("e", false, true));

            root.children.Add(new TrieNode("b", true, false));
            root.children[2].children.Add(new TrieNode("r", false, false));
            root.children[2].children[0].children.Add(new TrieNode("o", false, false));
            root.children[2].children[0].children[0].children.Add(new TrieNode("t", false, false));
            root.children[2].children[0].children[0].children[0].children.Add(new TrieNode("h", false, false));
            root.children[2].children[0].children[0].children[0].children[0].children.Add(new TrieNode("e", false, false));
            root.children[2].children[0].children[0].children[0].children[0].children[0].children.Add(new TrieNode("r", false, true));
            //End of Seed
            var TrieMapper = new Dictionary<string, List<TrieNode>>
            {
                { "l", new List<TrieNode>{ root.children[0] } },
                { "o", new List<TrieNode>{root.children[0].children[0],root.children[0].children[0].children[0] ,root.children[2].children[0].children[0] } },
                { "k", new List<TrieNode>{ root.children[0].children[0].children[0].children[0], root.children[0].children[1].children[0] } },
                { "i", new List<TrieNode>{ root.children[0].children[1] } },
                { "e", new List<TrieNode>{ root.children[0].children[1].children[0].children[0], root.children[1].children[0].children[0], root.children[2].children[0].children[0].children[0].children[0].children[0] } },
                { "h", new List<TrieNode>{root.children[1], root.children[2].children[0].children[0].children[0].children[0] } },
                { "r", new List<TrieNode>{ root.children[1].children[0].children[0], root.children[2].children[0], root.children[2].children[0].children[0].children[0].children[0].children[0].children[0] } },
                { "b", new List<TrieNode>{ root.children[2] } },
                {"t", new List<TrieNode>{ root.children[2].children[0].children[0].children[0] } }
            };

            string document = "jesslookedjustliketimherbrother";
            int index = 0;
            StringBuilder sbOut = new StringBuilder();
            while (index<document.Length)
            {
                string toCheck = document[index].ToString();
                if (!TrieMapper.ContainsKey(toCheck))
                {
                    sbOut.Append(toCheck);
                    index++;
                }
                else
                {
                    var prefixes = TrieMapper[toCheck];
                    prefixes = prefixes.Where(node => node.startsWith == true)?.Select(it => it).ToList();
                    bool foundOne = false;
                    foreach (var prefix in prefixes)
                    {
                        int result = reSpaceHelper(prefix, index, document);
                        if (result == -1) continue;
                        else
                        {
                            sbOut.Append(" ");
                            while (index <= result)
                            {
                                sbOut.Append(document[index].ToString());
                                index++;
                            }
                            sbOut.Append(" ");
                            foundOne = true;
                            break;
                        }
                    }
                    if (!foundOne)
                    {
                        sbOut.Append(toCheck);
                        index++;
                    }
                }
            }
            string outputDoc = sbOut.ToString();
        }

        public static int reSpaceHelper(TrieNode prefix,int index, string document)
        {
            var queue = new Queue<TrieNode>();
            queue.Enqueue(prefix);
            bool flag = false;
            while (queue.Count>0)
            {
                var tempNode = queue.Dequeue();
                //If you found the word return the index
                if (tempNode.isFinal)
                {
                    flag = true; 
                    break;
                }

                if (tempNode.Sentence != document[index].ToString()) continue;
                else
                {
                    foreach (var neighbor in tempNode.children)
                    {
                        queue.Enqueue(neighbor);
                    }
                    index++;
                }
                
            }
            return (flag) ? index : -1;
        }
    }

    public class TrieNode
    {
        public bool isVisited;
        public string Sentence;
        public bool startsWith;
        public bool isFinal;
        public List<TrieNode> children;

        public TrieNode(string sentence,bool startsWith = false, bool isFinal = false)
        {
            this.Sentence = sentence;
            this.startsWith = startsWith;
            this.isFinal = isFinal;
            this.children = new List<TrieNode>();
            this.isVisited = false;
        }
    }

    public class GraphNode
    {
        public List<GraphNode> adjacents;
        public string data;
        public bool visited;

        public GraphNode(string data)
        {
            this.adjacents = new List<GraphNode>();
            this.data = data;
        }
    }

    public class BiNode
    {
        public int data;
        public BiNode left;
        public BiNode right;

        public BiNode(int data)
        {
            this.data = data;
        }
    }

    

}
