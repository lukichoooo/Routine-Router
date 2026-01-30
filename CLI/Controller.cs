namespace CLI;

public static class Controller
{
    public static void Run()
    {
        Console.WriteLine("Welcome to Routine-Router!");

        while (true)
        {
            Console.WriteLine("Type 'exit' to exit");
            var input = Console.ReadLine();
            if (input == "exit")
                return;

        }
    }
}

