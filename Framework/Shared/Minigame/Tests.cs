
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace Framework.Minigames.MinigameDefClasses;


public class MyMinigame6 : MinigameDefBase
{


	public override string BackgroundImage { get; set; } = "/images/HM3_hallwayN.jpg";

	[Element]
	public Rectangle Rect { get; set; }

	public Rectangle Rect1 { get; set; }
	public GameObjectContainer<Rectangle> movingrects { get; set; } = new();

	public GameObjectContainer<Rectangle> rects { get; set; } = new();

	public MyMinigame6()
	{
		Rect = new()
		{
			Id = "rect",
			X = 100,
			Y = 100,
			Width = 100,
			Height = 100,
			Fill = "green",
			OnClick = spawnRect,


		};
		// AddElement(Rect);
		rects.Add(Rect);



		Update();
	}


	public void spawnRect(EventArgs args)
	{

		Random rnd = new Random();

		int randomx = rnd.Next(1, 1900);
		int randomy = rnd.Next(1, 1080);

		var Rect1 = new Rectangle()
		{
			X = randomx,
			Y = randomy,
			Width = 50,
			Height = 50,
			Fill = "red",
			//  OnClick = tot,
			OnClick = (args) => { movingrects.Transform((movingrects) => movingrects.Visible = !movingrects.Visible); Update(); },
		};
		AddElement(Rect1);
		movingrects.Add(Rect1);

		Update();
	}
	public override async Task GameLoop(CancellationToken token)
	{
		// rects.Add(Rect1);
		while (true)
		{

			if (token.IsCancellationRequested)
			{
				return;
			}

			movingrects.Transform((rect2) => rect2.Y += 5);
			Update();
			await Task.Delay(10, token);

		}
	}
	public void tot(EventArgs args)
	{
		Rect1.Kill();
	}
}
