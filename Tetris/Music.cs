using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml;

/*
 * Credits für die Musik gehen an:
 * 
 * 
 * The Essential Retro Video Game Sound Effects Collection [512 sounds] By Juhani Junkala |||  opengameart.org
 * “Invention 4” by Johann Sebastian Bach Chiptune rendition by Haley Halcyon ||| opengameart.org (MainTheme)
 * https://www.fesliyanstudios.com/royalty-free-music/downloads-c/8-bit-music/6 (MainMenu)
 * https://pixabay.com/music/epic-classical-inmenso-charles-michel-1771/ (Credits)
 * https://dominik-braun.net/retro-sounds/ (Sound-Effects)
 * 
 * 
 * Einzelne Musikstücke wurden gekürzt und alle wurden auf eine ungefähr gleiche Grundlautstärke gepegelt
 */


public class Music

{
    // Die einzelnen MediaPlayer, für die einzelnen Musik-Stücke/Sounds
    // Manche davon als public um ein einfacheres Play()/Pause() zu ermöglichen in den Klassen
    public MediaPlayer mainTheme = new MediaPlayer();
    MediaPlayer linesDestroyedEffect = new MediaPlayer();
    MediaPlayer pauseEffect = new MediaPlayer();
    MediaPlayer levelUpEffect = new MediaPlayer();
    MediaPlayer gameOverMusic = new MediaPlayer();
    MediaPlayer buttonClick = new MediaPlayer();
    public MediaPlayer creditMusic = new MediaPlayer();
    public MediaPlayer highscoreMusic = new MediaPlayer();
    MediaPlayer highscoreEffect = new MediaPlayer();
    public MediaPlayer mainMenuMusic = new MediaPlayer();
    public double volume;


    public Music()
    {
    }

    //Das verändern der jeweiligen Lautstärke des Musikstückes, wenn diese geändert wird durch den User.
    public void Change_Volume(String page)
    {
        if(page == "MainPage")
        {
            mainMenuMusic.Volume = volume;
        }
        else
        {
            mainTheme.Volume = volume;
        }
    }

    //Wichtig ist bei den meisten Musikstücken, dass LoopingEnabeld wird, da es in Dauerschleif abgespielt werden soll.
    public async void Play_MainTheme()
    {
        StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Music");
        StorageFile file = await folder.GetFileAsync("MainTheme.mp3");
        mainTheme.Source = MediaSource.CreateFromStorageFile(file);
        mainTheme.Volume = volume;
        mainTheme.Play();
        mainTheme.IsLoopingEnabled = true;
    }


    public async void Play_Highscore()
    {
        StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Music");
        StorageFile file = await folder.GetFileAsync("HighscoreList.mp3");
        highscoreMusic.Source = MediaSource.CreateFromStorageFile(file);
        highscoreMusic.Volume = volume;
        highscoreMusic.Play();
        highscoreMusic.IsLoopingEnabled = true;
    }

    public async void Play_MainMenu()
    {
        StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Music");
        StorageFile file = await folder.GetFileAsync("MainMenu.mp3");
        mainMenuMusic.Source = MediaSource.CreateFromStorageFile(file);
        mainMenuMusic.Volume = volume;
        mainMenuMusic.Play();
        mainMenuMusic.IsLoopingEnabled = true;
    }


    public async void Play_CreditMusic()
    {
        StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Music");
        StorageFile file = await folder.GetFileAsync("CreditsShortend.mp3");
        creditMusic.Source = MediaSource.CreateFromStorageFile(file);
        creditMusic.Volume = volume;
        creditMusic.Play();
    }

    public async void Play_LinesDestroyed()
    {
        StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Music");
        StorageFile file = await folder.GetFileAsync("LinesDestroyed.mp3");
        linesDestroyedEffect.Source = MediaSource.CreateFromStorageFile(file);
        linesDestroyedEffect.Volume = volume;
        linesDestroyedEffect.Play();
    }

    public async void Play_LevelUp()
    {
        StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Music");
        StorageFile file = await folder.GetFileAsync("LevelUp.mp3");
        levelUpEffect.Source = MediaSource.CreateFromStorageFile(file);
        mainTheme.Pause();
        levelUpEffect.Volume = volume;
        levelUpEffect.Play();
        //Hier wird eine Funktion aufgerufen, wenn der LevelUpEffect fertig ist
        levelUpEffect.MediaEnded += SoundEffect_MediaEnded;
    }

    public async void Play_PauseMusic()
    {
        StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Music");
        StorageFile file = await folder.GetFileAsync("Pause.wav");
        pauseEffect.Source = MediaSource.CreateFromStorageFile(file);
        mainTheme.Pause();
        pauseEffect.Volume = volume;
        pauseEffect.Play();
    }


    public async void Play_GameOverMusic()
    {
        StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Music");
        StorageFile file = await folder.GetFileAsync("GameOver.mp3");
        gameOverMusic.Source = MediaSource.CreateFromStorageFile(file);
        gameOverMusic.Volume = volume;
        mainTheme.Pause();
        gameOverMusic.Play();
    }

    public async void Play_NewHighscore()
    {
        StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Music");
        StorageFile file = await folder.GetFileAsync("NewHighscore.mp3");
        highscoreEffect.Source = MediaSource.CreateFromStorageFile(file);
        highscoreEffect.Volume = volume;
        highscoreEffect.Play();
    }


    public async void Play_ButtonClick()
    {
        StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Music");
        StorageFile file = await folder.GetFileAsync("Button_Click.wav");
        buttonClick.Source = MediaSource.CreateFromStorageFile(file);
        buttonClick.Volume = volume;
        buttonClick.Play();
    }


    public void SoundEffect_MediaEnded(MediaPlayer sender, object args)
    {
        mainTheme.Play();
    }

    public void Stop_BackgroundMusic()
    {
        mainTheme.Pause();
        mainTheme = null;
    }


    public async Task GetVolume(String currentPage)
    {
        // Es wird versucht, das die Datei mit der Lautstärke zu finden und zu öffnen. Falls dies nicht der Fall sein sollte, soll wird eine FileNotFoundException geworfen und ein neues File erstellt
        try
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("volume.txt");
            string volumeString = await FileIO.ReadTextAsync(file);
            volume = double.Parse(volumeString);
            if(currentPage == "MainPage")
            {
                Play_MainMenu();
            }
            else
            {
                Play_MainTheme();
            }
        }
        catch(FileNotFoundException e)
        {
            await CreateVolumeFile(currentPage);
        }
    }

    //Die neue Lautstärke wird in die Datei geschrieben
    public async Task SaveVolume(double newVolume)
    {
        StorageFolder folder = ApplicationData.Current.LocalFolder;
        StorageFile file = await folder.CreateFileAsync("volume.txt", CreationCollisionOption.OpenIfExists);
        await FileIO.WriteTextAsync(file, newVolume + "");
    }

    //Die Datei mit der Lautstärke wird mit dem Standardwert 0,5 erstellt
    private async Task CreateVolumeFile(String currentPage)
    {
        StorageFolder folder = ApplicationData.Current.LocalFolder;
        StorageFile file = await folder.CreateFileAsync("volume.txt");
        await FileIO.WriteTextAsync(file, "0,5");
        await GetVolume(currentPage);
    }
}
