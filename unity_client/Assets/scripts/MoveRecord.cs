using UnityEngine;
using System.Collections;

public class MoveRecord {

    public string name { get; set; }
    public int new_posx { get; set; }
    public int new_posy { get; set; }

    public MoveRecord()
    {

    }

    public MoveRecord(string _name, int _px, int _py)
    {
        name = _name;
        new_posx = _px;
        new_posy = _py;
    }

}
