using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

public class Tetron
{

    public Canvas tetronCanvas { get; set; }
    public int typeOfTetron { get; }
    Block[] block;
    Random rand = new Random();
    public int rotatePosition { set; get; }
    public int centerX { get; set; }
    public int centerY { get; set; }

    //Der Konstruktor für die normale Erstellung von neuen Tetrons
    public Tetron()
    {
        block = new Block[4];
        typeOfTetron = rand.Next(1, 8);
        CreateNewTetron(typeOfTetron);
    }

    //Der Konstruktor für die Erstellung des Tetrons, welcher gespeichert geworden ist und nun neu auf dem Spielfeld ist
    public Tetron(int typeOfTetron)
    {
        block = new Block[4];
        this.typeOfTetron = typeOfTetron;
        CreateNewTetron(typeOfTetron);
    }

    //Die Funktion fügt die einzelnen Blöcke dem Canvas hinzu, setzt die Positionen und die ID. Der am Schluss bekommt der tetronCanvas noch seine neue Thickness.
    //Bei den meisten Tetron-Typen kommen noch die center-Positionen zum drehen hinzu und bei manchen eine rotatePosition 1
    private void CreateNewTetron(int typeOfTetron)
    {
        tetronCanvas = new Canvas();
        switch (typeOfTetron)
        {
            case 1: /**
			         *    xx
			         *    xx
			         */
                for (int i = 0; i < 4; i++)
                {
                    block[i] = new Block(typeOfTetron, this);
                    switch (i)
                    {
                        case 0:
                            Canvas.SetTop(block[i].rectangle, 0);
                            Canvas.SetLeft(block[i].rectangle, 0);
                            block[i].SetPosition(5, 0);
                            block[i].Id = 1;
                            break;

                        case 1:
                            Canvas.SetTop(block[i].rectangle, 0);
                            Canvas.SetLeft(block[i].rectangle, 50);
                            block[i].SetPosition(6, 0);
                            block[i].Id = 2;
                            break;

                        case 2:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 0);
                            block[i].SetPosition(5, 1);
                            block[i].Id = 3;
                            break;

                        case 3:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 50);
                            block[i].SetPosition(6, 1);
                            block[i].Id = 4;
                            break;
                    }
                    tetronCanvas.Margin = new Thickness(850, 56, 974, 1020);
                    tetronCanvas.Children.Add(block[i].rectangle);
                }
                break;

            case 2:

                /**
                 *        xx
                 *       xx
                 */

                for (int i = 0; i < 4; i++)
                {
                    block[i] = new Block(typeOfTetron, this);
                    centerX = 75;
                    centerY = 75;
                    rotatePosition = 1;
                    switch (i)
                    {
                        case 0:
                            Canvas.SetTop(block[i].rectangle, 0);
                            Canvas.SetLeft(block[i].rectangle, 100);
                            block[i].SetPosition(6, 0);
                            block[i].Id = 1;
                            break;

                        case 1:
                            Canvas.SetTop(block[i].rectangle, 0);
                            Canvas.SetLeft(block[i].rectangle, 50);
                            block[i].SetPosition(5, 0);
                            block[i].Id = 2;
                            break;

                        case 2:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 50);
                            block[i].SetPosition(5, 1);
                            block[i].Id = 3;
                            break;

                        case 3:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 0);
                            block[i].SetPosition(4, 1);
                            block[i].Id = 4;
                            break;
                    }
                    tetronCanvas.Margin = new Thickness(800, 56, 974, 1070);
                    tetronCanvas.Children.Add(block[i].rectangle);
                }
                break;
            case 3:

                /**
				 *  xx
				 *   xx
				 */

                for (int i = 0; i < 4; i++)
                {
                    block[i] = new Block(typeOfTetron, this);
                    centerX = 75;
                    centerY = 75;
                    rotatePosition = 1;
                    switch (i)
                    {
                        case 0:
                            Canvas.SetTop(block[i].rectangle, 0);
                            Canvas.SetLeft(block[i].rectangle, 0);
                            block[i].SetPosition(4, 0);
                            block[i].Id = 1;
                            break;

                        case 1:
                            Canvas.SetTop(block[i].rectangle, 0);
                            Canvas.SetLeft(block[i].rectangle, 50);
                            block[i].SetPosition(5, 0);
                            block[i].Id = 2;
                            break;

                        case 2:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 50);
                            block[i].SetPosition(5, 1);
                            block[i].Id = 3;
                            break;

                        case 3:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 100);
                            block[i].SetPosition(6, 1);
                            block[i].Id = 4;
                            break;
                    }
                    tetronCanvas.Margin = new Thickness(800, 56, 974, 1070);
                    tetronCanvas.Children.Add(block[i].rectangle);
                }
                break;


            case 4:

                /**
				 *    x
				 *   xxx
				 */

                for (int i = 0; i < 4; i++)
                {
                    block[i] = new Block(typeOfTetron, this);
                    centerX = 75;
                    centerY = 75;
                    switch (i)
                    {
                        case 0:
                            Canvas.SetTop(block[i].rectangle, 0);
                            Canvas.SetLeft(block[i].rectangle, 50);
                            block[i].SetPosition(5, 0);
                            block[i].Id = 1;
                            break;

                        case 1:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 0);
                            block[i].SetPosition(4, 1);
                            block[i].Id = 2;
                            break;

                        case 2:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 50);
                            block[i].SetPosition(5, 1);
                            block[i].Id = 3;
                            break;

                        case 3:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 100);
                            block[i].SetPosition(6, 1);
                            block[i].Id = 4;
                            break;
                    }
                    tetronCanvas.Margin = new Thickness(800, 56, 974, 1070);
                    tetronCanvas.Children.Add(block[i].rectangle);
                }
                break;

            case 5:

                /**
                 *    x
                 *    xxx
                 */


                for (int i = 0; i < 4; i++)
                {
                    block[i] = new Block(typeOfTetron, this);
                    centerX = 75;
                    centerY = 75;
                    switch (i)
                    {
                        case 0:
                            Canvas.SetTop(block[i].rectangle, 0);
                            Canvas.SetLeft(block[i].rectangle, 0);
                            block[i].SetPosition(4, 0);
                            block[i].Id = 1;
                            break;

                        case 1:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 0);
                            block[i].SetPosition(4, 1);
                            block[i].Id = 2;
                            break;

                        case 2:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 50);
                            block[i].SetPosition(5, 1);
                            block[i].Id = 3;
                            break;

                        case 3:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 100);
                            block[i].SetPosition(6, 1);
                            block[i].Id = 4;
                            break;
                    }
                    tetronCanvas.Margin = new Thickness(800, 56, 974, 1070);
                    tetronCanvas.Children.Add(block[i].rectangle);
                }
                break;

            case 6:

                /**
                 *      x
                 *    xxx
                 */

                for (int i = 0; i < 4; i++)
                {
                    block[i] = new Block(typeOfTetron, this);
                    centerX = 75;
                    centerY = 75;
                    switch (i)
                    {
                        case 3:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 0);
                            block[i].SetPosition(4, 1);
                            block[i].Id = 1;
                            break;

                        case 2:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 50);
                            block[i].SetPosition(5, 1);
                            block[i].Id = 2;
                            break;

                        case 1:
                            Canvas.SetTop(block[i].rectangle, 50);
                            Canvas.SetLeft(block[i].rectangle, 100);
                            block[i].SetPosition(6, 1);
                            block[i].Id = 3;
                            break;

                        case 0:
                            Canvas.SetTop(block[i].rectangle, 0);
                            Canvas.SetLeft(block[i].rectangle, 100);
                            block[i].SetPosition(6, 0);
                            block[i].Id = 4;
                            break;
                    }
                    tetronCanvas.Margin = new Thickness(800, 56, 974, 1070);
                    tetronCanvas.Children.Add(block[i].rectangle);
                }
                break;
            case 7:

                /**
                 * 
                 *   xxxx
                 *   
                */

                for (int i = 0; i < 4; i++)
                {
                    block[i] = new Block(typeOfTetron, this);
                    rotatePosition = 1;
                    centerX = 100;
                    centerY = 0;
                    switch (i)
                    {
                        case 0:
                            Canvas.SetTop(block[i].rectangle, 0);
                            Canvas.SetLeft(block[i].rectangle, 0);
                            block[i].SetPosition(4, 0);
                            block[i].Id = 1;
                            break;

                        case 1:
                            Canvas.SetTop(block[i].rectangle, 0);
                            Canvas.SetLeft(block[i].rectangle, 50);
                            block[i].SetPosition(5, 0);
                            block[i].Id = 2;
                            break;

                        case 2:
                            Canvas.SetTop(block[i].rectangle, 0);
                            Canvas.SetLeft(block[i].rectangle, 100);
                            block[i].SetPosition(6, 0);
                            block[i].Id = 3;
                            break;

                        case 3:
                            Canvas.SetTop(block[i].rectangle, 0);
                            Canvas.SetLeft(block[i].rectangle, 150);
                            block[i].SetPosition(7, 0);
                            block[i].Id = 4;
                            break;
                    }
                    tetronCanvas.Margin = new Thickness(800, 56, 974, 1030);
                    tetronCanvas.Children.Add(block[i].rectangle);
                }
                break;
        }

    }

    public Block[] GetBlocks()
    {
        return block;
    }

}
