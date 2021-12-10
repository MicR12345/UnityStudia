using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//All utility classes used in multiple scripts are here
public class ThemedBlockList
{
    public string type;
    public List<Block> blocks;
    public ThemedBlockList(string _type, List<Block> _blocks)
    {
        type = _type;
        blocks = _blocks;
    }
}
