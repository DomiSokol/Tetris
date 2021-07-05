using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Tetris
{
    public sealed partial class GamePage : Page
    {
        private DispatcherTimer timer;
        Highscore highscore;
        List<Highscore> highscoreList = new List<Highscore>();
        DispatcherTimer fill = new DispatcherTimer();
        Random rand = new Random();
        Music music;
        RotateTransform rotateTransform = new RotateTransform();
        Block[,] playground = new Block[11, 18];
        Block[,] playgroundForRotating = new Block[11, 18];
        Block[,] matrixArray4x4 = new Block[4, 4];
        Block[,] matrixArray3x3 = new Block[3, 3];
        Block[,] matrixArray2x2 = new Block[2, 2];
        Block[,] matrixArray3x3AfterRotating = new Block[3, 3];
        Block[,] matrixArray4x4AfterRotating = new Block[4, 4];
        Block[] block;
        Tetron[] nextTetrons = new Tetron[5];
        Tetron tetron;
        Tetron[] storedTetron = new Tetron[2];
        Score score;
        TimeSpan speed;
        private bool tetronChanged;
        private bool pauseButtonHeldDown;
        private bool sideKeyHeldDown;
        private bool softDropKeyHeldDown;
        private bool switchKeyHeldDown;
        private bool rotateKeyHeldDown;
        private bool tetronMoving;
        private bool gameOver = false;
        private bool rotateSuccessfull = true;
        private String rotateDirection;
        private int currentLevel;
        private int rowsSoftDropped;
        private int firstDestroyedLine;
        private int numberOfRowsDeleted;
        private int currentTickRate = 10000000;
        int rowFilling = 17;
        int columnFilling = 0;

        //Im Konstruktor von der Gamepage werden vor allem Objekte erstellt, das Array an den nächsten Tetrons, das befüllen des PlaygroundForRotating und der Ticker, welcher initalisiert wird. 
        public GamePage()
        {
            this.InitializeComponent();
            music = new Music();
            _ = music.GetVolume("GamePage");
            highscore = new Highscore();
            _ = highscore.GetNewHighscoreList();
            highscoreList = highscore.GetHighscoreList();
            PlayfieldXAML.Visibility = Visibility.Visible;
            FillPlaygroundForRotating();
            FillNextTetrons();
            score = new Score();
            timer = new DispatcherTimer();
            speed = new TimeSpan(currentTickRate);
            timer.Interval = speed;
            timer.Tick += Timer_Ticker;
            timer.Start();    
        }

        //Eine einfache Funktion um das Array mit den folgenden Tetrons zu befüllen und als nächstes das erste Tetron dem Spielfeld hinzuzufügen
        private void FillNextTetrons()
        {
            for(int i = 0; i < nextTetrons.Length; i++)
            {
                nextTetrons[i] = new Tetron();
            }
            GetNextTetron();
        }

        //Das PlaygroundForRotating wird mit einzelnen, unrelevanten Blöcken gefüllt, welche nur dafür da sind, um die Position auf dem visuellen Spielfeld anzeigen zu können
        private void FillPlaygroundForRotating()
        {
            int hightUp = 56;
            int hightDown = 974;
            for (int row = 0; row < 18; row++)
            {
                int widthLeft = 600;
                int widthRight = 1270;
                for (int column = 0; column < 11; column++)
                {
                    playgroundForRotating[column, row] = new Block();
                    playgroundForRotating[column, row].positionXamlMargin = new Thickness(widthLeft, hightUp, widthRight, hightDown);
                    widthLeft += 50;
                    widthRight -= 50;
                }
                hightUp += 50;
                hightDown -= 50;
            }
        }

        //Es wird überprüft, welcher Typ des Tetrons als nächstes hinzugefügt werden soll. Folgend wird überprüft, ob die Positionen, welche die Blöcke bekommen sollen, leer sind. Sind sie leer, werden die Steine einzeln logisch plaziert und das gesamte Tetron wird dem visuellen Spielfeld hinzugefügt. Ist die Position von mindestens einem Tetron nicht leer, wird das Tetron nicht plaziert und die Runde wird beendet.
        private void InsertTetron()
        {
            switch (tetron.typeOfTetron)
            {
                case 1:
                    if (playground[5, 0] == null && playground[6, 0] == null && playground[5, 1] == null && playground[6, 1] == null)
                    {
                        playground[5, 0] = tetron.GetBlocks()[0];
                        playground[6, 0] = tetron.GetBlocks()[1];
                        playground[5, 1] = tetron.GetBlocks()[2];
                        playground[6, 1] = tetron.GetBlocks()[3];
                        PlayfieldXAML.Children.Add(tetron.tetronCanvas);
                    }
                    else GameOver();
                    break;

                case 2:
                    if (playground[6, 0] == null && playground[5, 0] == null && playground[5, 1] == null && playground[4, 1] == null)
                    {
                        playground[6, 0] = tetron.GetBlocks()[0];
                        playground[5, 0] = tetron.GetBlocks()[1];
                        playground[5, 1] = tetron.GetBlocks()[2];
                        playground[4, 1] = tetron.GetBlocks()[3];
                        PlayfieldXAML.Children.Add(tetron.tetronCanvas);
                    }
                    else GameOver();
                    break;

                case 3:
                    if (playground[4, 0] == null && playground[5, 0] == null && playground[5, 1] == null && playground[6, 1] == null)
                    {
                        playground[4, 0] = tetron.GetBlocks()[0];
                        playground[5, 0] = tetron.GetBlocks()[1];
                        playground[5, 1] = tetron.GetBlocks()[2];
                        playground[6, 1] = tetron.GetBlocks()[3];
                        PlayfieldXAML.Children.Add(tetron.tetronCanvas);
                    }
                    else GameOver();
                    break;

                case 4:
                    if (playground[5, 0] == null && playground[4, 1] == null && playground[5, 1] == null && playground[6, 1] == null)
                    {
                        playground[5, 0] = tetron.GetBlocks()[0];
                        playground[4, 1] = tetron.GetBlocks()[1];
                        playground[5, 1] = tetron.GetBlocks()[2];
                        playground[6, 1] = tetron.GetBlocks()[3];
                        PlayfieldXAML.Children.Add(tetron.tetronCanvas);
                    }
                    else GameOver();
                    break;

                case 5:
                    if (playground[4, 0] == null && playground[4, 1] == null && playground[5, 1] == null && playground[6, 1] == null)
                    {
                        playground[4, 0] = tetron.GetBlocks()[0];
                        playground[4, 1] = tetron.GetBlocks()[1];
                        playground[5, 1] = tetron.GetBlocks()[2];
                        playground[6, 1] = tetron.GetBlocks()[3];
                        PlayfieldXAML.Children.Add(tetron.tetronCanvas);
                    }
                    else GameOver();
                    break;

                case 6:
                    if (playground[4, 1] == null && playground[5, 1] == null && playground[6, 1] == null && playground[6, 0] == null)
                    {
                        playground[6, 0] = tetron.GetBlocks()[0];
                        playground[6, 1] = tetron.GetBlocks()[1];
                        playground[5, 1] = tetron.GetBlocks()[2];
                        playground[4, 1] = tetron.GetBlocks()[3];
                        PlayfieldXAML.Children.Add(tetron.tetronCanvas);
                    }
                    else GameOver();
                    break;

                case 7:
                    if (playground[4, 0] == null && playground[5, 0] == null && playground[6, 0] == null && playground[7, 0] == null)
                    {
                        playground[4, 0] = tetron.GetBlocks()[0];
                        playground[5, 0] = tetron.GetBlocks()[1];
                        playground[6, 0] = tetron.GetBlocks()[2];
                        playground[7, 0] = tetron.GetBlocks()[3];
                        PlayfieldXAML.Children.Add(tetron.tetronCanvas);
                    }
                    else GameOver();
                    break;
            }
            //Die block-Variable erscheint nützlich in vielen weiteren Fällen, während der aktuelle Stein im Spiel ist und bewegbar ist.
            block = tetron.GetBlocks();
            //Da der Timer erst nach dem ersten Befüllen des Spielfeldes initialisiert wird und ohne der Abfrage, eine nullPointerException geworfen würde
            if (timer != null && PauseScreen.Visibility != Visibility.Visible && !gameOver)
            {
                timer.Start();
            }
        }


        /*Der Ticker wird gestoppt, die Variable "gameOver" wird auf True gesetzt (welche bewirkt, dass der Timer nicht neu gestartet wird) und es wird die GameOver-Musik abgespielt. 
         * Ebenfalls wird erst das letzte Tetron, welches hinzugefügt wird, entfernt, damit es schöner aussieht beim späteren befüllen des GameOver-Screens, welcher mit dem neuen Ticker gestartet wird
         * Allgemein wird das Grid "GameOverScreen" auf Visible gesetzt und überprüft, ob die Punktzahl für einen neuen Highscore reicht. Dies wird einfach geprüft, ob der letzte Eintrag der Liste "highscoreList" (geordnet abfallend nach der Punktzahl) kleiner des aktuellen Punktestandes ist. Falls nein, werden die beiden XAML-Elemente "NameInput" und "NewHighscore" nicht angezeigt.
         * 
        */
        private void GameOver()
        {
            timer.Stop();
            gameOver = true;
            for (int row = 0; row < 2; row++)
            {
                for (int column = 4; column < 8; column++)
                {
                    if (playground[column, row] != null)
                    {
                        PlayfieldXAML.Children.Remove(playground[column, row].rectangle);
                    }
                }
            }
            music.Play_GameOverMusic();
            fill.Interval = new TimeSpan(10000);
            fill.Tick += FillGameOver;
            fill.Start();
            GameOverScreen.Visibility = Visibility.Visible;
            ScorePoints.Text = "Score: " + score.points;
            highscoreList = highscoreList.OrderByDescending(x => x.scored).ToList();
            if (score.points < highscoreList.Last<Highscore>().scored)
            {
                NameInput.Visibility = Visibility.Collapsed;
                NewHighscore.Visibility = Visibility.Collapsed;
                AcceptHighscore_Button.Visibility = Visibility.Collapsed;
            }
        }


        //Die Funktion, um das Spielfeld mit zufälligen Blöcken zu füllen. Hier geht es einzig und allein um verschiedene Farben, welche nach und nach eingefügt werden. Falls das letzte Element dran ist, wird der Timer danach gestoppt und auf null gesetzt um Probleme beim Neustarten des Spiels, zu vermeiden.
        private void FillGameOver(object sender, object e)
        {
            if(columnFilling == 10 && rowFilling == 0)
            {
                if (playground[columnFilling, rowFilling] != null) PlayfieldXAML.Children.Remove(playground[columnFilling, rowFilling].rectangle);
                playground[columnFilling, rowFilling] = new Block(rand.Next(1, 8), 0.4);
                playground[columnFilling, rowFilling].rectangle.Margin = playgroundForRotating[columnFilling, rowFilling].positionXamlMargin;
                PlayfieldXAML.Children.Add(playground[columnFilling, rowFilling].rectangle);
                fill.Stop();
                fill = null;
            }
            else
            {
                if (playground[columnFilling, rowFilling] != null) PlayfieldXAML.Children.Remove(playground[columnFilling, rowFilling].rectangle);
                playground[columnFilling, rowFilling] = new Block(rand.Next(1, 8), 0.4);
                playground[columnFilling, rowFilling].rectangle.Margin = playgroundForRotating[columnFilling, rowFilling].positionXamlMargin;
                PlayfieldXAML.Children.Add(playground[columnFilling, rowFilling].rectangle);
                if (columnFilling == 10)
                {
                    columnFilling = 0;
                    if (rowFilling > -1) rowFilling--;
                }
                else columnFilling++;
            }

        }
        
        //Die Funktion um den folgenden Tetron für das Spielfeld zu bekommen und das Array mit einem neuen zu befüllen. Hier wird auch die Anzeige rechts neben dem Spielfeld aktualisiert um zu sehen, welcher Tetron als nächstes kommt und welcher neu dazugekommen ist.
        private void GetNextTetron()
        {
            //Hier wird das nächste Tetron geholt
            tetron = nextTetrons[4];
            for(int i = 4; i >= 0; i--)
            {
                //Hier wird das neue Tetron erstellt
                if (i == 0) nextTetrons[i] = new Tetron();
                else nextTetrons[i] = nextTetrons[i - 1];
                switch (i)
                {
                    case 0:
                        Tetron0.Source = GetImageSource(nextTetrons[i].typeOfTetron);
                        break;

                    case 1:
                        Tetron1.Source = GetImageSource(nextTetrons[i].typeOfTetron);
                        break;

                    case 2:
                        Tetron2.Source = GetImageSource(nextTetrons[i].typeOfTetron);
                        break;

                    case 3:
                        Tetron3.Source = GetImageSource(nextTetrons[i].typeOfTetron);
                        break;

                    case 4:
                        Tetron4.Source = GetImageSource(nextTetrons[i].typeOfTetron);
                        break;
                }
                    
            }
            InsertTetron();
        }

        /*Die Hauptticker-Funktion. Hier bewegt sich der Stein von Vertikal nach jedem Tick. TetronMoving ist eine Variable, welche Deadlocks und Überschneidungen verhindert. Überschneidungen könnten auftreten, wenn  derSpieler den Stein dreht, horizontal bewegt und gleichzeitig der Ticker getriggered wird. Falls der Stein sich aktuell bewegt, wird der timer einfach neu aufgerufen, solange der Vorgang noch nicht abgeschlossen ist
         * Falls das Tetron keine Kollision besitzen sollte mit einem weiteren Tick, wird das Tetron um ein Block nach unten gesetzt. Falls doch, wird es gesetzt, überprüft ob das Spielfeld fertige Reihen besitzt, die Punktzahl aktualisiert und das nächste Tetron geholt.
         */
        private void Timer_Ticker(object sender, object e)
        {
            if (!tetronMoving)
            {
                if (CollisionY())
                {
                    ChangePositionY();
                }
                else
                {
                    SetTetron();
                    CheckLinesComplete();
                    Points.Text = score.points + "";
                    GetNextTetron();
                }
            }
            else
            {
                timer.Stop();
                timer.Start();
            }
        }


        /*Wie im GameBoy-Tetris wird hier die Geschwindigkeit des Timers auf die Hauptgeschwindigkeit gesetzt und zuerst einmal gestopt, damit der Vorgang erst einmal abgeschlossen werden kann.
         * Im weiteren Verlauf werden die einzelnen Blöcke vom tetronCanvas getrennt und einzeln in das Spielfeld visuelle hinzugefügt und die ID wird auf 0 gesetzt. Dies wird benötigt für die einzelnen Bewegungen.
         * 
         * rotateTransform.Angle wird auf 0 gesetzt, da bei einem neuen Tetron der Winkel neutral sein sollte
         * 
         * Die Punkte werden addiert und falls die maximal Anzahl erreicht wird, wird der Punktestand auf diesen gesetzt.
         * 
         * Im folgenden wird dann noch die Variable "rowsSoftDropped" auf 0 für den nächsten Stein gesetzt und "tetronChanged" wird auf false gesetzt, damit man wieder die Steine tauschen darf.
         */
        private void SetTetron()
        {
            timer.Stop();
            speed = new TimeSpan(currentTickRate);
            timer.Interval = speed;
            for (int i = 0; i < block.Length; i++)
            {
                playground[block[i].positionArrayX, block[i].positionArrayY] = block[i];
                block[i].tetronID.tetronCanvas.Children.Remove(block[i].rectangle);
                block[i].rectangle.Margin = block[i].positionXamlMargin;
                block[i].Id = 0;
                PlayfieldXAML.Children.Add(block[i].rectangle);
            }
            rotateTransform.Angle = 0;
            score.points += rowsSoftDropped;
            if (score.points > 9999999)
            {
                score.points = 9999999;
            }
            rowsSoftDropped = 0;
            tetronChanged = false;
        }

        /* Das komplette Spielfeld wird überprüft ob eine Linie komplett ist (column == 10)
         * 
         * Falls das der Fall sein sollte, wird die Anzahl der zerstörten Linien aktualisiert.
         * Falls keine Linien komplett sind, wird die Combo-Variable wieder auf 0 gesetzt, da es zu keiner aufeinanderkommenden combo gekommen ist.
         * 
         */
        private void CheckLinesComplete()
        {
            for (int row = 17; row > 0; row--)
            {
                for (int column = 0; column < 11; column++)
                {
                    if (playground[column, row] == null) break;
                    if (column == 10)
                    {
                        numberOfRowsDeleted++;
                    }
                }
            }
            if (numberOfRowsDeleted > 0)
            {
                DeleteLines();
                UpdateScreen();
                numberOfRowsDeleted = 0;
            }
            else
            {
                score.combo = 0;
            }
        }
        

        /* Die Funktion aktualisiert das visuelle sowie das logische Spielfeld
         * 
         * Diese Funktion wird so häufig ausgeführt, wie Lines vollständig gefüllt worden sind.
         * Da es sein kann, dass man sich bereits verbaut hatte und nun weiter höher erst einmal sich "freipuzzeln" muss, wird hier erst ab der ersten Reihe des zerstörten Blocks aktualisiert.
         * Anderenfalls könnte es dazukommen, dass weiter untergelegene Blöcke, verschoben werden, obwohl die Aktion sie nich beeinflussen konnte.
         */
        private void UpdateScreen()
        {
            for (int times = numberOfRowsDeleted; times > 0; times--)
            {
                for (int row = firstDestroyedLine; row >= 0; row--)
                {
                    for (int column = 0; column < 11; column++)
                    {
                        if (playground[column, row] != null && playground[column, row].positionArrayY + 1 != 18 && playground[column, row + 1] == null)
                        {
                            playground[column, row].rectangle.Margin = new Thickness(playground[column, row].rectangle.Margin.Left, playground[column, row].rectangle.Margin.Top + 50, playground[column, row].rectangle.Margin.Right, playground[column, row].rectangle.Margin.Bottom - 50);
                            playground[column, row + 1] = playground[column, row];
                            playground[column, row].positionArrayY++;
                            playground[column, row] = null;
                        }
                    }
                }
            }
            firstDestroyedLine = 0;
        }


        //Die Funktion ändert das aktuelle Level in der Klasse "Score". Ebenfalls wird die Geschwindigkeit der MainTheme erhöht, ein LevelUp-Sound gespielt, die Tickrate erhöht und die Anzeige des Levels angepasst.
        private void ChangeLevel()
        {
            score.ChangeLevel();
            if(currentLevel != score.level && currentLevel < 15)
            {
                music.mainTheme.PlaybackSession.PlaybackRate += 0.01;
                music.Play_LevelUp();
                currentTickRate -= 500000;
                speed = new TimeSpan(currentTickRate);
                timer.Interval = speed;
                currentLevel++;
                LevelTextBlock.Text = "Level: " + score.level;        
            }
        }


        /* Diese Funktion entfernt die Steine visuell wie auch logisch aus den jeweiligen Spielfeldern
         * Sobald die erste Reihe gefunden wurde, welche entfernt werden soll, wird diese Reihe als "firstDestroyedLine" gesetzt.
         * 
         * Ebenfalls werden die Anzahl der Reihen an die aktuelle Anzahl addiert und der Combo-Counter um eins erhöht.
         * 
         */
        private void DeleteLines()
        {
            for (int row = 17; row > 0; row--)
            {
                for (int column = 0; column < 11; column++)
                {
                    if (playground[column, row] == null) break;
                    else if (column == 10)
                    {
                        if (firstDestroyedLine == 0) firstDestroyedLine = row;
                        for (int i = 0; i < 11; i++)
                        {
                            PlayfieldXAML.Children.Remove(playground[i, row].rectangle);
                            playground[i, row] = null;
                        }
                    }
                }
            }
            score.rowsCompleted += numberOfRowsDeleted;
            score.combo++;
            music.Play_LinesDestroyed();
            AddPoints();
            ChangeLevel();
        }

        // Dises Bepunktungssystem wurde aus einer Seite heraus hinzugefügt. Quelle: https://tetris.fandom.com/wiki/Scoring Einzig das Combo-System wurde vereinfacht. Falls nach der Addition die Punktzahl das Maximum erreicht, wird diese auch auf das Maximum gesetzt.

        private void AddPoints()
        {
            switch (numberOfRowsDeleted)
            {
                case 1:
                    score.points += 40 * (score.level + 1);
                    break;
                case 2:
                    score.points += 100 * (score.level + 1);
                    break;
                case 3:
                    score.points += 300 * (score.level + 1);
                    break;
                case 4:
                    score.points += 1200 * (score.level + 1);
                    break;
            }
            if(score.combo > 0)
            {
                score.points += 50 * score.combo * score.level;
            }
            if (score.points > 9999999)
            {
                score.points = 9999999;
            }
        }


        /* Diese Funktion gibt einen bool-Wert zurück, ob die nächste Bewegung entlang der Y-Achse eine Kollision beinhaltet. Hier wird jeder einzelne Block überprüft und am Ende überprüft ob alle Blöcke sich bewegen dürften
         * Falls das der Fall wäre, würde "true" zurückgegeben werden.
         */
        private bool CollisionY()
        {
            for (int i = 0; i < block.Length; i++)
            {
                // Zuerst wird geschaut, ob der nächste Platz nicht außerhalb des Spielfelds wäre (block[i].positionArrayY + 1 != 18) und ob der nächste Platz null ist.
                if (block[i].positionArrayY + 1 != 18 && playground[block[i].positionArrayX, block[i].positionArrayY + 1] == null)
                {
                    block[i].allowMoving = true;
                }
                else if (block[i].positionArrayY + 1 != 18 && playground[block[i].positionArrayX, block[i].positionArrayY + 1] != null)
                {
                    //Falls der nächte Platz nicht null ist, wird überprüft, ob einer der Blöcke des Tetrons der nächste Platz im Spielfeld wäre. Falls ja, darf der Block sich bewegen, da der andere Block sich auch bewegen würde und somit Platz machen würde.
                    for (int ii = 0; ii < block.Length; ii++)
                    {
                        if (block[i] != block[ii] && playground[block[i].positionArrayX, block[i].positionArrayY + 1] == block[ii])
                        {
                            block[i].allowMoving = true;
                            break;
                        }
                        else
                        {
                            block[i].allowMoving = false;
                        }
                    }
                }
                else
                {
                    block[i].allowMoving = false;
                }
            }
            //Überprüfung ob alle 4 Blöcke sich bewegen dürfen
            for (int i = 0; i < block.Length; i++)
            {
                if (!block[i].allowMoving) return false;
            }
            return true;
        }    
        


        // Ähnlich wie bei der Kollisionsüberprüfung der Y-Achse. Unterschied hier ist, dass als Parameter die Richtung mitgegeben wird.
        private bool CollisionX(String direction)
        {
            for (int i = 0; i < block.Length; i++)
            {
                switch (direction)
                {
                    case "Left":
                        if(block[i].positionArrayX-1 != -1 && playground[block[i].positionArrayX -1, block[i].positionArrayY] == null){
                            block[i].allowMoving = true;
                        }
                        else if(block[i].positionArrayX - 1 != -1 && playground[block[i].positionArrayX - 1, block[i].positionArrayY] != null)
                        {
                            for (int ii = 0; ii < block.Length; ii++)
                            {
                                if (block[i] != block[ii] && playground[block[i].positionArrayX - 1, block[i].positionArrayY] == block[ii])
                                {
                                    block[i].allowMoving = true;
                                    break;
                                }
                                else
                                {
                                    block[i].allowMoving = false;
                                }
                            }
                        }
                        else
                        {
                            block[i].allowMoving = false;
                        }
                        break;

                    case "Right":
                        if (block[i].positionArrayX + 1 != 11 && playground[block[i].positionArrayX + 1, block[i].positionArrayY] == null)
                        {
                            block[i].allowMoving = true;
                        }
                        else if (block[i].positionArrayX + 1 != 11 && playground[block[i].positionArrayX + 1, block[i].positionArrayY] != null)
                        {
                            for (int ii = 0; ii < block.Length; ii++)
                            {
                                if (block[i] != block[ii] && playground[block[i].positionArrayX + 1, block[i].positionArrayY] == block[ii])
                                {
                                    block[i].allowMoving = true;
                                    break;
                                }
                                else
                                {
                                    block[i].allowMoving = false;
                                }
                            }
                        }                        
                        else
                        {
                            block[i].allowMoving = false;
                        }

                        break;
                }
               
            }
            for (int i = 0; i < block.Length; i++)
            {
                if (!block[i].allowMoving) return false;
            }
            return true;
        }

        /* Die Funktion verschiebt das Tetron um die Y-Achse nach unten. Zuerst einmal in visuellen Art, später in logischer Art
         * Hier wird auch die Variable "tetronMoving" true gesetzt, um Überschneidungen zu unterbinden
         */
        private void ChangePositionY()
        {
            tetronMoving = true;
            tetron.tetronCanvas.Margin = new Thickness(tetron.tetronCanvas.Margin.Left, tetron.tetronCanvas.Margin.Top + 50, tetron.tetronCanvas.Margin.Right, tetron.tetronCanvas.Margin.Bottom - 50);
            //Hier wird überpüft, ob der Stein gerade erst hinzugefügt worden ist. Dies dient dazu, da hier das verschieben auf logischer Art geändert werden muss ohne die Funktion PlaygroundToMatrixArray(), da es hier zu Fehlern kommen würde. Diese Fehler würden das Spiel crashen, da das MatrixArray Werte außerhalb des Spielbereichs nehmen würde. Tetrons vom Typ 7 benötigen eine gesonderte Überprüfung, da sie die längsten sind und der block[0] weiter unten ist.
            if (block[0].positionArrayY - 1 < 0 || tetron.typeOfTetron == 7 && block[0].positionArrayY - 2 < 0)
            {
                for (int i = block.Length - 1; i > -1; i--)
                {
                    playground[block[i].positionArrayX, block[i].positionArrayY] = null;
                    block[i].SetPosition(block[i].positionArrayX, block[i].positionArrayY + 1);
                    block[i].positionXamlMargin = new Thickness(block[i].positionXamlMargin.Left, block[i].positionXamlMargin.Top + 50, block[i].positionXamlMargin.Right, block[i].positionXamlMargin.Bottom - 50);
                    playground[block[i].positionArrayX, block[i].positionArrayY] = block[i];
                }
                tetronMoving = false;
            }
            else
            {   
                // Hier wird das aktuelle Tetron vom Spielfeld in ein extra MatrixArray hinzugefügt, die Position um 1 verändert und danach visuell wieder angepasst in der Funktion MatrixArrayToPlayground() und logisch hinzugefügt
                PlaygroundToMatrixArray();
                for (int i = 0; i < 4; i++)
                {
                    playground[block[i].positionArrayX, block[i].positionArrayY] = null;
                    block[i].SetPosition(block[i].positionArrayX, block[i].positionArrayY + 1);
                }            
                MatrixArrayToPlayground();
            }
            // Falls diese Aktion passierte, während "S" gedrückt gehalten wurde, wird der Wert um 1 erhöht.
            if (softDropKeyHeldDown) rowsSoftDropped++;
        }

        // Gleich Funktionsweise wie bei ChangePositionY mit der Ausnahme des Parameters "direction".
        private void ChangePositionX(String direction)
        {
            tetronMoving = true;
            switch (direction)
            {
                case "Left":
                    tetron.tetronCanvas.Margin = new Thickness(tetron.tetronCanvas.Margin.Left - 50, tetron.tetronCanvas.Margin.Top, tetron.tetronCanvas.Margin.Right + 50, tetron.tetronCanvas.Margin.Bottom);
                    PlaygroundToMatrixArray();
                    for (int i = 0; i < block.Length; i++)
                    {
                        playground[block[i].positionArrayX, block[i].positionArrayY] = null;
                        block[i].SetPosition(block[i].positionArrayX - 1, block[i].positionArrayY);
                    }
                    MatrixArrayToPlayground();
                    break;
                case "Right":
                    tetron.tetronCanvas.Margin = new Thickness(tetron.tetronCanvas.Margin.Left + 50, tetron.tetronCanvas.Margin.Top, tetron.tetronCanvas.Margin.Right - 50, tetron.tetronCanvas.Margin.Bottom);
                    PlaygroundToMatrixArray();
                    for (int i = 0; i < block.Length; i++)
                    {
                        playground[block[i].positionArrayX, block[i].positionArrayY] = null;
                        block[i].SetPosition(block[i].positionArrayX + 1, block[i].positionArrayY);
                    }
                    MatrixArrayToPlayground();
                    break;
            }
        }
      
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
        }

        // KeyUp wurde hinzugefügt, da es keine Möglichkeit gab "ButtonPressed" zu nutzen. Somit musste diese Weg gegangen sein mit Variablen, welche hier dann auf false gesetzt werden, sobald die Taste nicht mehr gedrückt wird.
        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            if (!gameOver)
            {
            switch (args.VirtualKey)
            {
                case VirtualKey.A:
                    sideKeyHeldDown = false;
                    break;

                case VirtualKey.D:
                    sideKeyHeldDown = false;
                    break;

                case VirtualKey.Left:
                    rotateKeyHeldDown = false;
                    break;
                case VirtualKey.Right:
                    rotateKeyHeldDown = false;
                    break;
                // Einzige Ausnahme ist hier die Geschwindigkeit des Fallens der Tetron. Diese wird hier auf die normale Geschwindigkeit zurückgesetzt.
                case VirtualKey.S:
                        speed = new TimeSpan(currentTickRate);
                        timer.Interval = speed;
                        softDropKeyHeldDown = false;
                    break;

                case VirtualKey.Q:
                    switchKeyHeldDown = false;
                    break;

                case VirtualKey.Escape:
                    pauseButtonHeldDown = false;
                    break;
            }
            }
        }

        public void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            String direction;
            // Da hier Funktionen in kritische Bereiche kommen können, wird zuerst überprüft ob diese frei sind.
            if (!tetronMoving)
            {
                if (!gameOver && PlayfieldXAML.Visibility == Visibility.Visible)
                {
                    switch (args.VirtualKey)
                    {
                        case VirtualKey.A:
                            direction = "Left";
                            if (CollisionX(direction) && !sideKeyHeldDown) ChangePositionX(direction);
                            sideKeyHeldDown = true;
                            break;
                        case VirtualKey.D:
                            direction = "Right";
                            if (CollisionX(direction) && !sideKeyHeldDown) ChangePositionX(direction);
                            sideKeyHeldDown = true;
                            break;
                        case VirtualKey.Left:
                            if (!rotateKeyHeldDown && AllowRotating())
                            {
                                // rotatePosition wird hier genutzt für die Tetrons 2, 3 und 7. Diese sollen sich immer, egal welcher Key gedrückt worden ist, jeweils einmal im und einmal gegen den Uhrzeigersinn drehen. Alle anderen Steine besitzen die rotatePosition 0
                                if (tetron.rotatePosition == 1)
                                {
                                    RotateClockwise();
                                }
                                else if (tetron.rotatePosition == 2)
                                {
                                    RotateAntiClockwise();
                                }
                                else
                                {
                                    RotateAntiClockwise();
                                }
                            }
                            break;
                        case VirtualKey.Right:
                            if (!rotateKeyHeldDown && AllowRotating())
                            {
                                if (tetron.rotatePosition == 1)
                                {
                                    RotateClockwise();
                                }
                                else if (tetron.rotatePosition == 2)
                                {
                                    RotateAntiClockwise();
                                }
                                else
                                {
                                    RotateClockwise();
                                }
                            }
                            break;
                        case VirtualKey.S:
                            // Die Geschwindigkeit des Tetrons wird hier verändert. Damit dies sofort passiert, wird der Ticker ersteinmal gestopt, das Interval aktualisiert und wieder neu gestartet. Damit dies nicht immer wieder passiert, ist es hier besonders wichtig darauf zu achten, dass die Taste gedrückt ist.
                            if (!softDropKeyHeldDown)
                            {
                                timer.Stop();
                                speed = new TimeSpan(700000);
                                timer.Interval = speed;
                                timer.Start();
                                softDropKeyHeldDown = true;
                            }
                            break;
                        case VirtualKey.Q:
                            if (!switchKeyHeldDown && !tetronChanged)
                            {
                                ChangeTetrons();
                            }
                            break;
                        case VirtualKey.Escape:
                            if (!pauseButtonHeldDown)
                            {
                                // Es wird überpfüft ob der Pausen-Screen sichtbar ist, da man die Pause ebenfalls mit Escape beenden kann.
                                if (PauseScreen.Visibility == Visibility.Collapsed)
                                {
                                    PauseScreen.Visibility = Visibility.Visible;
                                    //Da das music.volume als Dezimalzahl abgespeichert ist, muss diese hier mit 100 multipliziert werden um auf einen schönen, userfreundlichen Wert zu kommen.
                                    MusicVolume.Text = music.volume * 100 + "";
                                    music.Play_PauseMusic();
                                    //Da die mainTheme bereits initalisiert wurde früher, kann hier diese direkt ohne extra Funktion in der Klasse "Music" pausiert werden.
                                    music.mainTheme.Pause();
                                    timer.Stop();
                                }
                                else
                                {
                                    PauseScreen.Visibility = Visibility.Collapsed;
                                    music.mainTheme.Play();
                                    timer.Start();
                                }
                            }
                            break;
                    }
                }           
            }
        }

        //AllowRotating dient besonders dafür, dass das Tetron von Typ 7, erst ab einer späteren Position sich drehen darf, da ansonsten das Tetron oberhalb des Spielfeldes wäre. Ebenfalls darf sich das Tetron-Typ 1 gar nicht drehen, da es sowieso keinen visuellen Effekt hätte
        private bool AllowRotating()
        {
            if (tetron.typeOfTetron == 7)
            {
                if (block[2].positionArrayY > 1)
                {
                    return true;
                }
                else return false;
            }
            if (tetron.typeOfTetron == 1)
            {
                return false;
            }          
            return true;
        }

        // In dieser Funktion wird das aktuelle Tetron mit dem gespeicherten Tetron getauscht. Falls bislang kein Tetron gespeichert worden ist, wird das aktuelle gespeichert und das nächste dem Spielfeld hinzugefügt.
        // Das gespeicherte Tetron nimmt hier immer die Stelle 0 des Arrays "storedTetron" ein.
        private void ChangeTetrons()
        {
            PlayfieldXAML.Children.Remove(tetron.tetronCanvas);
            rotateTransform.Angle = 0;
            // Der Fall, falls es bereits ein gespeichertes Tetron gibt
            if (storedTetron[0] != null)
            {
                for(int i = 0; i < block.Length; i++)
                {
                    playground[block[i].positionArrayX, block[i].positionArrayY] = null;
                }
                if (tetron.rotatePosition != 0) tetron.rotatePosition = 1;
                storedTetron[1] = tetron;
                // Das gespeicherte Tetron wird nun neu erstellt, mit dem dazugehörigen Typen und das alte wird nun zum gespeicherten
                tetron = new Tetron(storedTetron[0].typeOfTetron);
                storedTetron[0] = storedTetron[1];
                storedTetron[1] = null;
                tetronChanged = true;
                SwapedTetron.Source = GetImageSource(storedTetron[0].typeOfTetron);
                InsertTetron();
            }
            else
            {
                for (int i = 0; i < block.Length; i++)
                {
                    playground[block[i].positionArrayX, block[i].positionArrayY] = null;
                }
                if (tetron.rotatePosition != 0) tetron.rotatePosition = 1;
                storedTetron[0] = tetron;
                tetronChanged = true;
                SwapedTetron.Source = GetImageSource(storedTetron[0].typeOfTetron);
                GetNextTetron();
            }           
        }


        /*
         * Das aktuelle Tetron wird aus dem aktuellen logischen Spielfeld herausgeholt. Hier sind entscheidend die einzelnen Steine, da es drei verschiedene Größen der Matrix gibt 2x2, 3x3 und 4x4
         * 
         */
        private void PlaygroundToMatrixArray()
        {
            int i = 0;
            int ii = 0;
            //Im Falle von Tetron-Typ 1 wird das MatrixArray komplett ausgefüllt mit den dazugehörigen Blöcken und es gibt somit keine Besonderheiten
            if (tetron.typeOfTetron == 1)
            {
                for (int row = block[3].positionArrayY - 1; row < block[3].positionArrayY + 1; row++)
                {
                    for (int column = block[3].positionArrayX - 1; column < block[3].positionArrayX + 1; column++)
                    {
                        matrixArray2x2[i, ii] = playground[column, row];
                        playground[column, row] = null;
                        i++;
                    }
                    ii++;
                    i = 0;
                }
            }
            //Tetron-Typ 7 ist das längste Tetron und benötigt somit das matrixArray4x4. Wichtig ist es hier darauf zu achten, welche rotatePosition das Array besitzt, da sich hier die Stelle des block-Arrays ändert umd das Array zu befüllen
            else if (tetron.typeOfTetron == 7)
            {
                if (tetron.rotatePosition == 1)
                {
                    for (int row = block[1].positionArrayY - 2; row < block[1].positionArrayY + 1; row++)
                    {
                        for (int column = block[1].positionArrayX - 1; column < block[1].positionArrayX + 3; column++)
                        {                           
                            if (column > 10)
                            {
                            }
                            // Hier werden neue Blöcke erstellt mit der ID 5 welche später nötig sein werden, wenn das MatrixArray dem Spielfeld wieder hinzugefügt werden.
                            else if (column < 0)
                            {
                                matrixArray4x4[i, ii] = new Block();
                                matrixArray4x4[i, ii].Id = 5;
                            }
                            else if (row > 17)
                            {
                                matrixArray4x4[i, ii] = new Block();
                                matrixArray4x4[i, ii].Id = 5;
                            }
                            else if (row < 0)
                            {
                            }
                            else if (playground[column, row] != null && playground[column, row].Id != 0)
                            {
                                matrixArray4x4[i, ii] = playground[column, row];
                                playground[column, row] = null;
                            }
                            i++;
                        }
                        ii++;
                        i = 0;
                    }
                }
                else
                {
                    for (int row = block[2].positionArrayY - 2; row < block[2].positionArrayY + 2; row++)
                    {
                        for (int column = block[2].positionArrayX - 1; column < block[2].positionArrayX + 3; column++)
                        {
                            if (column > 10)
                            {
                            }
                            else if (column < 0)
                            {
                                matrixArray4x4[i, ii] = new Block();
                                matrixArray4x4[i, ii].Id = 5;
                            }
                            else if (row > 17)
                            {
                                matrixArray4x4[i, ii] = new Block();
                                matrixArray4x4[i, ii].Id = 5;
                            }
                            else if (playground[column, row] != null && playground[column, row].Id != 0)
                            {
                                matrixArray4x4[i, ii] = playground[column, row];
                                playground[column, row] = null;
                            }
                            i++;
                        }
                        ii++;
                        i = 0;
                    }
                }
            }
            else
            {
                // Hier werden alle restlichen Tetron-Typen in das matrixArray3x3 hinzugefügt. Ähnlich wie bei Tetron-Typ 7, werden Positionen column < 0 und row > 17 mit einem extra Block mit der ID 5 erstellt.
                for (int row = block[2].positionArrayY - 1; row < block[2].positionArrayY + 2; row++)
                {
                    for (int column = block[2].positionArrayX - 1; column < block[2].positionArrayX + 2; column++)
                    {
                        if (column > 10)
                        {
                        }
                        else if (column < 0)
                        {
                            matrixArray3x3[i, ii] = new Block();
                            matrixArray3x3[i, ii].Id = 5;
                        }
                        else if (row > 17)
                        {
                            matrixArray3x3[i, ii] = new Block();
                            matrixArray3x3[i, ii].Id = 5;
                        }
                        else if (playground[column, row] != null && playground[column, row].Id != 0)
                        {
                            matrixArray3x3[i, ii] = playground[column, row];
                            playground[column, row] = null;
                        }
                        i++;
                    }
                    ii++;
                    i = 0;
                }
            }
        }



        // Diese Funktion fügt MatrixArrays dem aktuellen logischen Spielfeldes wieder hinzu
        private void MatrixArrayToPlayground()
        {
            if (tetron.typeOfTetron == 1)
            {
                for (int row = 0; row < 18; row++)
                {
                    for (int column = 0; column < 11; column++)
                    {
                        // Hier wird wieder die selbe Position der oberen linken Ecke abgefragt.
                        if (row == block[3].positionArrayY - 1 && column == block[3].positionArrayX - 1)
                        {                          
                            int newColumn = column;
                            int newRow = row;
                            for (int rowRotating = 0; rowRotating < 2; rowRotating++)
                            {
                                for (int columnRotating = 0; columnRotating < 2; columnRotating++)
                                {
                                    //Es werden die neuen Positionen des MatrixArray gesetzt, die Thickness (positoinXamlMargin) und das matrixArray wird dem playgroundForRotating hinzugefügt, damit es im nächsten Schritt geleert werden kann.
                                    matrixArray2x2[columnRotating, rowRotating].SetPosition(newColumn, newRow);
                                    matrixArray2x2[columnRotating, rowRotating].positionXamlMargin = playgroundForRotating[newColumn, newRow].positionXamlMargin;
                                    playgroundForRotating[newColumn, newRow] = matrixArray2x2[columnRotating, rowRotating];
                                    newColumn++;
                                }
                                newColumn = column;
                                newRow++;
                            }
                            for (int i = 0; i < 2; i++)
                            {
                                for (int ii = 0; ii < 2; ii++)
                                {
                                    matrixArray2x2[i, ii] = null;
                                }
                            }
                            break;
                        }


                    }
                }
            }

            // Ähnlich wie bei Tetron-Typ 1 wird das ähnliche bei den restlichen Tetron-Typen gemacht. Unterschied ist bei Tetron-Typ 7, dass dieses MatrixArray außerhalb des Spielfelds sein kann und die rotatePosition wichtig ist
            else if(tetron.typeOfTetron == 7)
            { 
                for (int row = 0; row < 18; row++)
                {
                    for (int column = -1; column < 11; column++)
                    {
                        if (tetron.rotatePosition == 1)
                        {
                            // Der normalfall, wenn das matrixArray innerhalb des Spielfelds ist
                            if (row == block[1].positionArrayY - 2 && column == block[1].positionArrayX - 1)
                            {
                                int newColumn = column;
                                int newRow = row;
                                for (int rowRotating = 0; rowRotating < 4; rowRotating++)
                                {
                                    for (int columnRotating = 0; columnRotating < 4; columnRotating++)
                                    {
                                        if (matrixArray4x4[columnRotating, rowRotating] != null && matrixArray4x4[columnRotating, rowRotating].Id != 5)
                                        {
                                            // Try-Catch, falls nach der Rotation eines der Blöcke außerhalb des Spielfelds wäre. In diesem Fall wirft das Programm eine IndexOutOfRangeException, welche aufgefangen wird und das Tetron zurückgedreht wird.
                                            try
                                            {
                                                matrixArray4x4[columnRotating, rowRotating].positionXamlMargin = playgroundForRotating[newColumn, newRow].positionXamlMargin;
                                                matrixArray4x4[columnRotating, rowRotating].SetPosition(newColumn, newRow);
                                                playgroundForRotating[newColumn, newRow] = matrixArray4x4[columnRotating, rowRotating];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                FillPlaygroundForRotating();
                                                rotateSuccessfull = false;
                                                RotateBack();
                                                break;
                                            }
                                        }
                                        newColumn++;
                                    }
                                    newColumn = column;
                                    newRow++;
                                }
                                break;
                            }
                            // Der Fall, falls das matrixArray außerhalb des Spielfelds wäre
                            else if (block[1].positionArrayX - 1 < 0 && block[2].positionArrayY - 2 == row)
                            {
                                int newColumn = 0;
                                int newRow = row;
                                for (int rowRotating = 0; rowRotating < 4; rowRotating++)
                                {
                                    for (int columnRotating = 1; columnRotating < 4; columnRotating++)
                                    {
                                        if (matrixArray4x4[columnRotating, rowRotating] != null && matrixArray4x4[columnRotating, rowRotating].Id != 5)
                                        {
                                            try
                                            {
                                                matrixArray4x4[columnRotating, rowRotating].positionXamlMargin = playgroundForRotating[newColumn, newRow].positionXamlMargin;
                                                matrixArray4x4[columnRotating, rowRotating].SetPosition(newColumn, newRow);
                                                playgroundForRotating[newColumn, newRow] = matrixArray4x4[columnRotating, rowRotating];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                FillPlaygroundForRotating();
                                                rotateSuccessfull = false;
                                                RotateBack();
                                                break;
                                            }
                                        }
                                        newColumn++;
                                    }
                                    newColumn = 0;
                                    newRow++;
                                }
                                break;
                            }
                        }
                        //Das selbe wie bei rotatePosition == 1, außer dass der block an der Stelle 2 betrachtet wird und nicht an der Stelle 1
                        else
                        {
                            if (row == block[2].positionArrayY - 2 && column == block[2].positionArrayX - 1)
                            {
                                int newColumn = column;
                                int newRow = row;
                                for (int rowRotating = 0; rowRotating < 4; rowRotating++)
                                {
                                    for (int columnRotating = 0; columnRotating < 4; columnRotating++)
                                    {
                                        if (matrixArray4x4[columnRotating, rowRotating] != null && matrixArray4x4[columnRotating, rowRotating].Id != 5)
                                        {
                                            try
                                            {
                                                matrixArray4x4[columnRotating, rowRotating].positionXamlMargin = playgroundForRotating[newColumn, newRow].positionXamlMargin;
                                                matrixArray4x4[columnRotating, rowRotating].SetPosition(newColumn, newRow);
                                                playgroundForRotating[newColumn, newRow] = matrixArray4x4[columnRotating, rowRotating];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                FillPlaygroundForRotating();
                                                rotateSuccessfull = false;
                                                RotateBack();
                                                break;
                                            }
                                        }
                                        newColumn++;
                                    }
                                    newColumn = column;
                                    newRow++;
                                }
                                break;
                            }
                            else if (block[2].positionArrayX - 1 < 0 && block[2].positionArrayY - 2 == row)
                            {
                                int newColumn = 0;
                                int newRow = row;
                                for (int rowRotating = 0; rowRotating < 4; rowRotating++)
                                {
                                    for (int columnRotating = 1; columnRotating < 4; columnRotating++)
                                    {
                                        if (matrixArray4x4[columnRotating, rowRotating] != null && matrixArray4x4[columnRotating, rowRotating].Id != 5)
                                        {
                                            try
                                            {
                                                matrixArray4x4[columnRotating, rowRotating].positionXamlMargin = playgroundForRotating[newColumn, newRow].positionXamlMargin;
                                                matrixArray4x4[columnRotating, rowRotating].SetPosition(newColumn, newRow);
                                                playgroundForRotating[newColumn, newRow] = matrixArray4x4[columnRotating, rowRotating];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                FillPlaygroundForRotating();
                                                rotateSuccessfull = false;
                                                RotateBack();
                                                break;
                                            }
                                        }
                                        newColumn++;
                                    }
                                    newColumn = 0;
                                    newRow++;
                                }
                                break;
                            }
                        }
                    }
                       
                }
            }
            // Die restlichen Tetron-Typs mit dem matrixArray3x3. Selbe Funktionsweise wie bei Tetron-Typ 7. Unterschied nur, dass nicht auf die rotatePosition geachtet wird.
            else
            {
                for (int row = 0; row < 18; row++)
                {
                    for (int column = -1; column < 11; column++)
                    {
                        if (row == block[2].positionArrayY - 1 && column == block[2].positionArrayX - 1)
                        {
                            int newColumn = column;
                            int newRow = row;
                            for (int rowRotating = 0; rowRotating < 3; rowRotating++)
                            {
                                for (int columnRotating = 0; columnRotating < 3; columnRotating++)
                                {
                                    if (matrixArray3x3[columnRotating, rowRotating] != null && matrixArray3x3[columnRotating, rowRotating].Id != 5)
                                    {
                                        try
                                        {
                                            matrixArray3x3[columnRotating, rowRotating].positionXamlMargin = playgroundForRotating[newColumn, newRow].positionXamlMargin;
                                            matrixArray3x3[columnRotating, rowRotating].SetPosition(newColumn, newRow);
                                            playgroundForRotating[newColumn, newRow] = matrixArray3x3[columnRotating, rowRotating];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            FillPlaygroundForRotating();
                                            rotateSuccessfull = false;
                                            RotateBack();
                                            break;
                                        }                                  
                                    }
                                    newColumn++;
                                }                                
                                newColumn = column;
                                newRow++;
                            }
                            break;
                        }
                        else if (block[2].positionArrayX - 1 < 0 && block[2].positionArrayY - 1 == row)
                        {
                            int newColumn = 0;
                            int newRow = row;
                            for (int rowRotating = 0; rowRotating < 3; rowRotating++)
                            {
                                for (int columnRotating = 1; columnRotating < 3; columnRotating++)
                                {
                                    if (matrixArray3x3[columnRotating, rowRotating] != null && matrixArray3x3[columnRotating, rowRotating].Id != 5)
                                    {
                                        try
                                        {
                                            matrixArray3x3[columnRotating, rowRotating].positionXamlMargin = playgroundForRotating[newColumn, newRow].positionXamlMargin;
                                            matrixArray3x3[columnRotating, rowRotating].SetPosition(newColumn, newRow);
                                            playgroundForRotating[newColumn, newRow] = matrixArray3x3[columnRotating, rowRotating];
                                        }
                                        catch (IndexOutOfRangeException) {
                                            FillPlaygroundForRotating();
                                            rotateSuccessfull = false;
                                            RotateBack();
                                            break;
                                        }                            
                                    }
                                    newColumn++;
                                }
                                newColumn = 0;
                                newRow++;
                            }
                            break;
                        }
                    }
                }
            }

            // Hier wird überprüft, ob nach der Drehung des MatrixArrays es Überschneidungen mit dem aktuellen Spielfeld gibt. Falls ja, wird das MatrixArray wieder zurückgedreht und der Stein wird sich visuell nicht gedreht haben.
            for (int row = 0; row < 18; row++)
            {
                for (int column = 0; column < 11; column++)
                {
                    if (playgroundForRotating[column, row] != null && playgroundForRotating[column, row].Id != 0 && playgroundForRotating[column, row].Id != 5)
                    {
                        if (playground[column, row] != null)
                        {
                            FillPlaygroundForRotating();
                            rotateSuccessfull = false;
                            RotateBack();
                            break;
                        }                       
                    }
                }
            }

            //Aktuelles MatrixArray3x3/MatrixArray4x4 löschen, damit beim befüllen eines neuen, keine Probleme auftreten
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    matrixArray3x3[row, column] = null;
                }
            }
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    matrixArray4x4[row, column] = null;
                }
            }
            //Hier werden nun die einzelnen blocks dem playground wieder hinzugefügt.
            for (int row = 0; row < 18; row++)
            {
                for (int column = 0; column < 11; column++)
                {
                    //Es werden nur die 4 Blöcke des Tetrons abgefragt. Sobald das erste gefunden wurde, werden die IDs verglichen und dem playground hinzugefügt
                    if (playgroundForRotating[column, row] != null && playgroundForRotating[column, row].Id != 0 && playgroundForRotating[column, row].Id != 5)
                    {
                        for (int a = 0; a < block.Length; a++)
                        {
                            
                            if (block[a].Id == playgroundForRotating[column, row].Id)
                            {
                                playground[column, row] = block[a];
                            }
                        }
                    }
                }
            }
            //tetronMoving auf false, da der kritische Bereich verlassen wird.
            tetronMoving = false;
            //Wird ausgeführt, damit es für den nächsten Aufruf wieder normal gefüllt ist.
            FillPlaygroundForRotating();
            }

        // Wird ausgeführt, falls das Tetron keine Erlaubnis hatte sich zu drehen.
        private void RotateBack()
        {
            // Tetron-Typ 7 besitzt wieder besonderheiten, da es matrixArray4x4 benutzt.
            if(tetron.typeOfTetron != 7)
            {
                for (int row = 0; row < 3; row++)
                {
                    for (int column = 0; column < 3; column++)
                    {
                        if (matrixArray3x3[column, row] != null)
                        {
                            //Das matrixArray bekommt die Positionswerte nach der Drehung zurück, da es mit diesen zurückgedreht wird. Das ist wichtig dieses Zusatz-Array zu nutzen, da einige Steine bereits eine neue Position bekommen haben könnten in der Funktion "MatrixArrayToPlayground()". Des weiteren wird das Zusatz-Array auf null gesetzt, damit beim nächsten mal keine Probleme entstehen
                            matrixArray3x3[column, row].SetPosition(matrixArray3x3AfterRotating[column, row].positionArrayX, matrixArray3x3AfterRotating[column, row].positionArrayY);
                            matrixArray3x3AfterRotating[column, row] = null;
                        }
                    }
                }
            }
            else
            {
                for (int row = 0; row < 4; row++)
                {
                    for (int column = 0; column < 4; column++)
                    {
                        if (matrixArray4x4[column, row] != null)
                        {
                            //Das selbe wie in den oberen Zeilen, nur hier mit matrixArray4x4
                            matrixArray4x4[column, row].SetPosition(matrixArray4x4AfterRotating[column, row].positionArrayX, matrixArray4x4AfterRotating[column, row].positionArrayY);
                            matrixArray4x4AfterRotating[column, row] = null;
                        }
                    }
                }
            }

            //Es wird abgefragt, welche Richtung davor gedreht worden ist, um nun in die andere Richtung zu drehen.
            if (rotateDirection == "anticlockwise")
            {
                if (tetron.typeOfTetron != 7) RotateArrayAntiClockwise(matrixArray3x3, tetron.typeOfTetron);
                else RotateArrayAntiClockwise(matrixArray4x4, tetron.typeOfTetron);
                //Hier wird ebenfalls die rotatePosition wieder richtig gestellt
                if (tetron.rotatePosition != 0) tetron.rotatePosition = 2;
                MatrixArrayToPlayground();
            }
            if (rotateDirection == "clockwise")
            {
                if (tetron.typeOfTetron != 7) RotateArrayClockwise(matrixArray3x3, tetron.typeOfTetron);
                else RotateArrayClockwise(matrixArray4x4, tetron.typeOfTetron);
                //Hier wird ebenfalls die rotatePosition wieder richtig gestellt
                if (tetron.rotatePosition != 0) tetron.rotatePosition = 1;
                MatrixArrayToPlayground();
            }
        }


        // Die Funktion, welche das Tetron gegen den Uhrzeigersinn dreht
        private void RotateAntiClockwise()
        {
            tetronMoving = true;
            //Die rotateDirection ist wichtig, für den Fall, dass das Rotieren fehlerhaft sein sollte und zurückgedreht werden müsste.
            rotateDirection = "anticlockwise";
            if (tetron.typeOfTetron != 7)
            {
                PlaygroundToMatrixArray();
                matrixArray3x3 = RotateArrayClockwise(matrixArray3x3, tetron.typeOfTetron);
                // Das Zusatz-Array "matrixArray3x3AfterRotating" wird befüllt mit den jeweiligen x- und y-Positionen des Arrays, für den Fall, dass es zurück gedreht werden muss.
                for (int row = 0; row < 3; row++)
                {
                    for (int column = 0; column < 3; column++)
                    {
                        if (matrixArray3x3[column, row] != null)
                        {
                            matrixArray3x3AfterRotating[column, row] = new Block();
                            matrixArray3x3AfterRotating[column, row].positionArrayX = matrixArray3x3[column, row].positionArrayX;
                            matrixArray3x3AfterRotating[column, row].positionArrayY = matrixArray3x3[column, row].positionArrayY;
                        }
                    }
                }      
                for (int i = 0; i < 4; i++)
                    {
                        playground[block[i].positionArrayX, block[i].positionArrayY] = null;
                    }
                MatrixArrayToPlayground();
                // Die rotatePosition wird angepasst
                if (tetron.rotatePosition != 0) tetron.rotatePosition = 1;
                // Falls die Rotation erfolgreich war, wird nun das Tetron visuell gedreht und das Zusatz-Array wird wieder auf null gesetzt
                if (rotateSuccessfull)
                {
                    rotateTransform.CenterX = tetron.centerX;
                    rotateTransform.CenterY = tetron.centerY;
                    rotateTransform.Angle -= 90;
                    tetron.tetronCanvas.RenderTransform = rotateTransform;
                    for (int row = 0; row < 3; row++)
                    {
                        for (int column = 0; column < 3; column++)
                        {
                                matrixArray3x3AfterRotating[column, row] = null;
                        }
                    }
                }
            }
            // Gleiches wie bei if(tetron.typeOfTetron != 7) nur mit matrixArray4x4 und größerem Zusatz-Array
            else if (tetron.typeOfTetron == 7)
            {
                PlaygroundToMatrixArray();
                matrixArray4x4 = RotateArrayClockwise(matrixArray4x4, tetron.typeOfTetron);
                for (int row = 0; row < 4; row++)
                {
                    for (int column = 0; column < 4; column++)
                    {
                        if (matrixArray4x4[column, row] != null)
                        {
                            matrixArray4x4AfterRotating[column, row] = new Block();
                            matrixArray4x4AfterRotating[column, row].positionArrayX = matrixArray4x4[column, row].positionArrayX;
                            matrixArray4x4AfterRotating[column, row].positionArrayY = matrixArray4x4[column, row].positionArrayY;
                        }
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    playground[block[i].positionArrayX, block[i].positionArrayY] = null;
                }
                MatrixArrayToPlayground();
                if(rotateSuccessfull)
                {
                    rotateTransform.CenterX = tetron.centerX;
                    rotateTransform.CenterY = tetron.centerY;
                    rotateTransform.Angle -= 90;
                    tetron.tetronCanvas.RenderTransform = rotateTransform;
                    tetron.rotatePosition = 1;
                    for (int row = 0; row < 4; row++)
                    {
                        for (int column = 0; column < 4; column++)
                        {
                                matrixArray4x4AfterRotating[column, row] = null;
                        }
                    }
                }
            }
            rotateSuccessfull = true;
        }

        //Gleiche Funktionsweise wie bei RotateAntiClockwise
        private void RotateClockwise()
        {
            tetronMoving = true;
            rotateDirection = "clockwise";
            if (tetron.typeOfTetron != 1 && tetron.typeOfTetron != 7)
            {
                PlaygroundToMatrixArray();
                matrixArray3x3 = RotateArrayAntiClockwise(matrixArray3x3, tetron.typeOfTetron);
                for (int row = 0; row < 3; row++)
                {
                    for (int column = 0; column < 3; column++)
                    {
                        if (matrixArray3x3[column, row] != null)
                        {
                            matrixArray3x3AfterRotating[column, row] = new Block();
                            matrixArray3x3AfterRotating[column, row].positionArrayX = matrixArray3x3[column, row].positionArrayX;
                            matrixArray3x3AfterRotating[column, row].positionArrayY = matrixArray3x3[column, row].positionArrayY;
                        }
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    playground[block[i].positionArrayX, block[i].positionArrayY] = null;
                }
                MatrixArrayToPlayground();
                if (tetron.rotatePosition != 0) tetron.rotatePosition = 2;
                if (rotateSuccessfull)
                {
                    rotateTransform.CenterX = tetron.centerX;
                    rotateTransform.CenterY = tetron.centerY;
                    rotateTransform.Angle += 90;
                    tetron.tetronCanvas.RenderTransform = rotateTransform;
                    for (int row = 0; row < 3; row++)
                    {
                        for (int column = 0; column < 3; column++)
                        {
                                matrixArray3x3AfterRotating[column, row] = null;
                        }
                    }
                }
            }
            else if(tetron.typeOfTetron == 7)
            {

                PlaygroundToMatrixArray();
                matrixArray4x4 = RotateArrayAntiClockwise(matrixArray4x4, tetron.typeOfTetron);
                for (int row = 0; row < 4; row++)
                {
                    for (int column = 0; column < 4; column++)
                    {
                        if (matrixArray4x4[column, row] != null)
                        {
                            matrixArray4x4AfterRotating[column, row] = new Block();
                            matrixArray4x4AfterRotating[column, row].positionArrayX = matrixArray4x4[column, row].positionArrayX;
                            matrixArray4x4AfterRotating[column, row].positionArrayY = matrixArray4x4[column, row].positionArrayY;
                        }
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    playground[block[i].positionArrayX, block[i].positionArrayY] = null;
                }

                MatrixArrayToPlayground();
                if (rotateSuccessfull)
                {
                    rotateTransform.CenterX = tetron.centerX;
                    rotateTransform.CenterY = tetron.centerY;
                    rotateTransform.Angle += 90;
                    tetron.tetronCanvas.RenderTransform = rotateTransform;
                    tetron.rotatePosition = 2;
                    for (int row = 0; row < 3; row++)
                    {
                        for (int column = 0; column < 3; column++)
                        {
                                matrixArray4x4AfterRotating[column, row] = null;
                        }
                    }
                }
            }
            rotateSuccessfull = true;
        }

        //Das MatrixArray wird einmal logisch um den Uhrzeigersinn gedreht
        Block[,] RotateArrayClockwise(Block[,] mat, int typeOfTetron)
        {
            int n;
            if (typeOfTetron != 7) n = 3;
            else n = 4;
            for (int i = 0; i < n / 2; i++)
            {
                for (int j = i; j < n - i - 1; j++)
                {
                    Block temp = mat[i, j];
                    mat[i, j] = mat[n - 1 - j, i];
                    mat[n - 1 - j, i] = mat[n - 1 - i, n - 1 - j];
                    mat[n - 1 - i, n - 1 - j] = mat[j, n - 1 - i];
                    mat[j, n - 1 - i] = temp;
                }
            }
            return mat;
        }

        //Das MatrixArray wird einmal logisch gegen den Uhrzeigersinn gedreht
        Block[,] RotateArrayAntiClockwise(Block[,] mat, int typeOfTetron)
        {
            int n;
            if (typeOfTetron != 7) n = 3;
            else n = 4;
            for (int x = 0; x < n / 2; x++)
            {
                for (int y = x; y < n - x - 1; y++)
                {
                    Block temp = mat[x, y];
                    mat[x, y] = mat[y, n - 1 - x];
                    mat[y, n - 1 - x] = mat[n - 1 - x, n - 1 - y];
                    mat[n - 1 - x,  n - 1 - y] = mat[n - 1 - y, x];
                    mat[n - 1 - y, x] = temp;
                }
            }
            return mat;
        }



        // Die einzelnen Assets für "Next Tetrons" und "Holding"
        public BitmapImage GetImageSource(int blockType)
        {
            BitmapImage imageSource = null;
            switch (blockType)
            {
                case 1:
                    imageSource = new BitmapImage(new Uri("ms-appx:/Assets/Tetrons/blueTetron.png"));
                    return imageSource;

                case 2:
                    imageSource = new BitmapImage(new Uri("ms-appx:/Assets/Tetrons/greenTetron.png"));
                    return imageSource;

                case 3:
                    imageSource = new BitmapImage(new Uri("ms-appx:/Assets/Tetrons/orangeTetron.png"));
                    return imageSource;

                case 4:
                    imageSource = new BitmapImage(new Uri("ms-appx:/Assets/Tetrons/purpleTetron.png"));
                    return imageSource;

                case 5:
                    imageSource = new BitmapImage(new Uri("ms-appx:/Assets/Tetrons/redTetron.png"));
                    return imageSource;

                case 6:
                    imageSource = new BitmapImage(new Uri("ms-appx:/Assets/Tetrons/turkisTetron.png"));
                    return imageSource; ;

                case 7:
                    imageSource = new BitmapImage(new Uri("ms-appx:/Assets/Tetrons/yellowTetron.png"));
                    return imageSource;
            }
            return imageSource;
        }



        //Die ButtonClick-Funktion, falls das Spiel im Pause-Modus ist und weitergespielt werden soll.
        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            music.Play_ButtonClick();
            music.mainTheme.Play();
            PauseScreen.Visibility = Visibility.Collapsed;
        }

        //Die ButtonClick-Funktion, um einen neuen Highscore hinzuzufügen
        private async void NewHighscore_Click(object sender, RoutedEventArgs e)
        {
            //Der neue Wert wird dem highscore hinzugefügt
            await highscore.NewHighscore(score.points, NameInput.Text);
            //Das Spielfeld wird komplett auf null gesetzt
            for (int row = 0; row < 18; row++)
            {
                for (int column = 0; column < 11; column++)
                {
                    playground[column, row] = null;
                }
            }
            PlayfieldXAML.Visibility = Visibility.Collapsed;
            this.Frame.Navigate(typeof(MainPage));
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            //Das Spielfeld wird komplett auf null gesetzt
            for (int row = 0; row < 18; row++)
            {
                for (int column = 0; column < 11; column++)
                {
                    playground[column, row] = null;
                }
            }
            PlayfieldXAML.Visibility = Visibility.Collapsed;
            music.Play_ButtonClick();
            this.Frame.Navigate(typeof(MainPage));
        }

        public void Navigate_MainMenu(object sender, RoutedEventArgs e)
        {
            for (int row = 0; row < 18; row++)
            {
                for (int column = 0; column < 11; column++)
                {
                    playground[column, row] = null;
                }
            }
            timer.Stop();
            music.Play_ButtonClick();
            music.mainTheme.Pause();
            PlayfieldXAML.Visibility = Visibility.Collapsed;
            this.Frame.Navigate(typeof(MainPage));
        }

        private void NameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Eine Überprüfung, ob der NameInput ungültige Zeichen oder eine Überlänge besitzt. Falls ja, werden diese Zeichen entfernt
            if (NameInput.Text != "" && (NameInput.Text.Contains("$") || NameInput.Text.Contains("%") || NameInput.Text.Contains("§") || NameInput.Text.Length > 6))
            {
                NameInput.Text = NameInput.Text.Substring(0, NameInput.Text.Length-1);
            }
        }

        private void ButtonVolumeHigher_Click(object sender, RoutedEventArgs e)
        {
            //Das erhöhen der Lautstärke. In einem try-catch, da es gelegentlich zu eienr FormatException kommen kann.
            try
            {
                //Die Lautstärke kann nur vergrößert werden, wenn sie unter 100 liegt
                if (int.Parse(MusicVolume.Text) < 100)
                {
                    music.volume += 0.05;
                    //Runden, da es sonst zu einem overflow kommen könnte
                    music.volume = Math.Round(music.volume, 2);
                    //Multiplikation mit 100, da der Wert mit 2 nachkomma Stellen gespeichert und benutzt werden
                    MusicVolume.Text = music.volume * 100 + "";
                    music.Change_Volume("GamePage");
                    music.Play_ButtonClick();
                }
            }
            catch (FormatException exc)
            {
            }

        }

        private void ButtonVolumeLower_Click(object sender, RoutedEventArgs e)
        {
            //Selbes wie bei ButtonVolumeHigher_Click
            try
            {
                //Die Lautstärke kann nur verkleinert werden, wenn sie über 0 liegt
                if (int.Parse(MusicVolume.Text) > 0)
                {
                    music.volume -= 0.05;
                    music.volume = Math.Round(music.volume, 2);
                    MusicVolume.Text = music.volume * 100 + "";
                    music.Change_Volume("GamePage");
                    music.Play_ButtonClick();
                }
            }
            catch (FormatException exc)
            {
            }
        }

        private void SaveVolume_Click(object sender, RoutedEventArgs e)
        {
            //Die Lautstärke wird in das dazugehörige File gespeichert
            try
            {
                double newVolume = Double.Parse(MusicVolume.Text);
                newVolume /= 100;
                //Runden, da es sonst zu einem overflow kommen könnte
                newVolume = Math.Round(newVolume, 2);
                _ = music.SaveVolume(newVolume);
                music.Play_ButtonClick();
            }
            catch (FormatException exc)
            {
            }
        }    

    }
}