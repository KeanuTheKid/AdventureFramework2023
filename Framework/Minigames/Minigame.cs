
namespace Framework.Minigames.MinigameDefClasses;

public class GarbageCollect : MinigameDefBase
{

    public override string BackgroundImage { get; set; } = "/GarbageCollect/NPC_1.JPG"; // Background Image


    //DIALOGUE STUFF
    private Image QuitButton { get; set; }
    private Image ForwardButton { get; set; }

    private Dialogue dialogue;

    //MINIGAME STUFF
    [Element]
    public required Rectangle NpcHitBox { get; set; }

    List<List<string>> messages = [
        ["player", "Hello"],
        ["npc", "bring me something"],
        ["player", "Hello"],
        ["npc", "did you bring it?"],

    ];

    public GarbageCollect()
    {

        dialogue = new Dialogue(messages);

        NpcHitBox = new()
        {
            X = 500,
            Y = 30,
            Width = 400,
            Height = 1000,
            Fill = "green",
            Opacity = 0.1,
            OnClick = async (args) => { Console.WriteLine("Click On NPC"); await StartDialogueAsync();/* Console.WriteLine(NpcHitBox.Opacity)*/}
        };

    }

    public async Task StartDialogueAsync()
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

    private async Task WaitForConditionAsync(Func<bool> condition)
    {
        while (!condition())
        {
            await Task.Delay(100); // Check the condition every 100 milliseconds
        }
    }



}