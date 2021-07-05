using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.Media.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.System;
using Windows.UI.Xaml.Media.Animation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace Tetris
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int left = 363;
        private int right = 1265;
        private int top = 395;
        private int bottom = 618;
        private Highscore highscore;
        TextBlock[,] scoreFiled = new TextBlock[9, 3];
        Music music;

        public MainPage()
        {
            this.InitializeComponent();
            music = new Music();
            _ = music.GetVolume("MainPage");
            highscore = new Highscore();
            _ = highscore.GetNewHighscoreList();       
            //Angenehmer für das Spielerlebnis
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
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
                    music.volume = Math.Round(music.volume, 2);
                    MusicVolume.Text = music.volume * 100 + "";
                    music.Change_Volume("MainPage");
                    music.Play_ButtonClick();
                }

            }
            catch(FormatException exc)
            {
            }

        }

        private void ButtonVolumeLower_Click(object sender, RoutedEventArgs e)
        {
            //Das erhöhen der Lautstärke. In einem try-catch, da es gelegentlich zu eienr FormatException kommen kann.
            try
            {
                //Die Lautstärke kann nur verkleinert werden, wenn sie über 0 liegt
                if (int.Parse(MusicVolume.Text) > 0)
                {
                    music.volume -= 0.05;
                    music.volume = Math.Round(music.volume, 2);
                    MusicVolume.Text = music.volume * 100 + "";
                    music.Change_Volume("MainPage");
                    music.Play_ButtonClick();
                }
            }
            catch(FormatException exc)
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



        //Die Highscore-Liste wird generiert mit den jeweiligen Werten.
        private void Highscore_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            HighscoreList_Grid.Visibility = Visibility.Visible;
            music.Play_ButtonClick();
            music.mainMenuMusic.Pause();
            music.Play_Highscore();
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    scoreFiled[row, column] = new TextBlock();
                    scoreFiled[row, column].Margin = new Thickness(left, top, right, bottom);
                    scoreFiled[row, column].FontSize = 50;
                    scoreFiled[row, column].FontFamily = new FontFamily("/Assets/Font/NeonTubes2.otf#Neon Tubes 2");
                    //Designentscheidungen: Punktzahl mit rechtem Alignment. Sowie die einzelnen Datentypen zu String konvertieren.
                    if (column == 0)
                    {
                        scoreFiled[row, column].TextAlignment = TextAlignment.Right;
                        scoreFiled[row, column].Text = highscore.highscoreList[row].scored.ToString();
                    }
                    else if (column == 1)
                    {
                        scoreFiled[row, column].Text = highscore.highscoreList[row].name;
                    }
                    else
                    {
                        scoreFiled[row, column].Text = highscore.highscoreList[row].date.ToString("dd.MM.yyyy");
                    }
                    HighscoreList_Grid.Children.Add(scoreFiled[row, column]);
                    left += 447;
                    right -= 447;
                }
                left = 363;
                right = 1265;
                top += 66;
                bottom -= 66;
            }
            left = 363;
            right = 1265;
            top = 395;
            bottom = 618;
        }


        private void HighscorePageClose_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Visible;
            HighscoreList_Grid.Visibility = Visibility.Collapsed;
            music.Play_ButtonClick();
            music.mainMenuMusic.Play();
            music.highscoreMusic.Pause();
            // HighscoreList wieder auf null setzen, da bei erneutem öffnen Fehler entstehen können
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    HighscoreList_Grid.Children.Remove(scoreFiled[row, column]);
                    scoreFiled[row, column] = null;
                }
            }
        }

        //Beenden der Applikation
        private void Schließen_Click(object sender, RoutedEventArgs e)
        {
            music.Play_ButtonClick();
            Application.Current.Exit();
        }

        // Das zurücksetzen des Highscores
        private async void Highscore_Reset_Click(object sender, RoutedEventArgs e)
        {
            await highscore.SetOriginHighscore();
            MainMenu.Visibility = Visibility.Visible;
            HighscoreList_Grid.Visibility = Visibility.Collapsed;
            music.Play_ButtonClick();
            music.highscoreMusic.Pause();
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    HighscoreList_Grid.Children.Remove(scoreFiled[row, column]);
                    scoreFiled[row, column] = null;
                }
            }
            // Zum neuladen der Seite, damit die neue Highscore-Datei abgerufen wird und nicht die alte weiterhin abgespeichert ist. Dies ist wichtig, dass wenn der User wieder auf Highscore klickt, die Originale Liste zu sehen bekommt
            this.Frame.Navigate(typeof(MainPage));
        }

        // Das Starten des Spiels
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            music.Play_ButtonClick();
            music.mainMenuMusic.Pause();
            this.Frame.Navigate(typeof(GamePage));
        }

        
        
        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            Credits_Grid.Visibility = Visibility.Visible;
            music.Play_ButtonClick();
            music.mainMenuMusic.Pause();
            music.Play_CreditMusic();
            Storyboard1.Begin();
        }


        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            Settings_Grid.Visibility = Visibility.Visible;
            music.Play_ButtonClick();
            MusicVolume.Text = music.volume * 100 + "";
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        public void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {           
            switch (args.VirtualKey)
            {
                // Zum Beenden des Storyboards und dem "schließen" der Credits
                case VirtualKey.Enter:
                    if(Credits_Grid.Visibility == Visibility.Visible)
                    {
                        Storyboard1.Stop();
                        MainMenu.Visibility = Visibility.Visible;
                        Credits_Grid.Visibility = Visibility.Collapsed;
                        music.creditMusic.Pause();
                        music.Play_MainMenu();
                        break;
                    }
                    break;
            }
        }

        private void Settings_Close_Button_Click(object sender, RoutedEventArgs e)
        {
            Settings_Grid.Visibility = Visibility.Collapsed;
            MainMenu.Visibility = Visibility.Visible;
            music.Play_ButtonClick();
        }

    }
}

