using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

public class Block
{

	public int positionArrayX { get; set; }
	public int positionArrayY { get; set; }
	public Thickness positionXamlMargin { get; set; }
	public bool allowMoving { get; set; }
	public Rectangle rectangle { get; }
	public Tetron tetronID { get; }
	public int Id { get; set; }

	// Ein leerer Konstruktor für übergangs Blöcke
	public Block()
    {
    }

	// Der Konstruktor der Blöcke, welche beim auffüllen des GameOver Screens benötigt werden, da hier der opacity-Wert mitgegeben wird
	public Block(int typeOfTetron, double opacity)
    {
		rectangle = new Rectangle();
		rectangle.Width = 50;
		rectangle.Height = 50;
		rectangle.Opacity = opacity;
		BitmapImage theImage;
		ImageBrush myImageBrush;
		switch (typeOfTetron)
		{
			case 1: /**
			         *    xx
			         *    xx
			         */
				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/blueBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;

			case 2:

				/**
				 *        xx
				 *       xx
				 */
				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/greenBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;

			case 3:

				/**
				 *  xx
				 *   xx
				 */
				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/orangeBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;

			case 4:

				/**
				 *    x
				 *   xxx
				 */

				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/purpleBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;

			case 5:

				/**
				 *    x
				 *    xxx
				 */


				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/redBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;

			case 6:

				/**
				 *      x
				 *    xxx
				 */

				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/turkisBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;

			case 7:

				/**
				 * 
				 *   xxxx
				 *   
				 */

				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/yellowBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;
		}
	}


	//Das Konstruktor für die Blöcke, welche später zu einem Tetron hinzugefügt werden un im Spielfeld zum spielen genutzt werdens
	public Block(int typeOfTetron, Tetron tetronID)
	{
		this.tetronID = tetronID;
		rectangle = new Rectangle();
		rectangle.Width = 50;
		rectangle.Height = 50;		
		BitmapImage theImage;
		ImageBrush myImageBrush;
		switch (typeOfTetron)
        {
			case 1: /**
			         *    xx
			         *    xx
			         */
				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/blueBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;

			case 2:

				/**
				 *        xx
				 *       xx
				 */
				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/greenBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;

			case 3:

				/**
				 *  xx
				 *   xx
				 */
				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/orangeBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;

			case 4:

				/**
				 *    x
				 *   xxx
				 */

				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/purpleBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;

			case 5:

				/**
				 *    x
				 *    xxx
				 */


				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/redBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;

			case 6:

				/**
				 *      x
				 *    xxx
				 */

				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/turkisBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;

			case 7:

				/**
				 * 
				 *   xxxx
				 *   
				 */

				theImage = new BitmapImage(new Uri("ms-appx:/Assets/Blocks/yellowBlock.png"));
				myImageBrush = new ImageBrush();
				myImageBrush.ImageSource = theImage;
				rectangle.Fill = myImageBrush;
				break;
        }
	}


    public void SetPosition(int x, int y)
	{
		positionArrayX = x;
		positionArrayY = y;
	}

}
