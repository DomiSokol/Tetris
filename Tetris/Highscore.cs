using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Tetris;
using Windows.Foundation;
using Windows.Storage;

public class Highscore
{

    private string highscoreString = "$Joseph$§999999§%05.06.2021%$Jonas$§555553§%13.02.2020%$Alex$§123213§%14.03.2021%$Lonki$§120543§%24.12.2020%$Cainon$§65123§%01.01.2020%$Choc$§55555§%04.06.2021%$CupYay$§42069§%01.02.2020%$Zelda$§70§%24.06.2021%$Noob$§63§%24.06.2021%";
    private string newHighscore;
    private string originHighscore = "$Joseph$§999999§%05.06.2021%$Jonas$§555553§%13.02.2020%$Alex$§123213§%14.03.2021%$Lonki$§120543§%24.12.2020%$Cainon$§65123§%01.01.2020%$Choc$§55555§%04.06.2021%$CupYay$§42069§%01.02.2020%$Zelda$§70§%24.06.2021%$Noob$§63§%24.06.2021%";
    public int beginning;
    private int index;
    private int listIndex;
    private int end;
    public List<Highscore> highscoreList = new List<Highscore>();
    public DateTime date;
    public int scored {get; set; }
    public string name { get; set; }


    public Highscore()
    {
    }

    public async Task GetNewHighscoreList()
    {
        //Es wird versucht die Datei zu finden und auszulesen, falls es nicht klappt, wird eine FileNotFoundException geworfen, aufgefangen und eine neue Datei erstellt
        //Die Funktionsweise, um mehrer Attribute zu speichern, ist, dass der char "$" jeweils den Namen enthält, der char "$" die Punktzahl und der char "%" das Datum
        //Diese Substrings werden dann in das Array "highscoreList" an die jeweilige Attribut-Stelle hinzugefügt
        try
        {
            highscoreList = new List<Highscore>();
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("highscore.txt");
            string speicher = await FileIO.ReadTextAsync(file);
            while (end < speicher.LastIndexOf("$"))
            {
                highscoreList.Add(new Highscore());
                beginning = speicher.IndexOf("$", index) + 1;
                end = speicher.IndexOf("$", beginning);
                index = end + 1;
                highscoreList[listIndex].name = speicher.Substring(beginning, end - beginning);
                listIndex++;
            }
            beginning = 0;
            end = 0;
            index = 0;
            listIndex = 0;
            while (end < speicher.LastIndexOf("§"))
            {
                beginning = speicher.IndexOf("§", index) + 1;
                end = speicher.IndexOf("§", beginning);
                index = end + 1;
                try
                {
                    highscoreList[listIndex].scored = Int32.Parse(speicher.Substring(beginning, end - beginning));
                }
                catch (FormatException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                }
                listIndex++;
            }
            beginning = 0;
            end = 0;
            index = 0;
            listIndex = 0;
            while (end < speicher.LastIndexOf("%"))
            {
                beginning = speicher.IndexOf("%", index) + 1;
                end = speicher.IndexOf("%", beginning);
                index = end + 1;
                highscoreList[listIndex].date = Convert.ToDateTime(speicher.Substring(beginning, end - beginning));
                listIndex++;
            }
            //Wenn alle Substrings abgespeichert wurden, werden diese nun aus dem Array heraus in den neuenHighscore als String hinzugefügt
            for (int i = 0; i < highscoreList.Count; i++)
            {
                newHighscore += "$" + highscoreList[i].name + "$";
                newHighscore += "§" + highscoreList[i].scored + "§";
                newHighscore += "%" + highscoreList[i].date.ToString("dd.MM.yyyy") + "%";
            }
            highscoreList = highscoreList.OrderByDescending(x => x.scored).ToList();
            highscoreString = newHighscore;
            newHighscore = "";
        }
        catch (FileNotFoundException e)
        {
            await CreateHighscoreFile();
        }
    }

    public async Task NewHighscore(int score, string name)
    {
        // Zuerst wird die highscoreList sortiert, der letzte mit dem neuen ersetzt, die Attribute hinzugefügt, dem String ebenfalls hinzugefügt und die Funktion zum speichern des Files aufgerufen.
        highscoreList = highscoreList.OrderByDescending(x => x.scored).ToList();
        highscoreList.Remove(highscoreList.Last<Highscore>());
        highscoreList.Add(new Highscore());
        highscoreList.Last<Highscore>().scored = score;
        highscoreList.Last<Highscore>().name = name;
        //Es wird das aktuelle Datum als Datums-Wert genommen
        highscoreList.Last<Highscore>().date = DateTime.Today;
        for (int i = 0; i < highscoreList.Count; i++)
        {
            newHighscore += "$" + highscoreList[i].name + "$";
            newHighscore += "§" + highscoreList[i].scored + "§";
            newHighscore += "%" + highscoreList[i].date.ToString("dd.MM.yyyy") + "%";
        }
        highscoreString = newHighscore;
        newHighscore = "";
        await SaveHighscore();
    }

    public async Task SaveHighscore()
    {
        StorageFolder folder = ApplicationData.Current.LocalFolder;
        StorageFile file = await folder.CreateFileAsync("highscore.txt", CreationCollisionOption.OpenIfExists);
        await FileIO.WriteTextAsync(file, highscoreString);
    }

    public async Task SetOriginHighscore()
    {
        //Die vorherige HighscoreList widerherstellen
        highscoreString = originHighscore;
        StorageFolder folder = ApplicationData.Current.LocalFolder;
        StorageFile file = await folder.CreateFileAsync("highscore.txt", CreationCollisionOption.OpenIfExists);
        await FileIO.WriteTextAsync(file, originHighscore);
    }

    public async Task CreateHighscoreFile()
    {
        StorageFolder folder = ApplicationData.Current.LocalFolder;
        StorageFile file = await folder.CreateFileAsync("highscore.txt");
        await FileIO.WriteTextAsync(file, originHighscore);
        await GetNewHighscoreList();
    }


    public List<Highscore> GetHighscoreList()
    {
        // Die highscoreList in geordneter Art zurückgegeben
        highscoreList = highscoreList.OrderByDescending(x => x.scored).ToList();
        return highscoreList;
    }
}