
namespace Framework.Minigames.MinigameDefClasses;

public class GarbageCollect : MinigameDefBase
{

    public override string BackgroundImage { get; set; } = "/GarbageCollect/NPC_1.JPG"; // Background Image


    //DIALOGUE STUFF
    private Image QuitButton { get; set; }
    private Image ForwardButton { get; set; }

    private Dialogue startdialogue;
    private Dialogue finishdialogue;

    //MINIGAME STUFF
    [Element]
    public required Rectangle NpcHitBox { get; set; }
    [Element]
    public required Rectangle table1 { get; set; }
    [Element]
    public required Rectangle table2 { get; set; }
    [Element]
    public required Rectangle table3 { get; set; }
    [Element]
    public required Rectangle back { get; set; }

    List<List<string>> start = [
        ["Player", "Alles OK?"],
        ["2M Schüler", "NEIN, ich muss all die Tische putzen"],
        ["Player", "Schlecht für Dich"],
        ["NPC", "Wenn du mir hilfst kriegst du ein RedBull"],
        ["Player", "Dann helfe ich natürlich"],
        ["NPC", "Gut, dann sammel all den Müll von den Tischen und komm dann zurück zu mir!"],
        ["Player", "Alles klar!"],

    ];

    List<List<string>> finish = [ //ANPASSEN
        ["Player", "Alles OK?"],
        ["2M Schüler", "NEIN, ich muss all die Tische putzen"],
        ["Player", "Schlecht für Dich"],
        ["NPC", "Wenn du mir hilfst kriegst du ein RedBull"],
        ["Player", "Dann helfe ich natürlich"],
        ["NPC", "Gut, dann sammel all den Müll von den Tischen und komm dann zurück zu mir!"],
        ["Player", "Alles klar!"],

    ];

    public GarbageCollect()
    {

        startdialogue = new Dialogue(start);
        finishdialogue = new Dialogue(finish);
        bool itemscollected = true; //function for checking if all the thrash has been collected from the tables

        NpcHitBox = new()
        {
            X = 500,
            Y = 30,
            Width = 400,
            Height = 1000,
            Fill = "green",
            FillOpacity = 0.1,
            OnClick = async (args) =>
            {
                if (itemscollected)
                {
                    await FinishMissionDialogueAsync();
                    //add redbull here
                }
                else
                {
                    await StartMissionDialogueAsync();
                }
            }
        };

        table1 = new()
        {
            X = 500,
            Y = 1000,
            Width = 50,
            Height = 50,
            Fill = "green",
            FillOpacity = 1,
            OnClick = (args) => { Console.WriteLine("Click On NPC"); }
        };
        table2 = new()
        {
            X = 570,
            Y = 1000,
            Width = 50,
            Height = 50,
            Fill = "blue",
            FillOpacity = 1,
            OnClick = (args) => { Console.WriteLine("Click On NPC"); }
        };
        table3 = new()
        {
            X = 640,
            Y = 1000,
            Width = 50,
            Height = 50,
            Fill = "yellow",
            FillOpacity = 1,
            OnClick = (args) => { Console.WriteLine("Click On NPC"); }
        };
        back = new()
        {
            X = 710,
            Y = 1000,
            Width = 50,
            Height = 50,
            Fill = "red",
            FillOpacity = 1,
            OnClick = (args) => { Console.WriteLine("Click On NPC"); }
        };

    }

    public async Task FinishMissionDialogueAsync()
    {

        bool quit = false;
        bool forward = false;

        //Create forward and quit button
        QuitButton = dialogue.DrawQuitButton();
        ForwardButton = dialogue.DrawForwardButton();

        AddElement(QuitButton);
        AddElement(ForwardButton);

        QuitButton.OnClick = (args) => { quit = true; Console.WriteLine("Quit"); };
        ForwardButton.OnClick = (args) => { forward = true; Console.WriteLine("Forward"); };

        Update();


        foreach (List<string> speech in messages)
        {

            GameObjectContainer<SVGElement> Bubble = dialogue.DrawSpeechBubble(speech[0], speech[1]);

            AddElementsInContainer(Bubble);

            Update();

            await WaitForConditionAsync(() => forward || quit);

            foreach (string key in Bubble.Keys)
            {
                Elements.Remove(key);
            }

            if (quit == true)
            {

                Update();
                break;
            }

            forward = false;

            Update();
        }


    }

    public async Task StartMissionDialogueAsync()
    {

        bool quit = false;
        bool forward = false;

        //Create forward and quit button
        QuitButton = dialogue.DrawQuitButton();
        ForwardButton = dialogue.DrawForwardButton();

        AddElement(QuitButton);
        AddElement(ForwardButton);

        QuitButton.OnClick = (args) => { quit = true; Console.WriteLine("Quit"); };
        ForwardButton.OnClick = (args) => { forward = true; Console.WriteLine("Forward"); };

        Update();


        foreach (List<string> speech in start)
        {

            GameObjectContainer<SVGElement> Bubble = dialogue.DrawSpeechBubble(speech[0], speech[1]);

            AddElementsInContainer(Bubble);

            Update();

            await WaitForConditionAsync(() => forward || quit);

            foreach (string key in Bubble.Keys)
            {
                Elements.Remove(key);
            }

            if (quit == true)
            {

                Update();
                break;
            }

            forward = false;

            Update();
        }


    }


    private async Task WaitForConditionAsync(Func<bool> condition)
    {
        while (!condition())
        {
            await Task.Delay(100); // Check the condition every 100 milliseconds
        }
    }



}