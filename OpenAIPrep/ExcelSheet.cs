using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;


namespace DeepDiveTechnicals.OpenAIPrep
{
    public sealed class ExcelSheet
    {
        private readonly ConcurrentDictionary<string, Cell> _sheets = new();
        private readonly ConcurrentDictionary<string, ImmutableList<Cell>> _inverseCellDependents = new();
        ///rivate readonly ConcurrentDictionary<string, ImmutableHashSet<string>> _parentToChildrenBookeeping = new(); good idea but gets too complex
        private readonly ConcurrentDictionary<string, int> _computedCellCached = new();

        public bool TrySetCell(string cellKey, string expr)
        {
            if (string.IsNullOrEmpty(cellKey))
            {
                throw new ArgumentException("Key cannot be null or empty;");
            }

            var (NodeExpression, inverseDependents) = TokenizeExpAndLocateDependents(expr);

            var cell = new Cell { Key = cellKey, Value = NodeExpression };
            
            var pendingTransaction = new List<Action>
            {
                () => _sheets[cellKey] = cell
            };

            foreach (var dependent in inverseDependents)
            {
                /*///Bookeeping
                if (_parentToChildrenBookeeping.TryGetValue(cellKey, out ImmutableHashSet<string> children))
                {
                    _parentToChildrenBookeeping[cellKey] = children.Add(dependent.Key);
                }
                else
                {
                    _parentToChildrenBookeeping.TryAdd(cellKey, [dependent.Key]);
                }
*/
                ///Inverse Dependencies
                if (_inverseCellDependents.TryGetValue(dependent.Key, out ImmutableList<Cell> value))
                {
                    pendingTransaction.Add(() => _inverseCellDependents[dependent.Key] = value.Add(cell));
                }
                else
                {
                    pendingTransaction.Add(() => _inverseCellDependents.TryAdd(dependent.Key, [cell]));
                    
                }
            }

            Dictionary<string, VisitState> cyclicalDetector = new()
            {
                { cellKey, VisitState.NotVisited}
            };

            if (ShouldInvalidateParents(cell, cyclicalDetector, ref pendingTransaction))
            {
                foreach(var trx in pendingTransaction)
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

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
            Node? topSign = null;
            Node? topSignWithinParentheses = null;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

            var finalOutput = 0;
            var localFinalOutputWithinParentheses = 0;

            var foundOpenParentheses = false;

            foreach (var node in currentCell.Value)
            {
                if (node is LiteralNode l)
                {
                    if (foundOpenParentheses)
                    {
                        UpdateOutputValue(ref topSignWithinParentheses, ref localFinalOutputWithinParentheses, l);
                        continue;
                    }

                    UpdateOutputValue(ref topSign, ref finalOutput, l);
                    continue;
                }
                else if (node is OpenParantheses)
                {
                    foundOpenParentheses = true;
                }
                else if (node is CloseParantheses)
                {
                    foundOpenParentheses = false; //reset signal
                    UpdateOutputValue(ref topSign, ref finalOutput, new LiteralNode { Value = localFinalOutputWithinParentheses });
                    continue;
                }
                else if (IsSign(node))
                {
                    if (foundOpenParentheses)
                    {
                        topSignWithinParentheses = node;
                        continue;
                    }

                    topSign = node;
                    continue;
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
                        if (foundOpenParentheses)
                        {
                            UpdateOutputValue(ref topSignWithinParentheses, ref localFinalOutputWithinParentheses, new LiteralNode { Value = _computedCellCached[refNode.CellKey] }); // MEMO
                            continue;
                        }

                        UpdateOutputValue(ref topSign, ref finalOutput, new LiteralNode { Value = _computedCellCached[refNode.CellKey] }); // MEMO
                        continue;
                    }

                    var evaluatedCell = EvaluateCell(_sheets[refNode.CellKey], cyclicalDetector);

                    if (foundOpenParentheses)
                    {
                        UpdateOutputValue(ref topSignWithinParentheses, ref localFinalOutputWithinParentheses, new LiteralNode { Value = evaluatedCell });
                    }
                    else
                    {
                        UpdateOutputValue(ref topSign, ref finalOutput, new LiteralNode { Value = evaluatedCell });
                    }
                }
            }

            cyclicalDetector[currentCell.Key] = VisitState.Visited;
            if (foundOpenParentheses)
            {
                _computedCellCached[currentCell.Key] = localFinalOutputWithinParentheses;
                return localFinalOutputWithinParentheses;
            }
            
            _computedCellCached[currentCell.Key] = finalOutput;
            return finalOutput;
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
            else // already visited
            {
                return true;
            }

            // reset
            pendingTransaction.Add(() => _computedCellCached.TryRemove(cell.Key, out _));

            if (_inverseCellDependents.ContainsKey(cell.Key))
            {
                foreach (var parent in _inverseCellDependents[cell.Key])
                {
                    ShouldInvalidateParents(parent, cyclicalDetector, ref pendingTransaction);
                }
            }

            return true;
        }

        private static bool IsSign(Node node) => node is PlusNode || node is MinusNode || node is MultiplyNode || node is DivideNode;

        private static void UpdateOutputValue(ref Node topSign, ref int finalOutput, LiteralNode l)
        {
            if (topSign is PlusNode)
            {
                // reset
                topSign = null;
                finalOutput += l.Value;
            }
            else if (topSign is MinusNode)
            {
                // reset
                topSign = null;
                finalOutput -= l.Value;
            }
            else if (topSign is MultiplyNode)
            {
                // reset
                topSign = null;
                finalOutput *= l.Value;
            }
            else if (topSign is DivideNode)
            {
                // reset
                topSign = null;
                finalOutput /= l.Value;
            }
            else
            {
                // this is the first case and there is no preceding sign
                finalOutput = l.Value;
            }
        }

        private (List<Node> NodeExpression, List<Cell> Dependents) TokenizeExpAndLocateDependents(string expression)
        {
            // examples
            // A1 + B1 
            // -6
            // A1 + 76 -6 + B1

            var nodesOutput = new List<Node>();
            var dependentCells = new List<Cell>();

            (bool Semaphore, StringBuilder Builder) nodeBuilder = new(false, new StringBuilder());

            foreach (var ch in expression.Trim())
            {
                if (ch == ' ')
                {
                    continue; // just skip spaces
                }
                if (ch == '(')
                {
                    if (nodeBuilder.Builder.Length > 0)
                    {
                        nodeBuilder = Flush(ref nodesOutput, ref dependentCells, nodeBuilder.Semaphore, nodeBuilder.Builder);
                    }

                    nodesOutput.Add(new OpenParantheses());
                }
                else if (ch == ')')
                {
                    if (nodeBuilder.Builder.Length > 0)
                    {
                        nodeBuilder = Flush(ref nodesOutput, ref dependentCells, nodeBuilder.Semaphore, nodeBuilder.Builder);
                    }

                    nodesOutput.Add(new CloseParantheses());
                }
                else if (ch == '+')
                {
                    if (nodeBuilder.Builder.Length > 0)
                    {
                        nodeBuilder = Flush(ref nodesOutput, ref dependentCells, nodeBuilder.Semaphore, nodeBuilder.Builder);
                    }

                    nodesOutput.Add(new PlusNode());
                }
                else if (ch == '-')
                {
                    if (nodeBuilder.Builder.Length > 0)
                    {
                        nodeBuilder = Flush(ref nodesOutput, ref dependentCells, nodeBuilder.Semaphore, nodeBuilder.Builder);
                    }

                    nodesOutput.Add(new MinusNode());
                }
                else if (ch == '/')
                {
                    if (nodeBuilder.Builder.Length > 0)
                    {
                        nodeBuilder = Flush(ref nodesOutput, ref dependentCells, nodeBuilder.Semaphore, nodeBuilder.Builder);
                    }

                    nodesOutput.Add(new DivideNode());
                }
                else if (ch == '*')
                {
                    if (nodeBuilder.Builder.Length > 0)
                    {
                        nodeBuilder = Flush(ref nodesOutput, ref dependentCells, nodeBuilder.Semaphore, nodeBuilder.Builder);
                    }

                    nodesOutput.Add(new MultiplyNode());
                }
                else if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'))
                {
                    nodeBuilder.Builder.Append(ch);
                    nodeBuilder.Semaphore = true;
                }
                else if (int.TryParse(ch.ToString(), CultureInfo.InvariantCulture, out var number))
                {
                    nodeBuilder.Builder.Append(number);
                }
                else
                {
                    throw new ArgumentException($"Unrecognized character {ch}");
                }
            }

            if (nodeBuilder.Builder.Length > 0)
            {
                nodeBuilder = Flush(ref nodesOutput, ref dependentCells, nodeBuilder.Semaphore, nodeBuilder.Builder);
            }

            return (nodesOutput, dependentCells);
        }

        private (bool Semaphore, StringBuilder Builder) Flush(
            ref List<Node> nodes,
            ref List<Cell> dependents,
            bool semaphore,
            StringBuilder builder)
        {
            if (semaphore)
            {
                var dependentCellKey = builder.ToString();
                nodes.Add(new RefNode { CellKey =  dependentCellKey });
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

        private sealed class PlusNode : Node { }

        private sealed class MinusNode : Node { }

        private sealed class  MultiplyNode : Node { }
        
        private sealed class DivideNode : Node { }

        private sealed class OpenParantheses : Node {}

        private sealed class CloseParantheses : Node { }

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
            excelSheet.TrySetCell("A1", "B1");
        }
    }
}
