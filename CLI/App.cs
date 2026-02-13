namespace CLI;

public class App
{
    private readonly IController _controller;

    public App(IController controller)
        => _controller = controller;

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


            _controller.Handle(input);
        }
    }
}

