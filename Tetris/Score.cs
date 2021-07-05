using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Score
{
    public int combo { get; set; }
    public int rowsCompleted { get; set; }
    public int points;
    public int level { get; set; }


    public void ChangeLevel()
    {
        level = Convert.ToInt32(Math.Floor(rowsCompleted / 10.0));
    }

}
