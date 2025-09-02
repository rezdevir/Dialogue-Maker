// Drop this in a .cs file. Works in Unity (Color) and in plain .NET (System.Drawing).
// In .NET, define USE_SYSTEM_DRAWING; in Unity, do nothing (uses UnityEngine.Color).


using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Col = System.Drawing.Color;

public  static class ColorValidation
{

    static readonly Regex Hex3 = new Regex(@"^#([0-9A-Fa-f]{3})$", RegexOptions.Compiled);
    static readonly Regex Hex6 = new Regex(@"^#([0-9A-Fa-f]{6})$", RegexOptions.Compiled);
    static readonly Regex Hex8 = new Regex(@"^#([0-9A-Fa-f]{8})$", RegexOptions.Compiled);

    public static  bool TryParse(string input)
    {
   

        if (string.IsNullOrWhiteSpace(input))
            return false;

        input = input.Trim();

       
        if (TryParseHex(input))
            return true;
        
        if( TryParsColorName(input))
            return true;
           
        
        return false;   
           
    }


    


     static bool TryParsColorName(string s)
    {

        //List<string> ColorNames= new List<string>();    
        for (int i=0; i < typeof(Colors).GetProperties().Length;i++)
        {
           if(s.ToLower().Equals( typeof(Colors).GetProperties()[i].Name.ToLower()))
              {
                return true;    
            }

        }

        return false;
    }
    static bool TryParseHex(string s)
    {
    

        Match m;
        if ((m = Hex3.Match(s)).Success)
        {
  
            return true;
        }
        if ((m = Hex6.Match(s)).Success)
        {

            return true;
        }
        if ((m = Hex8.Match(s)).Success)
        {

            return true;
        }
        return false;
    }

}
