using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TypesFile", menuName = "Create TypesFile")]

public class TypeOfItemScript : ScriptableObject
{
    public List<string> typesOfItem;

    public List<string> nameOfCategoryForMainWindow;
    public List<string> secondNameOfCategoryForMainWindow;
    public List<Sprite> imageOfCategoryForMainWindow;
}