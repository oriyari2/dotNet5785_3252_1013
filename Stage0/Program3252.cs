partial class Program
{
    private static void Main(string[] args)
    {
        Welcome3252();
        Welcome1013();
        Console.ReadKey();
    }

    private static void Welcome3252()
    {
        Console.Write("Enter your name: ");
        string? name = Console.ReadLine();//we add ? in order to prevent problems of enter null to string
        Console.WriteLine("{0}, welcome to my first console application", name);
    }
    static partial void Welcome1013();
    
}