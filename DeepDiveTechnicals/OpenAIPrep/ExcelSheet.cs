using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;


namespace DeepDiveTechnicals.OpenAIPrep
{
    public sealed class ExcelSheet
    {
        private readonly ConcurrentDictionary<string, Cell> _sheets = new();
        private readonly ConcurrentDictionary<string, ImmutableList<Cell>> _inverseCellDependents = new();
        private readonly ConcurrentDictionary<string, ImmutableHashSet<string>> _forwardDependencies = new();
        private readonly ConcurrentDictionary<string, int> _computedCellCached = new();

        public bool TrySetCell(string cellKey, string expr)
        {
            if (string.IsNullOrEmpty(cellKey))
            {
                throw new ArgumentException("Key cannot be null or empty;");
            }

            var (RPN, dependentCells) = ExpressionToRPNAndLocateDependents(expr);

            var cell = new Cell { Key = cellKey, Value = RPN };

            var pendingTransaction = new List<Action>
            {
                () => _sheets[cellKey] = cell
            };

            _forwardDependencies.TryGetValue(cellKey, out var existingForwardDependents);
            var newForwardDependents = new HashSet<string>();

            foreach (var dependent in dependentCells)
            {
                /// Forward Dependencies
                newForwardDependents.Add(dependent.Key);
                
                /// Inverse Dependencies
                if (_inverseCellDependents.TryGetValue(dependent.Key, out ImmutableList<Cell> value))
                {
                    pendingTransaction.Add(() => _inverseCellDependents[dependent.Key] = value.Add(cell));
                }
                else
                {
                    pendingTransaction.Add(() => _inverseCellDependents.TryAdd(dependent.Key, [cell]));

                }
            }

            var delta = existingForwardDependents?.Where(item => !newForwardDependents.Contains(item)) ?? [];
            foreach(var redundantInverseDependency in delta)
            {
                if (_inverseCellDependents.TryGetValue(redundantInverseDependency, out var cells))
                {
                    pendingTransaction.Add(() => _inverseCellDependents[redundantInverseDependency] = [.. cells.Where(parentCell => parentCell.Key != cellKey)]);
                }
            }

            pendingTransaction.Add(() => _forwardDependencies[cellKey] = newForwardDependents.ToImmutableHashSet());

            Dictionary<string, VisitState> cyclicalDetector = new()
            {
                { cellKey, VisitState.NotVisited}
            };

            if (ShouldInvalidateParents(cell, cyclicalDetector, ref pendingTransaction))
            {
                /// This is good only for single-threaded. We will use semaphoreSlim for locking per key so it can be ACID.
                foreach (var trx in pendingTransaction)
                {
                    trx.Invoke();
                }

                return true;
            }

            return false;
        }

        // if cell not set return 0
        // throw on circular dependencies
        public int GetCell(string cell)
        {
            if (!_sheets.ContainsKey(cell))
            {
                return 0;
            }

            var currentCell = _sheets[cell];

            if (currentCell.Value.Count == 0)
            {
                _computedCellCached[cell] = 0;
                return 0;
            }

            // holds pair from->to
            // example AB means from A visited B
            // example BA means from B visited A
            Dictionary<string, VisitState> cyclicalDetector = new()
            {
                { cell, VisitState.NotVisited}
            };

            if (_computedCellCached.TryGetValue(cell, out int cellValue))
            {
                return cellValue;
            }

            cellValue = EvaluateCell(currentCell, cyclicalDetector);

            return cellValue;
        }

        private int EvaluateCell(Cell currentCell, Dictionary<string, VisitState> cyclicalDetector)
        {
            if (!cyclicalDetector.ContainsKey(currentCell.Key) || cyclicalDetector[currentCell.Key] == VisitState.NotVisited)
            {
                cyclicalDetector[currentCell.Key] = VisitState.Visiting;
            }
            else if (cyclicalDetector[currentCell.Key] == VisitState.Visiting)
            {
                throw new ArgumentException("Cyclical Dependency Detected!");
            }
            else // already visited
            {
                return _computedCellCached[currentCell.Key];
            }

            var rpnEvaluated = new Stack<int>();
            
            foreach (var node in currentCell.Value)
            {
                if (node is LiteralNode l)
                {
                    rpnEvaluated.Push(l.Value);
                    continue;
                }
                
                if (node is OpenParantheses)
                {
                    throw new Exception($"RPN wrongly calculated, {node} is not supported during evaluation");
                }
                else if (node is CloseParantheses)
                {
                    throw new Exception($"RPN wrongly calculated, {node} is not supported during evaluation");
                }


                if (IsSign(node))
                {
                    if (rpnEvaluated.Count < 2)
                    {
                        throw new Exception($"RPN wrongly evaluated, cannot apply {node}.");
                    }

                    var b = rpnEvaluated.Pop();
                    var a = rpnEvaluated.Pop();

                    rpnEvaluated.Push(ApplyOperator(node, a, b));
                }
                else
                {
                    var refNode = node as RefNode;
                    // it must be a reference
                    if (!_sheets.ContainsKey(refNode.CellKey))
                    {
                        // key has not been set
                        continue;
                    }
                    if (_computedCellCached.ContainsKey(refNode.CellKey))
                    {
                        rpnEvaluated.Push(_computedCellCached[refNode.CellKey]);// MEMO
                        continue;
                    }

                    var evaluatedCell = EvaluateCell(_sheets[refNode.CellKey], cyclicalDetector);
                    rpnEvaluated.Push(evaluatedCell);
                }
            }

            cyclicalDetector[currentCell.Key] = VisitState.Visited;

            if (rpnEvaluated.Count > 1)
            {
                throw new Exception("RPN not evaluted correctly. It has more than one outputs.");
            }

            var output = rpnEvaluated.Pop();
            _computedCellCached[currentCell.Key] = output;
            return output;
        }

        /// <summary>
        ///  Simple DFS cache invalidation
        /// </summary>
        private bool ShouldInvalidateParents(Cell cell, Dictionary<string, VisitState> cyclicalDetector, ref List<Action> pendingTransaction)
        {
            if (!cyclicalDetector.ContainsKey(cell.Key) || cyclicalDetector[cell.Key] == VisitState.NotVisited)
            {
                cyclicalDetector[cell.Key] = VisitState.Visiting;
            }
            else if (cyclicalDetector[cell.Key] == VisitState.Visiting)
            {
                Console.Write("Cyclical Dependency Detected upon Set, exit.");
                return false;
            }

            // reset
            pendingTransaction.Add(() => _computedCellCached.TryRemove(cell.Key, out _));

            if (_inverseCellDependents.ContainsKey(cell.Key))
            {
                foreach (var parent in _inverseCellDependents[cell.Key])
                {
                    if(!ShouldInvalidateParents(parent, cyclicalDetector, ref pendingTransaction))
                    {
                        return false; // somewhere found at least one cyclical dependency. Skip invalidation
                    }
                }
            }

            return true;
        }

        private static bool IsSign(Node node) => node is PlusNode || node is MinusNode || node is MultiplyNode || node is DivideNode;

        private static int ApplyOperator(Node oper, int a, int b)
        {
            if (oper is PlusNode)
            {
                // reset
                return a + b;
            }
            else if (oper is MinusNode)
            {
                // reset
                return a - b;
            }
            else if (oper is MultiplyNode)
            {
                // reset
                return a * b;
            }
            else if (oper is DivideNode)
            {
                // reset
                return a / b;
            }
            else
            {
                // this is the first case and there is no preceding sign
                throw new Exception("Not supported operator");
            }
        }

        private (List<Node> PRN, List<Cell> Dependents) ExpressionToRPNAndLocateDependents(string expression)
        {
            // examples
            // A1 + B1 
            // -6
            // A1 + 76 -6 + B1
            var operations = new Stack<Operator>();
            var nodesOutput = new List<Node>();
            var dependentCells = new List<Cell>();

            (bool Semaphore, StringBuilder Builder) currentRefOrLiteralBuilder = new(false, new StringBuilder());

            foreach (var ch in expression)
            {
                if (ch == ' ')
                {
                    continue; // just skip spaces
                }
                if (ch == '(')
                {
                    if (currentRefOrLiteralBuilder.Builder.Length > 0)
                    {
                        currentRefOrLiteralBuilder = FlushNodeBuilder(ref nodesOutput, ref dependentCells, currentRefOrLiteralBuilder.Semaphore, currentRefOrLiteralBuilder.Builder);
                    }

                    operations.Push(new OpenParantheses());
                }
                else if (ch == ')')
                {
                    if (currentRefOrLiteralBuilder.Builder.Length > 0)
                    {
                        currentRefOrLiteralBuilder = FlushNodeBuilder(ref nodesOutput, ref dependentCells, currentRefOrLiteralBuilder.Semaphore, currentRefOrLiteralBuilder.Builder);
                    }

                    while (operations.Count > 0)
                    {
                        var opTop = operations.Pop();
                        if (opTop is OpenParantheses)
                        {
                            break;
                        }
                        else
                        {
                            nodesOutput.Add(opTop);
                        }
                    }
                }
                else if (ch == '+')
                {
                    if (currentRefOrLiteralBuilder.Builder.Length > 0)
                    {
                        currentRefOrLiteralBuilder = FlushNodeBuilder(ref nodesOutput, ref dependentCells, currentRefOrLiteralBuilder.Semaphore, currentRefOrLiteralBuilder.Builder);
                    }

                    var plus = new PlusNode();
                    RPNHousekeepOperators(plus, ref nodesOutput, ref operations);
                }
                else if (ch == '-')
                {
                    if (currentRefOrLiteralBuilder.Builder.Length > 0)
                    {
                        currentRefOrLiteralBuilder = FlushNodeBuilder(ref nodesOutput, ref dependentCells, currentRefOrLiteralBuilder.Semaphore, currentRefOrLiteralBuilder.Builder);
                    }

                    var plus = new MinusNode();
                    RPNHousekeepOperators(plus, ref nodesOutput, ref operations);
                }
                else if (ch == '/')
                {
                    if (currentRefOrLiteralBuilder.Builder.Length > 0)
                    {
                        currentRefOrLiteralBuilder = FlushNodeBuilder(ref nodesOutput, ref dependentCells, currentRefOrLiteralBuilder.Semaphore, currentRefOrLiteralBuilder.Builder);
                    }

                    var plus = new DivideNode();
                    RPNHousekeepOperators(plus, ref nodesOutput, ref operations);
                }
                else if (ch == '*')
                {
                    if (currentRefOrLiteralBuilder.Builder.Length > 0)
                    {
                        currentRefOrLiteralBuilder = FlushNodeBuilder(ref nodesOutput, ref dependentCells, currentRefOrLiteralBuilder.Semaphore, currentRefOrLiteralBuilder.Builder);
                    }

                    var plus = new MultiplyNode();
                    RPNHousekeepOperators(plus, ref nodesOutput, ref operations);
                }
                else if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'))
                {
                    currentRefOrLiteralBuilder.Builder.Append(ch);
                    currentRefOrLiteralBuilder.Semaphore = true;
                }
                else if (int.TryParse(ch.ToString(), CultureInfo.InvariantCulture, out var number))
                {
                    currentRefOrLiteralBuilder.Builder.Append(number);
                }
                else
                {
                    throw new ArgumentException($"Unrecognized character {ch}");
                }
            }

            if (currentRefOrLiteralBuilder.Builder.Length > 0)
            {
                currentRefOrLiteralBuilder = FlushNodeBuilder(ref nodesOutput, ref dependentCells, currentRefOrLiteralBuilder.Semaphore, currentRefOrLiteralBuilder.Builder);
            }

            while (operations.Count >0)
            {
                // add remaining operations
                nodesOutput.Add(operations.Pop());
            }

            return (nodesOutput, dependentCells);
        }

        private void RPNHousekeepOperators(Operator currentOperatorNode, ref List<Node> nodesOutput, ref Stack<Operator> operations)
        {
            while (operations.Count > 0 && (operations.Peek().Priority >= currentOperatorNode.Priority))
            {
                nodesOutput.Add(operations.Pop());
            }
            operations.Push(currentOperatorNode);
        }

        private (bool Semaphore, StringBuilder Builder) FlushNodeBuilder(
            ref List<Node> nodes,
            ref List<Cell> dependents,
            bool semaphore,
            StringBuilder builder)
        {
            if (semaphore)
            {
                var dependentCellKey = builder.ToString();
                nodes.Add(new RefNode { CellKey = dependentCellKey });
                if (_sheets.ContainsKey(dependentCellKey))
                {
                    dependents.Add(_sheets[dependentCellKey]);
                }
                else
                {
                    /// cell not yet initialized
                    dependents.Add(new Cell { Key = dependentCellKey });
                }
            }
            else
            {
                nodes.Add(new LiteralNode { Value = Convert.ToInt32(builder.ToString()) });
            }

            return new(false, new StringBuilder());
        }

        private sealed class RefNode : Node
        {
            public string CellKey { get; set; }
        }

        private sealed class LiteralNode : Node
        {
            public int Value { get; set; }
        }

        private sealed class PlusNode : Operator { public override int Priority { get; set; } = 1; }

        private sealed class MinusNode : Operator { public override int Priority { get; set; } = 1; }

        private sealed class MultiplyNode : Operator { public override int Priority { get; set; } = 2; }

        private sealed class DivideNode : Operator { public override int Priority { get; set; } = 2; }

        private sealed class OpenParantheses : Operator { public override int Priority { get; set; } = 0; }

        private sealed class CloseParantheses : Operator { public override int Priority { get; set; } = 0; }

        private abstract class Operator : Node
        {
            public abstract int Priority { get; set; }
        }

        private abstract class Node{ }

        private sealed class Cell
        {
            public string Key { get; set; }

            public List<Node> Value { get; set; }
        }

        public enum VisitState
        {
            NotVisited,
            Visiting,
            Visited
        }
    }

    public class ExcelSheetTests
    {
        public void ExcelSheet_VariousOperations_Handled()
        {
            var excelSheet = new ExcelSheet();
            excelSheet.TrySetCell("A1", "1+2");
            var A1 = excelSheet.GetCell("A1"); // should be 3
            excelSheet.TrySetCell("B1", "10*A1");
            var B1 = excelSheet.GetCell("B1"); // should be 30

            excelSheet.TrySetCell("C1", "(B1-A1)/3*(1+0)"); 
            var C1 = excelSheet.GetCell("C1"); // should be (30-3)/3*(1) = 9

            excelSheet.TrySetCell("D1", "(4-1*(7-6))/4");
            var D1 = excelSheet.GetCell("D1"); // should be 0 as we do not handle floating point and (4-1)/4 has div 0 ==> 0

            excelSheet.TrySetCell("B1", "D1");
            B1 = excelSheet.GetCell("B1"); // should be 0

            excelSheet.TrySetCell("A1", "B1"); // cyclical, return false and invalidate nothing
        }
    }
}
