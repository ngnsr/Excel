namespace MyExcel.Tests;

[TestClass]
public class BasicArithmeticTests
{

    [TestMethod]
    public void PowerTest()
    {
        Assert.AreEqual(Calculator.Evaluate("2^10"), 1024);
    }

    [TestMethod]
    public void VariablePowerTest()
    {
        Table.AddNewEntry("A1");
        Table.cells.Add("A2", new Cell(2));
        Table.cells.Add("A3", new Cell(10));
        Calculator.EvaluatingCellName = "A1";
        Assert.AreEqual(Calculator.Evaluate("A2^A3"), 1024);
        Table.cells.Remove("A1");
        Table.cells.Remove("A2");
        Table.cells.Remove("A3");
        Calculator.EvaluatingCellName = null;
    }

    [TestMethod]
    public void maxTest()
    {
        Assert.AreEqual(Calculator.Evaluate("max(11, 13)"), 13);
    }

    [TestMethod]
    public void minTest()
    {
        Assert.AreEqual(Calculator.Evaluate("min(1, -1)"), -1);
    }

    [TestMethod]
    public void mmaxTest()
    {
        Assert.AreEqual(Calculator.Evaluate("mmax(1, 2, 3, 4, 10)"), 10);
    }

    [TestMethod]
    public void mminTest()
    {
        Assert.AreEqual(Calculator.Evaluate("mmin(-11, 12, -15, 19, 10)"), -15);
    }

    [TestMethod]
    public void UnaryTest()
    {
        Table.cells.Add("A1", new Cell(10));
        Table.AddNewEntry("A2");
        Calculator.EvaluatingCellName = "A2";
        double a = Calculator.Evaluate("-A1");
        double b = Calculator.Evaluate("A1");
        Assert.AreEqual(a, -10);
        Assert.AreEqual(b, 10);
        Table.cells.Remove("A1");
        Table.cells.Remove("A2");
        Calculator.EvaluatingCellName = null;   
    }

    [TestMethod]
    public void ExceptionTest()
    {
        Exception expectedException = null;
        try
        {
            Calculator.Evaluate("min(,)");
        }
        catch (Exception e)
        {
            expectedException = e;
        }
        Assert.IsNotNull(expectedException);
    }

    [TestMethod]
    public void UnsuportedOperationTest()
    {
        Exception expectedException = null;
        try
        {
            Calculator.Evaluate("4/2");
        }
        catch (Exception e)
        {
            expectedException = e;
        }
        Assert.IsNotNull(expectedException);
    }

    [TestMethod]
    public void ParenthesesPriorityTest()
    {   
        var r = Calculator.Evaluate("(-max(2,0))^3");
        
        Assert.AreEqual(r, -8);
    }

    [TestMethod]
    public void CompostitionTest()
    {
        var r = Calculator.Evaluate("min(max(1,2), max(-1,-2))");

        Assert.AreEqual(r, -1);
    }

    [TestMethod]
    public void VariableCompostitionTest()
    {
        Table.cells.Add("A1", new Cell(1));
        Table.cells.Add("A2", new Cell(2));
        Table.AddNewEntry("A3");
        Calculator.EvaluatingCellName = "A3";
        var r = Calculator.Evaluate("min(max(A1,A2), max(-A1,-A2))");

        Assert.AreEqual(r, -1);
        Table.cells.Remove("A1");
        Table.cells.Remove("A2");
        Table.cells.Remove("A3");
        Calculator.EvaluatingCellName = null;
    }
}