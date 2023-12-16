using System.Diagnostics;
using Antlr4.Runtime.Misc;

namespace MyExcel
{
	public class GrammarVisitor: GrammarBaseVisitor<double>
	{
        public override double VisitCompileUnit(GrammarParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitNumberExpr(GrammarParser.NumberExprContext context)
        {
            var result = double.Parse(context.GetText());
            Debug.WriteLine(result);

            return result;
        }

        public override double VisitParenthesizedExpr(GrammarParser.ParenthesizedExprContext context)
        {
            return Visit(context.expression());
        }

        //IdentifierExpr
        public override double VisitIdentifierExpr(GrammarParser.IdentifierExprContext context)
        {
            var identifier = context.GetText();
            Table.cells[Calculator.EvaluatingCellName].DependsOn.Add(identifier);
            Table.cells[identifier].AppearsIn.Add(Calculator.EvaluatingCellName);
            return Table.GetValue(identifier);
        }


        public override double VisitExponentialExpr(GrammarParser.ExponentialExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            Debug.WriteLine("{0} ^ {1}", left, right);
            return System.Math.Pow(left, right);
        }


        public override double VisitUnaryExpr([NotNull] GrammarParser.UnaryExprContext context)
        {
            var left = WalkLeft(context);

            if (context.operatorToken.Type == GrammarLexer.PLUS)
                return left;
            else return -left;
        }

        public override double VisitMinExpr(GrammarParser.MinExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            Debug.WriteLine("min({0}, {1})", left, right);
            return System.Math.Min(left, right);
        }

        public override double VisitMaxExpr(GrammarParser.MaxExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            Debug.WriteLine("max({0}, {1})", left, right);
            return System.Math.Max(left, right);
        }

        public override double VisitMMinExpr(GrammarParser.MMinExprContext context)
        {
            double minValue = Double.PositiveInfinity;
            foreach (var child in context.paramlist.children.OfType<GrammarParser.ExpressionContext>())
            {
                double childValue = this.Visit(child);
                if (childValue < minValue)
                {
                    minValue = childValue;
                }
            }
            return minValue;
        }

        public override double VisitMMaxExpr(GrammarParser.MMaxExprContext context)
        {
            double maxValue = Double.NegativeInfinity;
            foreach (var child in context.paramlist.children.OfType<GrammarParser.ExpressionContext>())
            {
                double childValue = this.Visit(child);
                if (childValue > maxValue)
                {
                    maxValue = childValue;
                }
            }
            return maxValue;
        }

        private double WalkLeft(GrammarParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<GrammarParser.ExpressionContext>(0));
        }

        private double WalkRight(GrammarParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<GrammarParser.ExpressionContext>(1));
        }
    }
}

