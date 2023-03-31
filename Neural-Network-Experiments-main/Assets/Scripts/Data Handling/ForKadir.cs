using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForKadir : MonoBehaviour
{
    [SerializeField] string[] labelNames;

    // Start is called before the first frame update
    void Start()
    {
        labelNames = new string[62];
        int i = 0;
        for(; i<=9; i++)
        {
            labelNames[i] = ChangeIntToString(i);
        }

        for(;i<=35;i++)
        {
            labelNames[i] = ChangeIntToChar(i);
        }

        for(;i<=61;i++)
        {
            labelNames[i] = ChangeIntToSmallChar(i);
        }
    }

    string ChangeIntToString(int a)///////////////////////////////////////////////////////////////////
    {
        if (a == 0) return "Zero";
        else if (a == 1) return "One";
        else if (a == 2) return "Two";
        else if (a == 3) return "Three";
        else if (a == 4) return "Four";
        else if (a == 5) return "Five";
        else if (a == 6) return "Six";
        else if (a == 7) return "Seven";
        else if (a == 8) return "Eight";
        else return "Nine";
    }

    string ChangeIntToChar(int i)
    {
        int characterAscii = i + 55;
        char a = (char)characterAscii;
        string s = a.ToString();
        return s;
    }
    private int GetImageLabelOutput(Image image)
    {
        int imgLabel = image.label;
        char c = (char)image.label;
        Debug.Log(c + " my char");
        // Debug.Log(image.label);
       // display.texture = image.ConvertToTexture2D();
        if (image.label >= 48 && image.label <= 57)
        {
            imgLabel = image.label - 48;
        }
        else if (image.label >= 65 && image.label <= 90)
        {
            imgLabel = image.label - 55;
        }
        else if (image.label >= 97 && image.label <= 122)
        {
            imgLabel = image.label - 61;
        }

        return imgLabel;
    }
    string ChangeIntToSmallChar(int i)
    {
        int characterAscii = i + 61;
        char a = (char)characterAscii;
        string s = a.ToString();
        return s;
    }

}
