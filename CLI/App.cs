namespace CLI;

public class App(IController controller)
{
    public void Run()
    {
        Console.WriteLine("Welcome to Routine-Router!");

        while (true)
        {
            Console.WriteLine("Type 'exit' to exit");
            var input = Console.ReadLine();
            if (input == null)
                continue;
            if (input == "exit")
                return;


            controller.Handle(input);
        }
    }
}

