namespace MachineLoadProject;

public static class Program
{
    private static readonly Factory Factory = new ();
    
    public static void Main(string[] args)
    {
        Factory.Start();
        Factory.Process();
        Factory.PrintResults();
    }
}